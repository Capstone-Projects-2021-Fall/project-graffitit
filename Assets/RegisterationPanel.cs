using UnityEngine;
using UnityEngine.SceneManagement;
using MySql.Data.MySqlClient;
using System;

public class RegisterationPanel : MonoBehaviour
{
    private TMPro.TMP_InputField email;
    private TMPro.TMP_InputField password;
    private int currentID = 0;

    static string connStr = "server=graffitit-database-instance-1.cakajll39l1z.us-east-2.rds.amazonaws.com; database=graffitit-database; Username=admin; Password=GraffitIT12";
    MySqlConnection conn;
    private void Start()
    {
        email = GameObject.Find("Email").GetComponent<TMPro.TMP_InputField>();
        password = GameObject.Find("Password").GetComponent<TMPro.TMP_InputField>();
        try
        {
            conn = new MySqlConnection(connStr);
            conn.Open();
        } catch(MySqlException ex)
        {
            Debug.Log(ex.Message);
        }

        currentID = getDBRowLength();
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


    public void sendToMySQL()
    {
        var cmd = new MySqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "INSERT INTO USERS (user_id, user_email, user_password) VALUES (" + currentID + ", " + email.text + ", " + password.text + ")";
        cmd.ExecuteNonQuery();
    }

    public int getDBRowLength()
    {
        MySqlCommand cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT COUNT(user_id) FROM USERS";
        MySqlDataReader reader = cmd.ExecuteReader();
        int currentid = Int32.Parse(reader.GetString(0));
        return currentid;
    }
}
