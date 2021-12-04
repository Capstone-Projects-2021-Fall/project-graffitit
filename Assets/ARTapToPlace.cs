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

    private IAmazonS3 client;

    // Start is called before the first frame update
    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        raycastManager = FindObjectOfType<ARRaycastManager>();

        CognitoAWSCredentials credentials = new CognitoAWSCredentials(
        "us-east-2:0b2ab95e-b5ce-4a6e-ba47-9d2ef2a72b7e", // Identity pool ID
        RegionEndpoint.USEast2 // Region
        );

        client = new AmazonS3Client(credentials, RegionEndpoint.USEast2);

        updateS3ObjectMeta();

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
    //
    public void updateS3ObjectMeta()
    {
        //use CopyObjectRequest
        //use .Metadata.Add
        //put in the pose.position
        //put in the pose.rotation
        //issue the request with client.copyObject(request)

        CopyObjectRequest copyRequest = new CopyObjectRequest
        {
            SourceBucket = "my-graffitit-s3-bucket",
            SourceKey = "1280px-Philadelphia_City_Hall_at_night.jpg",
            DestinationBucket = "my-graffitit-s3-bucket",
            DestinationKey = "1280px-Philadelphia_City_Hall_at_night.jpg",
            MetadataDirective = S3MetadataDirective.REPLACE,
            CannedACL = S3CannedACL.PublicRead
        };
        copyRequest.Metadata.Add("AR-Position", placementPose.position.ToString());
        copyRequest.Metadata.Add("AR-Rotation", placementPose.rotation.ToString());

        client.CopyObject(copyRequest);


    }
}
