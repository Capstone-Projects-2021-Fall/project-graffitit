using System.Collections;
using System.Collections.Generic;
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
    private float zoom = 15;
    string url = "";
    private IEnumerator mapCoroutine;

    // Start is called before the first frame update
    [System.Obsolete]
    void Start()
    {
        mapDisplay = GameObject.Find("MapDisplay").GetComponent<RawImage>();
        backButton = GameObject.Find("BackButton").GetComponent<Button>();
        backButton.onClick.AddListener(getBackHome);
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
        Debug.Log("La and lo: " + latitude + " | " + longitude);
        url = "https://maps.googleapis.com/maps/api/staticmap?" + "size=" + width + "x" + height + "&center=" + latitude + "," + longitude
                + "&zoom=" + zoom + "&markers=color:blue%7Clabel:S%7C" + latitude + "," + longitude + "&maptype" + "roadmap" 
                + "&key=" + APIKey;

        WWW www = new WWW(url);
        yield return www;
        mapDisplay.texture = www.texture;
    }

    public static void getBackHome()
    {
        SceneManager.LoadScene("TemHomePage");
    }
}
