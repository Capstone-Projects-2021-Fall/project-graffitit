using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Net.NetworkInformation;
using Amazon.DynamoDBv2;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2.DataModel;
using Amazon;

public class RegisterationPanel : MonoBehaviour
{
    private TMPro.TMP_InputField email;
    private TMPro.TMP_InputField password;

    //Fields for DynamoDB
    public static CognitoAWSCredentials credentials = new CognitoAWSCredentials(
    "us-east-2:772d1d1d-22e9-43cd-b015-ed51a12068ab", // Identity pool ID
    RegionEndpoint.USEast2 // Region
    );
    public static AmazonDynamoDBClient client = new AmazonDynamoDBClient(credentials);
    DynamoDBContext Context = new DynamoDBContext(client);




    private void Start()
    {
        email = GameObject.Find("Email").GetComponent<TMPro.TMP_InputField>();
        password = GameObject.Find("Password").GetComponent<TMPro.TMP_InputField>();

        Debug.Log(client.ListTables().ToString());
    }

    private void Update()
    {

    }
    public void loadLoginPage()
    {
        SceneManager.LoadScene("LoginMenuPage");
    }

    public void loadHomePage()
    {
        SceneManager.LoadScene("TemHomePage");
    }



}
