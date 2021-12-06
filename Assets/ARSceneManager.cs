using Amazon;
using Amazon.CognitoIdentity;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ARSceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    public ARTapToPlace ar;
    public AmazonS3Client client;
    private string[] keys;
    private Dictionary<string,Texture> textures;
    private bool texturesLoaded = false;
    public GameObject prefab;
    public Dictionary<string, GameObject> gameobjects;
    private Dictionary<string, byte[]> contentBodyBytes = null;
    public int limit = 5;
    private Dictionary<string, string> locations;
    private Dictionary<string, string> descriptions;
    private Dictionary<string, string> positions;
    public Vector2 loc;
    void Start()
    {
        CognitoAWSCredentials credentials = new CognitoAWSCredentials(
        "us-east-2:0b2ab95e-b5ce-4a6e-ba47-9d2ef2a72b7e", // Identity pool ID
        RegionEndpoint.USEast2 // Region
        );
        textures = new Dictionary<string, Texture>();
        contentBodyBytes = new Dictionary<string, byte[]>();

        client = new AmazonS3Client(credentials, RegionEndpoint.USEast2);
        keys = getListFilesInBucket(client);
        gameobjects = new Dictionary<string, GameObject>();
        InvokeRepeating("loadImages", 0f, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (texturesLoaded)
        {
            
            
            positions = getARPositionAndRotation(keys, client);
            
            loadTextures();
        }
            
    }

    void loadLocations()
    {
        foreach (KeyValuePair<string, string> location in locations)
        {
            string v = location.Value;
            Debug.Log(v + " " + location.Key);
        



        }

        foreach (KeyValuePair<string, string> pos in positions)
        {
            string v = pos.Value;
            Debug.Log(v + " " + pos.Key);
        }
    }
    public async Task loadImages()
    {
        foreach (KeyValuePair<string, GameObject> obj in gameobjects)
        {
            //Destroy(obj.Value);
        }
        gameobjects.Clear();
        contentBodyBytes.Clear();
        textures.Clear();
        locations = getLocationStringsOnServer(keys, client);
        descriptions = getARDescriptions(keys, client);
        int lim = 0;
        foreach (string key in keys)
        {
            Debug.Log(key);
            if (isNearByObject(key)){
                lim += 1;
                await ReadObjectDataAsync(client, "my-graffitit-s3-bucket", key);
                Texture2D tex = new Texture2D(512, 512);
                tex.LoadImage(contentBodyBytes[key]);
                if (!textures.ContainsKey(key))
                    textures.Add(key, tex);

                if (lim >= limit)
                    break;
            }
        }
        texturesLoaded = true;
    }

    bool isNearByObject(string key)
    {
        try
        {
            Vector2 gps = StringToVector2(locations[key]);
            Debug.Log(gps);
            Vector2 currentGPS = loc;//new Vector2(ARTapToPlace.latitude, ARTapToPlace.longitude);
            Debug.Log(currentGPS);
            float dist = Vector2.Distance(gps, currentGPS);
            Debug.Log(dist);
            if (dist < 2)
                return true;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return false;
        }
        return false;
    }
    void loadTextures()
    {
        
        foreach (KeyValuePair<string, Texture> tex in textures)
        {
            Texture text = tex.Value;
            GameObject obj = Instantiate(prefab, gameObject.transform);
            obj.GetComponentInChildren<TextMeshPro>().text = descriptions[tex.Key];
            obj.transform.localPosition += StringToVector3(positions[tex.Key]);
            GameObject contentPlane = obj.GetComponentInChildren<ContentPlane>().gameObject;
            Material mat = new Material(Shader.Find("Unlit/PlaneTexture"));
            mat.SetTexture("_MainTex", text);
            contentPlane.GetComponent<Renderer>().material = mat;
            texturesLoaded = false;
            gameobjects.Add(tex.Key, obj);

        }
    }

    public async Task ReadObjectDataAsync(IAmazonS3 client, string bucketname, string keyName)
    {
        if (!contentBodyBytes.ContainsKey(keyName))
        {
            try
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = bucketname,
                    Key = keyName
                };

                using (GetObjectResponse response = await client.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                using (StreamReader reader = new StreamReader(responseStream))
                using (var memstream = new MemoryStream())
                {
                    var buffer = new byte[512];
                    var bytesRead = default(int);
                    while ((bytesRead = reader.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
                        memstream.Write(buffer, 0, bytesRead);

                    contentBodyBytes.Add(keyName, memstream.ToArray());
                }
            }
            catch (AmazonS3Exception e)
            {
                Debug.Log($"Error: '{e.Message}'");
            }
        }
    }

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

    //Key, GPS.
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

    public Dictionary<string, string> getARPositionAndRotation(string[] files, AmazonS3Client client)
    {
        Dictionary<string, string> filteredFiles = new Dictionary<string, string>();
        for (int i = 0; i < files.Length; i++)
        {
            var request = new GetObjectMetadataRequest()
            {
                BucketName = "my-graffitit-s3-bucket",
                Key = files[i],
            };
            var response = client.GetObjectMetadata(request);
            string posAndRot = response.Metadata["x-amz-meta-ar-position"];
            if (posAndRot != null)
                filteredFiles.Add(files[i], posAndRot);
        }
        return filteredFiles;
    }

    public Dictionary<string, string> getARDescriptions(string[] files, AmazonS3Client client)
    {
        Dictionary<string, string> filteredFiles = new Dictionary<string, string>();
        for (int i = 0; i < files.Length; i++)
        {
            var request = new GetObjectMetadataRequest()
            {
                BucketName = "my-graffitit-s3-bucket",
                Key = files[i],
            };
            var response = client.GetObjectMetadata(request);
            string posAndRot = response.Metadata["x-amz-meta-post-description"];
            if (posAndRot != null)
                filteredFiles.Add(files[i], posAndRot);
        }
        return filteredFiles;
    }

    public static Vector3 StringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }

    public static Vector3 StringToVector2(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector2
        Vector2 result = new Vector2(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]));

        return result;
    }

}
