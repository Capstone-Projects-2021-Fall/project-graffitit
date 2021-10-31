using Amazon;
using Amazon.CognitoIdentity;
using Amazon.S3;
using Amazon.S3.Model;
using System.IO;
using UnityEngine;

public class S3Manager : MonoBehaviour
{
    CognitoAWSCredentials credentials;

    private string S3BucketName;
    private string fileName;
    private string filePath;
    AmazonS3Client client;

    void Awake()
    {
        UnityInitializer.AttachToGameObject(this.gameObject);
        AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;

        credentials = new CognitoAWSCredentials(
        "us-east-2:0b2ab95e-b5ce-4a6e-ba47-9d2ef2a72b7e", // Identity pool ID
        RegionEndpoint.USEast2 // Region
        );

        client = new AmazonS3Client(credentials);

        S3BucketName = "my-graffitit-s3-bucket";

    }

    public void uploadFileToS3()
    {
        var stream = new FileStream(this.filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        var request = new PostObjectRequest()
        {
            Bucket = this.S3BucketName,
            Key = this.fileName,
            InputStream = stream,
            CannedACL = S3CannedACL.Private,
            Region = RegionEndpoint.USEast2
        };

        this.client.PostObjectAsync(request, (responseobj) =>
        {
            if(responseobj.Exception == null)
            {
                Debug.Log(string.Format("\nobject {0} posted to bucket {1}", responseobj.Request.Key, responseobj.Request.Bucket));
            }
            else if(responseobj.Exception !=null) 
            {
                Debug.Log("An error received during posting");
            }
        });
    }

    public void setFileName(string name)
    {
        this.fileName = name;
    }

    public void setFilePath(string path)
    {
        this.filePath = path;
    }
}
