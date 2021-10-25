using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Threading.Tasks;

public class PhoneCamera : MonoBehaviour
{
    //S3
    private static IAmazonS3 _s3Client;

    private const string BUCKET_NAME = "my-graffitit-s3-bucket";


    //Camera 
    private bool camAvailable;
    private WebCamTexture backCam;
    private Texture defaultBackground;

    public RawImage background;
    public AspectRatioFitter fit;
    // Start is called before the first frame update
    void Start()
    {
        //Camera
        defaultBackground = background.texture;
        WebCamDevice[] devices = WebCamTexture.devices;

        if(devices.Length ==0)
        {
            Debug.Log("No camera detected");
            camAvailable = false;
            return;
        }

        for(int i = 0; i < devices.Length; i++)
        {
            if(!devices[i].isFrontFacing)
            {
                backCam = new WebCamTexture(devices[i].name, Screen.width, Screen.height, 60);
            }
        }

        if(backCam == null)
        {
            Debug.Log("Unable to find back camera");
            return;
        }

        backCam.Play();
        background.texture = backCam;

        camAvailable = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (!camAvailable)
            return;

        float ratio = (float)backCam.width / (float)backCam.height;
        fit.aspectRatio = ratio;

        float scaleY = backCam.videoVerticallyMirrored ? -1f : 1f;
        background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

        int orient = -backCam.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
    }

    public static void TakePicture(int maxSize)
    {
        NativeCamera.Permission permission = NativeCamera.TakePicture(async (path) =>
        {
            Debug.Log("Image path: " + path);
            if(path != null)
            {
                Texture2D texture = NativeCamera.LoadImageAtPath(path, maxSize);
                NativeGallery.SaveImageToGallery(texture, "test", "myimg.png");
                _s3Client = new AmazonS3Client();
                await UploadObjectFromFileAsync(_s3Client, BUCKET_NAME, "myimg.png", path);
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
                    material.shader = Shader.Find("Legacy Shaders/Diffuse");

                material.mainTexture = texture;

                Destroy(quad, 5f);

                Destroy(texture, 5f);
            }
        }, maxSize);
    }
    public static void RecordVideo()
    {
        NativeCamera.Permission permission = NativeCamera.RecordVideo((path) =>
        {
            Debug.Log("Video path: " + path);
            if (path != null)
            {
                NativeGallery.SaveVideoToGallery(path, "testvideo", "myvideo.mp4");
                //Play the recorded video
                Handheld.PlayFullScreenMovie("file://" + path);
            }
        });
        Debug.Log("Permission result: " + permission);
    }

    public static void loadProfilePage()
    {
        SceneManager.LoadScene("ProfilePage");
    }

    //--------------S3 Functions---------------------
    private static async Task UploadObjectFromFileAsync(
            IAmazonS3 client,
            string bucketName,
            string objectName,
            string filePath)
    {
        try
        {
            var putRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = objectName,
                FilePath = filePath,
                ContentType = "text/plain"
            };
            Console.WriteLine("It gets to the try block");
            putRequest.Metadata.Add("x-amz-meta-title", "someTitle");

            PutObjectResponse response = await client.PutObjectAsync(putRequest);
        }
        catch (AmazonS3Exception e)
        {
            Console.WriteLine("Something bad happen");
            Console.WriteLine($"Error: {e.Message}");
        }
    }
    private static async Task UploadObjectFromContentAsync(IAmazonS3 client,
            string bucketName,
            string objectName,
            string content)
    {
        var putRequest = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = objectName,
            ContentBody = content
        };

        PutObjectResponse response = await client.PutObjectAsync(putRequest);
    }
}

