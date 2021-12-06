using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UnlockProfile : MonoBehaviour
{
    public static Text userName;
    public static InputField profileBio;
    public Button editProfileButton;
    public Button profileButton;
    public static Image profileImage;
    private static string proImgPath;
    private static byte[] contentBodyBytes=null;
    private static IAmazonS3 S3client;
    private static AmazonDynamoDBClient DBclient;
    private static DynamoDBContext DBcontext;
    // Start is called before the first frame update
    async Task Start()
    {
        //TODO:
        //	Pull User Data from SQL Database
        //	Set Username text
        //	Set Profile picture
        //	Set Bio text

        //S3
        CognitoAWSCredentials S3credentials = new CognitoAWSCredentials(
        "us-east-2:0b2ab95e-b5ce-4a6e-ba47-9d2ef2a72b7e", // Identity pool ID
        RegionEndpoint.USEast2 // Region
        );
        S3client = new AmazonS3Client(S3credentials, RegionEndpoint.USEast2);

        //DB
        CognitoAWSCredentials DBcredentials = new CognitoAWSCredentials(
            "us-east-2:455127a3-0e56-4e32-9fb3-82f7e9ac6e93", // Identity pool ID
            RegionEndpoint.USEast2 // Region
        );
        DBclient = new AmazonDynamoDBClient(DBcredentials, RegionEndpoint.USEast2);
        DBcontext = new DynamoDBContext(DBclient);

        profileImage = GameObject.Find("ProfilePicture").GetComponent<Image>();
        editProfileButton.onClick.AddListener(updateAll);
        profileButton.onClick.AddListener(PickImage);
        userName = GameObject.Find("ProfileUsername").GetComponent<Text>();
        profileBio = GameObject.Find("ProfileBio").GetComponent<InputField>();
        userName.text = LoginMenuScript.curUserName;
        profileBio.text = LoginMenuScript.curUserBio;

        await ReadObjectDataAsync(S3client, "my-graffitit-s3-bucket", LoginMenuScript.curUserName + "profile.png");
        Texture2D tex = new Texture2D(512, 512);
        tex.LoadImage(contentBodyBytes);
        profileImage.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        Debug.Log("tex null?: " + tex is null);
        Debug.Log("tex readable?: " + tex.isReadable);
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void PickImage()
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
       {
           Debug.Log("Image path:" + path);
           if (path != null)
           {
               Texture2D texture = NativeGallery.LoadImageAtPath(path, 512);
               if (texture == null)
               {
                   Debug.Log("Couldn't load texture from " + path);
                   return;
               }

               GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
               quad.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
               quad.transform.forward = Camera.main.transform.forward;
               quad.transform.localScale = new Vector3(1f, texture.height / (float)texture.width, 1f);

               Material material = quad.GetComponent<Renderer>().material;
               if (!material.shader.isSupported)
                   material.shader = Shader.Find("legacy Shader/Diffuse");

               material.mainTexture = texture;

               Destroy(quad, 5f);

               //profileImage.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
               //Destroy(texture, 5f);

               proImgPath = path;
           }
       });
    }

    public static async void uploadFileToS3()
    {
        try
        {
            var putRequest = new PutObjectRequest
            {
                BucketName = "my-graffitit-s3-bucket",
                Key = userName.text + "profile.png",
                FilePath = proImgPath,
                ContentType = "image/png"
            };

            PutObjectResponse response = await S3client.PutObjectAsync(putRequest);
        }
        catch (AmazonS3Exception e)
        {
            Debug.Log($"Error: {e.Message}");
        }
    }

    public static async Task updateUserBioAsync()
    {
        Debug.Log("Current User email: " + LoginMenuScript.curUserEmail);
        GraffitITUsers retrievedUser = await DBcontext.LoadAsync<GraffitITUsers>(LoginMenuScript.curUserEmail);
        Debug.Log("Current bio in the db: " + retrievedUser.UserBio);
        retrievedUser.UserBio = profileBio.text;
        Debug.Log("Bio input box: " + profileBio.text);
        await DBcontext.SaveAsync(retrievedUser);
    }

    public static void updateAll()
    {
        uploadFileToS3();
        _ = updateUserBioAsync();
    }

    public static async Task ReadObjectDataAsync(IAmazonS3 client, string bucketname, string keyName)
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