using UnityEngine;
using UnityEngine.SceneManagement;
using Amazon.DynamoDBv2;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2.DataModel;
using Amazon;
using Amazon.DynamoDBv2.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.UI;

public class RegisterationPanel : MonoBehaviour
{
    private static TMPro.TMP_InputField email;
    private static TMPro.TMP_InputField password;
    private Button registerButton;

    //Fields for DynamoDB
    public static CognitoAWSCredentials credentials = new CognitoAWSCredentials(
        "us-east-2:455127a3-0e56-4e32-9fb3-82f7e9ac6e93", // Identity pool ID
        RegionEndpoint.USEast2 // Region
    );
    public static AmazonDynamoDBClient client = new AmazonDynamoDBClient(credentials);
    public static DynamoDBContext Context = new DynamoDBContext(client);

    private void Start()
    {
        email = GameObject.Find("Email").GetComponent<TMPro.TMP_InputField>();
        password = GameObject.Find("Password").GetComponent<TMPro.TMP_InputField>();
        registerButton = GameObject.Find("RegisterButton").GetComponent<Button>();
        registerButton.onClick.AddListener(clickToSend);
        ListTablesResponse result = client.ListTables();
        List<string> tables = result.TableNames;
        Debug.Log(string.Format("Retrieved tables {0}", tables[0]));
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

    public static async Task SingleTableBatchWrite()
    {
        GraffitITUsers currentUser = new GraffitITUsers
        {
            UserID = 0,
            UserEmail = email.text,
            UserPassword = password.text
        };

        var userBatch = Context.CreateBatchWrite<GraffitITUsers>();
        userBatch.AddPutItem(currentUser);
        await userBatch.ExecuteAsync();
    }

    public void clickToSend()
    {
        _ = SingleTableBatchWrite();
    }
}
