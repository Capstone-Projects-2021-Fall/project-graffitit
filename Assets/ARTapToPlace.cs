using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using System;
using UnityEngine.XR.ARSubsystems;
using Amazon.S3Control.Model;
using Amazon.S3;
using Amazon.CognitoIdentity;
using Amazon;
using Amazon.S3.Model;
using UnityEngine.SceneManagement;
using System.Threading;

public class ARTapToPlace : MonoBehaviour
{
    public GameObject objectToPlace;
    public GameObject placementIndicator;
    private ARRaycastManager raycastManager;
    private ARSessionOrigin arOrigin;
    private Pose placementPose;
    private bool placementPoseIsValid = false;
    private float latitude;
    private float longitude;
    private Canvas canvas;
    public static bool hideCanvas;
    public IAmazonS3 client;

    // Start is called before the first frame update
    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        raycastManager = FindObjectOfType<ARRaycastManager>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        if(hideCanvas)
        {
            canvas.enabled = false;
        } 
        else
        {
            canvas.enabled = true;
        } 
        CognitoAWSCredentials credentials = new CognitoAWSCredentials(
        "us-east-2:0b2ab95e-b5ce-4a6e-ba47-9d2ef2a72b7e", // Identity pool ID
        RegionEndpoint.USEast2 // Region
        );

        client = new AmazonS3Client(credentials, RegionEndpoint.USEast2);
        //getListFilesInBucket((AmazonS3Client)client);
        InvokeRepeating("startGPS", 3f, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PlaceObject();
        }
    }

    private void PlaceObject()
    {
        Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
        if(canvas.enabled == true)
        {
            Destroy(canvas);
            Debug.Log("TexturePlane in the AR Scene: " + objectToPlace.transform.Find("TexturePlane") is null);
            Debug.Log("PhoneCamera temTexture: " + PhoneCamera.temTexture is null);
            //objectToPlace.transform.Find("TexturePlane").GetComponent<Renderer>().material.mainTexture = PhoneCamera.temTexture;
            //SceneManager.LoadScene("TemHomePage");
        }
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        raycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;

            var cameraForward = Camera.main.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }

    }
    public void startGPS()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("The user did not enable location service");
        }
        else
        {
            Input.location.Start(5.0f, 5.0f);
        }
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Detect location failed");
        }
        else
        {
   
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;
            Debug.Log("latitude in AR Scene: " + latitude + "longitude in AR Scene: " + longitude);
        }
    }

    //use to send the additional data about the recorded audiovisual content
    //required client, filename/key
    public void updateS3ObjectMeta(AmazonS3Client client, string key)
    {
        //use CopyObjectRequest
        //use .Metadata.Add
        //put in the pose.position
        //put in the pose.rotation
        //issue the request with client.copyObject(request)

        CopyObjectRequest copyRequest = new CopyObjectRequest
        {
            SourceBucket = "my-graffitit-s3-bucket",
            SourceKey = key,
            DestinationBucket = "my-graffitit-s3-bucket",
            DestinationKey = key,
            MetadataDirective = S3MetadataDirective.REPLACE,
            CannedACL = S3CannedACL.PublicRead
        };
        copyRequest.Metadata.Add("AR-Position-Rotation", placementPose.position.ToString()+ "|" + placementPose.rotation.ToString());
        client.CopyObject(copyRequest);
    }

    //This function allows the applicatoin to get all the keys in the S3
    public string[] getListFilesInBucket(AmazonS3Client client)
    {
        List<string> filesList = new List<string>();
        var listResponse = client.ListObjects("my-graffitit-s3-bucket");
        foreach (S3Object obj in listResponse.S3Objects)
        {
            filesList.Add(obj.Key);
        }
        return filesList.ToArray();
    }

    //This function allows us to filter out keys that have location data attached
    public Dictionary<string, string> getLocationStringsOnServer(string[] files, AmazonS3Client client)
    {
        Dictionary<string, string> filesAndLocation = new Dictionary<string, string>();

        for (int i = 0; i < files.Length; i++)
        {
            var request = new GetObjectMetadataRequest()
            {
                BucketName = "my-graffitit-s3-bucket",
                Key = files[i],
            };
            var response = client.GetObjectMetadata(request);
            string location = response.Metadata["x-amz-meta-location-info"];
            if (location != null)
                filesAndLocation.Add(files[i], location);
        }
        return filesAndLocation;
    }

    //This function will be used to get the position and the rotation data about the files that are
    //nearby the user currently
    public Dictionary<string, string> getARPositionAndRotation(string[] files, AmazonS3Client client)
    {
        Dictionary<string, string> filteredFiles = new Dictionary<string, string>();
        for(int i = 0; i < files.Length; i++)
        {
            var request = new GetObjectMetadataRequest()
            {
                BucketName = "my-graffitit-s3-bucket",
                Key = files[i],
            };
            var response = client.GetObjectMetadata(request);
            string posAndRot = response.Metadata["x-amz-meta-AR-Position-Rotation"];
            if (posAndRot != null)
                filteredFiles.Add(files[i], posAndRot);
        }
        return filteredFiles;
    }
}
