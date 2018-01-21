using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
//using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class mainMusic : MonoBehaviour {
    AudioSource audio;
    bool isMute=false;
    Scene scene;
    bool mapFlag = true;
    [SerializeField]
    AudioSource beep;
    [SerializeField]
    AudioSource champ;
    [SerializeField]
    AudioSource iAm;

    [SerializeField]
    AudioSource exploration;
    private bool tRBool = true;
    private bool gASBool = true;

    // Use this for initialization
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Start () {
        DontDestroyOnLoad(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
        scene = SceneManager.GetActiveScene();
        
        
        
        if(scene.name=="Map" && mapFlag)
        {
            iAm.Stop();
            champ.Stop();
            exploration.Play();
            mapFlag = false;
            gASBool = true;
            tRBool = true;
        }

        if(scene.name== "Test Room 1" && tRBool)
        {
            iAm.Stop();
            exploration.Stop();
            champ.Play();
            tRBool = false;
            mapFlag = true;
            gASBool = true;
        }
        if (scene.name == "GlobalAccountSystem" && gASBool)
        {
            
            exploration.Stop();
            champ.Stop();
            iAm.Play();
            gASBool = false;
            mapFlag = true;
            tRBool = true;
        }

    }

     public void musicMap()
    {
        audio.Stop();
        exploration.Play();
    }

    public void muteM()
    {
        if (audio.mute == false)
        {

            audio.mute = true;
            audio.Stop();
        }
        else
        {
            audio.Play(); 
        }
           
    }

}
