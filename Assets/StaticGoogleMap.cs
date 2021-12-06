using Amazon;
using Amazon.CognitoIdentity;
using Amazon.S3;
using Amazon.S3.Model;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StaticGoogleMap : MonoBehaviour
{
    private RawImage mapDisplay;
    private Button backButton;
    private string APIKey = "AIzaSyDU7gepzeR9vqWBWgTsP7ceQk8j1i8Ctn8";
    public static double latitude;
    public static double longitude;
    private float width = 640;
    private float height = 640;
    private float zoom = 13.5F;
    string url = "";
    string appendMarkerStr = "";
    private IEnumerator mapCoroutine;

    // Start is called before the first frame update
    [System.Obsolete]
    void Start()
    {
        CognitoAWSCredentials credentials = new CognitoAWSCredentials(
        "us-east-2:0b2ab95e-b5ce-4a6e-ba47-9d2ef2a72b7e", // Identity pool ID
        RegionEndpoint.USEast2 // Region
        );

        AmazonS3Client client = new AmazonS3Client(credentials, RegionEndpoint.USEast2);
        mapDisplay = GameObject.Find("MapDisplay").GetComponent<RawImage>();
        backButton = GameObject.Find("BackButton").GetComponent<Button>();
        backButton.onClick.AddListener(getBackHome);
        string[] files = getListFilesInBucket(client);
        Dictionary<string, string> filesAndLocation = getLocationStringsOnServer(files, client);
        foreach(KeyValuePair<string, string> kvp in filesAndLocation)
        {
            appendMarkerStr += "&markers=color:yellow%7Clabel:S%7C" + kvp.Value;
        }
        mapCoroutine = GetGoogleMap(latitude, longitude);
        StartCoroutine(mapCoroutine);
    }

    // Update is called once per frame
    void Update()
    {
    }

    [System.Obsolete]
    IEnumerator GetGoogleMap(double latitude, double longitude)
    {
        Debug.Log(appendMarkerStr);
        url = "https://maps.googleapis.com/maps/api/staticmap?" + "size=" + width + "x" + height + "&center=" + latitude + "," + longitude//"39.95561" + "," + "-75.15779"
                + "&zoom=" + zoom + "&markers=color:blue%7Clabel:S%7C" + latitude + "," + longitude + appendMarkerStr + "&key=" + APIKey;

        WWW www = new WWW(url);
        yield return www;
        mapDisplay.texture = www.texture;
    }

    public static void getBackHome()
    {
        SceneManager.LoadScene("TemHomePage");
    }

    public static string[] getListFilesInBucket(AmazonS3Client client)
    {
        List<string> filesList = new List<string>();
        var listResponse = client.ListObjects("my-graffitit-s3-bucket");
        foreach(S3Object obj in listResponse.S3Objects)
        {
            filesList.Add(obj.Key);
        }
        return filesList.ToArray();
    }

    public static Dictionary<string, string> getLocationStringsOnServer(string[] files, AmazonS3Client client)
    {
        Dictionary<string, string> filesAndLocation = new Dictionary<string, string>();
        
        for(int i = 0; i<files.Length; i++)
        {
            var request = new GetObjectMetadataRequest()
            {
                BucketName = "my-graffitit-s3-bucket",
                Key = files[i],
            };
            var response = client.GetObjectMetadata(request);
            string location = response.Metadata["x-amz-meta-location-info"];
            if(location!=null)
                filesAndLocation.Add(files[i], location);
        }
        return filesAndLocation;
    }

    public static async Task GetImageTextureAsync(IAmazonS3 client, string bucketname, string keyName, byte[] contentBodyBytes)
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

                contentBodyBytes = memstream.ToArray();
            }
        }
        catch (AmazonS3Exception e)
        {
            Debug.Log($"Error: '{e.Message}'");
        }
    }
}
