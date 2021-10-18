using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class RecordAction : MonoBehaviour
{
	Action imageAction = () => {
		PhoneCamera.TakePicture(512);
	};

	Action videoAction = () =>
	{
		PhoneCamera.RecordVideo();
	};
	// Start is called before the first frame update
	public void invokePopup()
    {
		Popup popup = UIController.Instance.CreatePopup();
		//Init popup with params (canvas, text, text, text, action)
		popup.Init(UIController.Instance.MainCanvas,
			"Do you want to take a picture or record a video?",
			"Image",
			"Video",
			imageAction,
			videoAction
			);
	}

}
