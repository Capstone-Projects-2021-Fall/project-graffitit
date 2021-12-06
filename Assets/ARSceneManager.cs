using Amazon;
using Amazon.CognitoIdentity;
using Amazon.S3;
using Amazon.S3.Model;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class ARSceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    private byte[] contentBodyBytes = null;
    private ARTapToPlace ar;
    public AmazonS3Client client;
    private string[] keys;
    private List<Texture> textures;
    private bool texturesLoaded = false;
    public GameObject prefab;
    public Texture testText;
    public Dictionary<string, GameObject> gameobjects;
    void Start()
    {
        ar = this.GetComponent<ARTapToPlace>();
        CognitoAWSCredentials credentials = new CognitoAWSCredentials(
        "us-east-2:0b2ab95e-b5ce-4a6e-ba47-9d2ef2a72b7e", // Identity pool ID
        RegionEndpoint.USEast2 // Region
        );
        textures = new List<Texture>();

        client = new AmazonS3Client(credentials, RegionEndpoint.USEast2);
        keys = getListFilesInBucket(client);
        gameobjects = new Dictionary<string, GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if(texturesLoaded)
            loadTextures();
    }

    public async Task loadImages()
    {
        foreach (string key in keys)
        {
            await ReadObjectDataAsync(client, "my-graffitit-s3-bucket", key);
            Texture2D tex = new Texture2D(512, 512);
            tex.LoadImage(contentBodyBytes);
            textures.Add(tex);

        }
        texturesLoaded = true;
    }

    void loadTextures()
    {
        foreach (Texture text in textures)
        {
            GameObject obj = Instantiate(prefab, gameObject.transform);
            obj.transform.localPosition += Random.onUnitSphere;
            GameObject contentPlane = obj.GetComponentInChildren<ContentPlane>().gameObject;
            Material mat = new Material(Shader.Find("Unlit/PlaneTexture"));
            mat.SetTexture("_MainTex", text);
            contentPlane.GetComponent<Renderer>().material = mat;
            texturesLoaded = false;
        }
    }

    public async Task ReadObjectDataAsync(IAmazonS3 client, string bucketname, string keyName)
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
}
