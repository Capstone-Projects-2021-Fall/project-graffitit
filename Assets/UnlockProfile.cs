using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnlockProfile : MonoBehaviour
{
    public static Text userName;
    public InputField profileBio;
    public Button editProfileButton;

    // Start is called before the first frame update
    void Start()
    {
        //TODO:
        //	Pull User Data from SQL Database
        //	Set Username text
        //	Set Profile picture
        //	Set Bio text

        editProfileButton.onClick.AddListener(activateEditing);
        userName = GameObject.Find("ProfileUsername").GetComponent<Text>();
        userName.text = LoginMenuScript.curUserName;
    }

    void activateEditing()
    {
        editProfileButton.gameObject.SetActive(false);
        profileBio.interactable = true;
    }
    void saveProfile()
    {
        profileBio.interactable = false;
        editProfileButton.gameObject.SetActive(true);

        //ADD PUSHING UPDATES TO SQL DATABASE
    }
}