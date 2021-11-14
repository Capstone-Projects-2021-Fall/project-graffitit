using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnlockProfile : MonoBehaviour
{
    public InputField profileBio;
    public Button editProfileButton, saveProfileButton, profilePicButton;

    // Start is called before the first frame update
    void Start()
    {
        //TODO:
        //	Pull User Data from SQL Database
        //	Set Username text
        //	Set Profile picture
        //	Set Bio text

        editProfileButton.onClick.AddListener(activateEditing);
        saveProfileButton.onClick.AddListener(saveProfile);
    }

    void activateEditing()
    {
        editProfileButton.gameObject.SetActive(false);
        profileBio.interactable = true;
        profilePicButton.gameObject.SetActive(true);
        saveProfileButton.gameObject.SetActive(true);
    }
    void saveProfile()
    {
        saveProfileButton.gameObject.SetActive(false);
        profileBio.interactable = false;
        profilePicButton.gameObject.SetActive(false);
        editProfileButton.gameObject.SetActive(true);

        //ADD PUSHING UPDATES TO SQL DATABASE
    }
}