using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class opening : MonoBehaviour {
    private float endOpeningTime = 4;
    private float startOpeningTime = 0;

    // Use this for initialization
    void Start () {
        
        
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.time > startOpeningTime + endOpeningTime)
        {
            startOpeningTime = Time.time;
            if (PlayerPrefs.HasKey("userID"))
            {
                SceneManager.LoadScene("Map");
            }
            else
            {
                SceneManager.LoadScene("GlobalAccountSystem");
            }
        }
    }
}
