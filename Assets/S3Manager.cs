using Amazon;
using Amazon.CognitoIdentity;
using Amazon.S3;
using Amazon.S3.Model;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class S3Manager : MonoBehaviour
{
    //Store GPS and create buckets based on GPS
    //Retreive contents based off GPS
    public Image previewImg;
    public static Texture2D contentTexCopy = null;
    public InputField description;
    public Button uploadButton;
    public static string S3BucketName;
    public static string fileName;
    public static string filePath;
    public static string locationString;
    public static IAmazonS3 client;
    private RegionEndpoint _S3Region;
    void Start()
    {
        uploadButton.onClick.AddListener(uploadFileToS3);

        CognitoAWSCredentials credentials = new CognitoAWSCredentials(
        "us-east-2:0b2ab95e-b5ce-4a6e-ba47-9d2ef2a72b7e", // Identity pool ID
        RegionEndpoint.USEast2 // Region
        );

        client = new AmazonS3Client(credentials, RegionEndpoint.USEast2);

        S3BucketName = "my-graffitit-s3-bucket";

        _S3Region = RegionEndpoint.GetBySystemName("us-east-2");

        //_ = ListingObjectsAsync(client, S3BucketName);

        //_ = ReadObjectDataAsync(client, S3BucketName, "1280px-Philadelphia_City_Hall_at_night.jpg");

        previewImg.sprite = Sprite.Create(contentTexCopy, new Rect(0.0f, 0.0f, contentTexCopy.width, contentTexCopy.height), new Vector2(0.5f, 0.5f));
        //Debug.Log(PhoneCamera.temTexture.Equals(null));
    }


    public async void uploadFileToS3()
    {
        try{
            var putRequest = new PutObjectRequest
            {
                BucketName = S3BucketName,
                Key = fileName,
                FilePath = filePath,
                ContentType = "image/png"
            };

            putRequest.Metadata.Add("Location-Info", locationString);

            PutObjectResponse response = await client.PutObjectAsync(putRequest);
        }
        catch (AmazonS3Exception e)
        {
            Debug.Log($"Error: {e.Message}");
        }
        SceneManager.LoadScene("TemHomePage");
    }

    public static async Task ListingObjectsAsync(IAmazonS3 client, string bucketname)
    {
        try
        {
            ListObjectsRequest request = new ListObjectsRequest
            {
                BucketName = bucketname,
                MaxKeys = 10
            };

            do
            {
                ListObjectsResponse response = await client.ListObjectsAsync(request);

                response.S3Objects
                    .ForEach(obj => Debug.Log($"{obj.Key,-35}{obj.LastModified.ToShortDateString(),10}{obj.Size,10}"));

                if (response.IsTruncated)
                {
                    request.Marker = response.NextMarker;
                } else
                {
                    request = null;
                }
            } while (request != null);
        }
        catch (AmazonS3Exception ex)
        {
            Debug.Log($"Error encountered on server. Messagre:'{ex.Message}' getting list of objects.");
        }
    }

    public static async Task ReadObjectDataAsync(IAmazonS3 client, string bucketname, string keyName)
    {
        string responseBody = string.Empty;

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
            {
                //Process responseBody to original files
                responseBody = reader.ReadToEnd();
                Debug.Log(responseBody.Length);
                string filePath = $"/data/user/0/com.DefaultCompany.GraffitIT/files/{keyName}";
            }
        }
        catch (AmazonS3Exception e)
        {
            Debug.Log($"Error: '{e.Message}'");
        }
    }

    
}
