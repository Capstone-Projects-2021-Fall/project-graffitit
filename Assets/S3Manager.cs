using Amazon;
using Amazon.CognitoIdentity;
using Amazon.S3;
using Amazon.S3.Model;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Amazon.S3.Transfer;

public class S3Manager : MonoBehaviour
{
    //Store GPS and create buckets based on GPS
    //Retreive contents based off GPS
    public Image previewImg;
    public static Texture2D contentTexCopy;
    public InputField description;
    public Button uploadButton;
    public static string S3BucketName;
    public static string fileName;
    public static string filePath;
    public static string contentType;
    public static IAmazonS3 client;
    public static string descriptionText;
    void Start()
    {
        uploadButton.onClick.AddListener(uploadFileToS3);

        CognitoAWSCredentials credentials = new CognitoAWSCredentials(
        "us-east-2:0b2ab95e-b5ce-4a6e-ba47-9d2ef2a72b7e", // Identity pool ID
        RegionEndpoint.USEast2 // Region
        );

        client = new AmazonS3Client(credentials, RegionEndpoint.USEast2);

        S3BucketName = "my-graffitit-s3-bucket";


        //_ = ListingObjectsAsync(client, S3BucketName);

        //_ = ReadObjectDataAsync(client, S3BucketName, "1280px-Philadelphia_City_Hall_at_night.jpg");

        previewImg.sprite = Sprite.Create(contentTexCopy, new Rect(0.0f, 0.0f, contentTexCopy.width, contentTexCopy.height), new Vector2(0.5f, 0.5f));
        Debug.Log(contentTexCopy.Equals(null));

    }


    public async void uploadFileToS3()
    {
        try{
            var putRequest = new PutObjectRequest
            {
                BucketName = S3BucketName,
                Key = fileName,
                FilePath = filePath,
                ContentType = contentType
            };

            Debug.Log(PhoneCamera.locationString);
            putRequest.Metadata.Add("Location-Info", PhoneCamera.locationString);
            descriptionText = description.text;
            putRequest.Metadata.Add("Post-Description", description.text);
            PutObjectResponse response = await client.PutObjectAsync(putRequest);
        }
        catch (AmazonS3Exception e)
        {
            Debug.Log($"Error: {e.Message}");
        }
        ARTapToPlace.placeOBJ = true;
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

    

    
}
