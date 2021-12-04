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
    private static TMPro.TMP_InputField userName;
    private Button registerButton;

    public static AmazonDynamoDBClient client;
    public static DynamoDBContext context;


    private void Start()
    {
        email = GameObject.Find("Email").GetComponent<TMPro.TMP_InputField>();
        password = GameObject.Find("Password").GetComponent<TMPro.TMP_InputField>();
        userName = GameObject.Find("UserName").GetComponent<TMPro.TMP_InputField>();
        registerButton = GameObject.Find("RegisterButton").GetComponent<Button>();
        CognitoAWSCredentials credentials = new CognitoAWSCredentials(
            "us-east-2:455127a3-0e56-4e32-9fb3-82f7e9ac6e93", // identity pool id
            RegionEndpoint.USEast2 // region
        );
        client = new AmazonDynamoDBClient(credentials, RegionEndpoint.USEast2);
        context = new DynamoDBContext(client);
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

    public static async Task SingleTableBatchWrite(DynamoDBContext Context)
    {
        GraffitITUsers currentUser = new GraffitITUsers
        {
            UserEmail = email.text,
            UserPassword = password.text,
            UserName = userName.text,
            UserBio = ""
        };

        var userBatch = Context.CreateBatchWrite<GraffitITUsers>();
        userBatch.AddPutItem(currentUser);
        await userBatch.ExecuteAsync();
    }

    public void clickToSend()
    {
        _ = SingleTableBatchWrite(context);
    }

    public int getItemCountInTable()
    {
        return (int)client.DescribeTable("GraffitITUsers").Table.ItemCount;
    }
    
}
