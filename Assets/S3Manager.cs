using Amazon;
using Amazon.CognitoIdentity;
using Amazon.S3;
using Amazon.S3.Model;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class S3Manager : MonoBehaviour
{
    public Image previewImg;
    public InputField description;
    public Button uploadButton;
    private string S3BucketName;
    public static string fileName;
    public static string filePath;
    private IAmazonS3 client;
    private RegionEndpoint _S3Region;
    private void Start()
    {
        previewImg.sprite = Sprite.Create(PhoneCamera.temTexture, new Rect(0.0f, 0.0f, previewImg.flexibleWidth, previewImg.flexibleHeight), new Vector2(0.5f, 0.5f), 100.0f);
        uploadButton.onClick.AddListener(uploadFileToS3);

        CognitoAWSCredentials credentials = new CognitoAWSCredentials(
        "us-east-2:0b2ab95e-b5ce-4a6e-ba47-9d2ef2a72b7e", // Identity pool ID
        RegionEndpoint.USEast2 // Region
        );

        this.client = new AmazonS3Client(credentials, RegionEndpoint.USEast2);

        this.S3BucketName = "my-graffitit-s3-bucket";

        previewImg.sprite = Sprite.Create(PhoneCamera.temTexture, new Rect(0, 0, 500, 500), new Vector2());
        _S3Region = RegionEndpoint.GetBySystemName("us-east-2");


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

            PutObjectResponse response = await client.PutObjectAsync(putRequest);
        }
        catch (AmazonS3Exception e)
        {
            Debug.Log($"Error: {e.Message}");
        }

    }

}
