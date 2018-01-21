using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour {

    [SerializeField] public string SecureKey;
    [SerializeField] public string ChooseClassURL;
    [SerializeField] public string ChangeClassURL;
    [SerializeField] private Text WarningMSG;
    [SerializeField] public GameObject world;
    [SerializeField] public Canvas settings;

    private string userID;
    private WebCamTexture cam;
    private bool settingFlag = false;

    // Use this for initialization
    void Start() {
        if (settings != null)
        {
            settings.enabled = false;
        }
        userID = PlayerPrefs.GetInt("userID").ToString();
        cam = new WebCamTexture(Screen.width, Screen.height);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void LoadMain()
    {
        //myEvent.RemoveAllListeners();
        SceneManager.LoadScene("Main");

    }
    public void LoadBattle()
    {
        SceneManager.LoadScene("Battle");
    }

    public void LoadMap()
    {
        SceneManager.LoadScene("Map");
    }
    public void LoadClassChoose()
    {
        SceneManager.LoadScene("ClassChoose");
    }
    public void continueTo()
    {
        StartCoroutine(QueryAether());
    }
    public void changeC()
    {
        StartCoroutine(ChangeClass());
    }
    public void LoadAccount()
    {
        SceneManager.LoadScene("AfterLogin");
    }
    public void LoadGAS()
    {
        PlayerPrefs.DeleteKey("userID");
        SceneManager.LoadScene("GlobalAccountSystem");
    }
    public void LoadDemoBattle()
    {
        SceneManager.LoadScene("DemoBattle");
    }
    public void BackToMap()
    {
        cam.Stop();
        Destroy(world);
        StartCoroutine(Map());
    }
    public void PUNBasic()
    {
        SceneManager.LoadScene("PunBasics-Launcher");
    }
    IEnumerator Map()
    {
        
        yield return null;        
        SceneManager.LoadScene("Map");
    }

    IEnumerator QueryAether()
    {
        WWW query = new WWW(ChooseClassURL + "?id=" + userID + "&secure=" + SecureKey);
        WarningMSG.text = "Please Wait ... "+userID;
        yield return query;
        string[] split = query.text.Split(',');
        if (split[0].Trim() == "0")
        {
            LoadClassChoose();
        } else if(query.text.Trim() == "1")
        {
            LoadMap();
        }
    }

    IEnumerator ChangeClass()
    {
        WWW query = new WWW(ChangeClassURL + "?id=" + userID + "&secure=" + SecureKey);
        WarningMSG.text = "Please Wait ... " + userID;
        yield return query;
        continueTo();
    }

    public void openSettings()
    {
        if (settingFlag)
        {
            settings.enabled = false;
            settingFlag = false;
        } else
        {
            settings.enabled = true;
            settingFlag = true;
        }
    }
}
