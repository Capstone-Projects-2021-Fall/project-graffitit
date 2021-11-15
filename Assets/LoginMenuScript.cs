using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginMenuScript : MonoBehaviour
{
    private static TMPro.TMP_InputField email;
    private static TMPro.TMP_InputField password;
    private static Button loginButton;

    //DB
    private static AmazonDynamoDBClient client;
    private static DynamoDBContext context;

    public static string curUserEmail;
    public static string curUserName;
    public static string curUserPassword;
    public static string curUserBio;
    void Start()
    {
        email = GameObject.Find("Email").GetComponent<TMPro.TMP_InputField>();
        password = GameObject.Find("Password").GetComponent<TMPro.TMP_InputField>();
        loginButton = GameObject.Find("LoginButton").GetComponent<Button>();
        curUserEmail = "";
        curUserName = "";
        curUserPassword = "";
        curUserBio = "";
        CognitoAWSCredentials credentials = new CognitoAWSCredentials(
            "us-east-2:455127a3-0e56-4e32-9fb3-82f7e9ac6e93", // Identity pool ID
            RegionEndpoint.USEast2 // Region
        );
        client = new AmazonDynamoDBClient(credentials, RegionEndpoint.USEast2);
        context = new DynamoDBContext(client);
        loginButton.onClick.AddListener(login);
       
    }

    
    public static async Task getGraffitITUser(IDynamoDBContext context, string userEmail)
    {
        GraffitITUsers temUser = await context.LoadAsync<GraffitITUsers>(userEmail);
        curUserEmail = temUser.UserEmail;
        curUserName = temUser.UserName;
        curUserPassword = temUser.UserPassword;
        curUserBio = temUser.UserBio;
    }


    private static bool checkForValidCredentials(string inputEmail, string inputPass, string storedEmail, string storedPass)
    {
        if (storedEmail.Equals(inputEmail) && storedPass.Equals(inputPass))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private static void login()
    {
        _ = getGraffitITUser(context, email.text);

        bool check = checkForValidCredentials(email.text, password.text, curUserEmail, curUserPassword);
        if (check)
        {
            Debug.Log(curUserName);
            SceneManager.LoadScene("ProfilePage");
        }
    }



    //***************************************************************************************************
    public static async void RetrieveItem(AmazonDynamoDBClient client)
    {
        var request = new GetItemRequest
        {
            TableName = "GraffitITUsers",
            Key = new Dictionary<string, AttributeValue>()
            {
                {"UserEmail", new AttributeValue
                    {
                       S = "first@gmail.com"
                    }
                }
            },
            ProjectionExpression = "UserEmail, UserName, UserPassword",
            ConsistentRead = true
        };
        var response = await client.GetItemAsync(request);

        var attributeList = response.Item;
        Debug.Log("\nPrinting item after retreival.......");
        PrintItem(attributeList);
    }


    //Helper function to print out the information about the items after retrieving them from DynamoDB
    private static void PrintItem(Dictionary<string, AttributeValue> attributeList)
    {
        foreach (KeyValuePair<string, AttributeValue> kvp in attributeList)
        {
            string attributeName = kvp.Key;
            AttributeValue value = kvp.Value;

            Debug.Log(
                attributeName + " " +
                (value.S == null ? "" : "S=[" + value.S + "]") +
                (value.N == null ? "" : "N=[" + value.N + "]") +
                (value.SS == null ? "" : "SS=[" + string.Join(",", value.SS.ToArray()) + "]") +
                (value.NS == null ? "" : "NS=[" + string.Join(",", value.NS.ToArray()) + "]")
                );
        }
        Debug.Log("************************************************");
    }
}

