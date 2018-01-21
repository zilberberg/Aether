using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class guestCheck : MonoBehaviour {
    public Button thisButton; 
	// Use this for initialization
	void Start () {
        
        if (PlayerPrefs.GetString("Guest") == "yes")
        {
            thisButton.interactable = false;
        } else if (PlayerPrefs.GetString("Guest") == "no")
        {
            thisButton.interactable = true;
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
