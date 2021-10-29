using UnityEngine;
using UnityEngine.SceneManagement;
using MySql.Data.MySqlClient;
using UnityEngine.UI;

public class RegisterationPanel : MonoBehaviour
{
    private TMPro.TMP_InputField email;

    static string connStr = "server=graffitit-database-instance-1.cakajll39l1z.us-east-2.rds.amazonaws.com; database=graffitit-database; Username=admin; Password=GraffitIT12";
    private void Start()
    {
        email = GameObject.Find("Email").GetComponent<TMPro.TMP_InputField>();
        
        try
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
        } catch(MySqlException ex)
        {
            Debug.Log(ex.Message);
        }
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
