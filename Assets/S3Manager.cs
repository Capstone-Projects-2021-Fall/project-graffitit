using Amazon;
using Amazon.CognitoIdentity;
using Amazon.S3;
using UnityEngine;
using UnityEngine.UI;

public class S3Manager : MonoBehaviour
{
    CognitoAWSCredentials credentials;

    public string S3BucketName = null;
    public string SampleFileName = null;
    public Text ResultText = null;
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

    }

}
