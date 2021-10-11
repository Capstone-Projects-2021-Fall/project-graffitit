using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LoginMenuScript : MonoBehaviour
{
    public void backToRegisteration()
    {
        SceneManager.LoadScene("RegisterationPage");
    }
}
