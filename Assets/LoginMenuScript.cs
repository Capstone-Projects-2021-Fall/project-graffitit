using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Collections.Generic;
using UnityEngine;
public class LoginMenuScript : MonoBehaviour
{


    private TMPro.TMP_InputField email;
    private TMPro.TMP_InputField password;

    public static CognitoAWSCredentials credentials = new CognitoAWSCredentials(
    "us-east-2:455127a3-0e56-4e32-9fb3-82f7e9ac6e93", // Identity pool ID
    RegionEndpoint.USEast2 // Region
    );
    public static AmazonDynamoDBClient client = new AmazonDynamoDBClient(credentials);


    void Start()
    {
        email = GameObject.Find("Email").GetComponent<TMPro.TMP_InputField>();
        password = GameObject.Find("Password").GetComponent<TMPro.TMP_InputField>();


        RetrieveItem(client);


    }


    public static async void RetrieveItem(AmazonDynamoDBClient client)
    {
        var request = new GetItemRequest
        {
            TableName = "GraffitITUsers",
            Key = new Dictionary<string, AttributeValue>()
            {
                {"UserID", new AttributeValue
                    {
                       N = "0"
                    }
                },
                {"UserEmail", new AttributeValue
                    { 
                       S = "test@gmail.com"
                    }
                }
            },
            ProjectionExpression = "UserID, UserEmail, UserPassword",
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

