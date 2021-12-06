using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class PhoneCamera : MonoBehaviour
{
    //Recorded content
    public static Texture2D temTexture;

    //Location
    private static string N_Latitude;
    private static string E_Longtitude;
    public static string locationString;
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

        StartCoroutine(startGPS());
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

        NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
        {
            Debug.Log("Image path: " + path);
            if(path != null)
            {
                
                Texture2D texture = NativeCamera.LoadImageAtPath(path, maxSize);
                S3Manager.contentTexCopy = texture;
                DateTime localDate = DateTime.Now;
                S3Manager.fileName = localDate.ToString().Replace('/','-') + ".png";
                S3Manager.filePath = path;
                S3Manager.contentType = "image/png";
                NativeGallery.SaveImageToGallery(texture, "test", "myimg.png");
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
                temTexture = texture;
                Destroy(quad, 5f);

                //Destroy(texture, 5f);
                SceneManager.LoadScene("UploadContentPage");
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
                S3Manager.contentTexCopy = NativeCamera.GetVideoThumbnail(path);
                DateTime localDate = DateTime.Now;
                S3Manager.fileName = localDate.ToString().Replace('/','-') + ".mp4";
                S3Manager.filePath = path;
                S3Manager.contentType = "video/mp4";
                //Handheld.PlayFullScreenMovie("file://" + path);
                SceneManager.LoadScene("UploadContentPage");
            }
        }, NativeCamera.Quality.Low);
        Debug.Log("Permission result: " + permission);
    }

    public static void loadProfilePage()
    {
        SceneManager.LoadScene("ProfilePage");
    }

    public static void loadMapDisplay()
    {
        StaticGoogleMap.latitude = ARTapToPlace.latitude;
        StaticGoogleMap.longitude = ARTapToPlace.longitude;
        SceneManager.LoadScene("MapScene");
    }
    
    static IEnumerator startGPS()
    {
        if(!Input.location.isEnabledByUser)
        {
            Debug.Log("The user did not enable location service");
            yield break;
        } else
        {
            Input.location.Start(10.0f, 10.0f);
        }
        int maxWait = 20;
        while(Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        if(maxWait<1)
        {
            Debug.Log("Time exceeds limit");
            yield break;
        }
        if(Input.location.status==LocationServiceStatus.Failed)
        {
            Debug.Log("Detect location failed");
            yield break;
        } else
        {
            N_Latitude = Input.location.lastData.latitude.ToString();
            E_Longtitude = Input.location.lastData.longitude.ToString();
            locationString = N_Latitude + "," + E_Longtitude;
            Input.location.Stop();
            yield return null;
        }
    }

    public static void loadARScene()
    {
        ARTapToPlace.hideCanvas = true;
        SceneManager.LoadScene("TemHomePage");
    }

    //Helper function to make Texture2D Readable through script
    /*https://stackoverflow.com/questions/44733841/how-to-make-texture2d-readable-via-script
    */
    public static Texture2D duplicateTexture(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }
}

