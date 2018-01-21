using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Target : MonoBehaviour {
    [SerializeField] public string SecureKey;
    [SerializeField] public string addAetherURL;
    [SerializeField] private Text WarningMSG;

    private string userID;

    [SerializeField]
    private GameObject tank;
    [SerializeField]
    private GameObject healer;
    [SerializeField]
    private GameObject dps;

    private Animator ani;

    private string aetherClass;
    private float life, defence, offence, heal;
    private float em, gravity, wf, sf;
    private int lvl;

    


    bool dps_b = false, healer_b = false, tank_b = false;
    // Use this for initialization
    void Start () {        
        userID = PlayerPrefs.GetInt("userID").ToString();       

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }


    }

    public void TargetHealer()
    {
        healer_b = true;
        if (tank_b)
        {
            tank.GetComponent<Animator>().Play("tank reset");
            tank_b = false;
        }
        if (dps_b)
        {
            dps.GetComponent<Animator>().Play("dps reset");
            dps_b = false;
        }           
        healer.GetComponent<Animator>().Play("Healer animation");
        
    }

    public void TargetTank()
    {
        tank_b = true;
        if (healer_b)
        {
            healer.GetComponent<Animator>().Play("healer reset");
            healer_b = false;
        }
        if (dps_b)
        {
            dps.GetComponent<Animator>().Play("dps reset");
            dps_b = false;
        }
        
        tank.GetComponent<Animator>().Play("Tank animation");

    }

    public void TargetDPS()
    {
        dps_b = true;
        if (tank_b)
        {
            tank.GetComponent<Animator>().Play("tank reset");
            tank_b = false;
        }
        if (healer_b)
        {
            healer.GetComponent<Animator>().Play("healer reset");
            healer_b = false;
        }
        
        dps.GetComponent<Animator>().Play("DPS animation");
    }

    public void Verify()
    {
        if (healer_b)
        {            
            StartCoroutine(CreateHealer());            
        } else if (tank_b)
        {            
            StartCoroutine(CreateTank());            
        } else
        {            
            StartCoroutine(CreateDPS());           
        }
        
    }

    IEnumerator CreateHealer()
    {
        
        aetherClass = "healer";        
        lvl = 65;
        em = 40 * 5;
        gravity = 10 * 5;
        wf = 7 * 5;
        sf = 8 * 5;
        life = gravity * 10 + lvl * 2;
        offence = wf * 10 + lvl * 2;
        defence = sf * 10 + lvl * 2;
        heal = em * 10 + lvl * 2;
        WWW query = new WWW(addAetherURL + "?id=" + userID + "&secure=" + SecureKey + 
                            "&class=" + aetherClass + "&lvl=" + lvl + "&em=" + em + 
                            "&gravity=" + gravity + "&wf=" + wf + "&sf=" + sf + 
                            "&life=" + life + "&defence=" + defence + "&offence=" + offence + "&heal=" + heal);
        WarningMSG.text = "Please Wait ... " + userID;        
        yield return query;
        setPrefs(aetherClass, em, gravity, sf, wf, lvl);
        SceneManager.LoadScene("Map");

    }

    IEnumerator CreateDPS()
    {
        
        aetherClass = "DPS";
        lvl = 65;
        em = 5 * 5;
        gravity = 10 * 5;
        wf = 40 * 5;
        sf = 10 * 5;
        life = gravity * 10 + lvl * 2;
        offence = wf * 10 + lvl * 2;
        defence = sf * 10 + lvl * 2;
        heal = em * 10 + lvl * 2;
        WWW query = new WWW(addAetherURL + "?id=" + userID + "&secure=" + SecureKey + "&class=" + aetherClass + 
                            "&lvl=" + lvl + "&em=" + em + "&gravity=" + gravity + "&wf=" + wf + "&sf=" + sf + 
                            "&life=" + life + "&defence=" + defence + "&offence=" + offence + "&heal=" + heal);
        WarningMSG.text = "Please Wait ... " + userID;        
        yield return query;
        setPrefs(aetherClass, em, gravity, sf, wf, lvl);
        SceneManager.LoadScene("Map");
    }

    IEnumerator CreateTank()
    {
        
        aetherClass = "tank";
        lvl = 65;
        em = 5 * 5;
        gravity = 20 * 5;
        wf = 5 * 5;
        sf = 35 * 5;
        life = gravity * 10 + lvl * 2;
        offence = wf * 10 + lvl * 2;
        defence = sf * 10 + lvl * 2;
        heal = em * 10 + lvl * 2;
        WWW query = new WWW(addAetherURL + "?id=" + userID + "&secure=" + SecureKey + "&class=" + aetherClass + 
                            "&lvl=" + lvl + "&em=" + em + "&gravity=" + gravity + "&wf=" + wf + "&sf=" + sf + 
                            "&life=" + life + "&defence=" + defence + "&offence=" + offence + "&heal=" + heal);
        WarningMSG.text = "Please Wait ... " + userID;        
        yield return query;
        setPrefs(aetherClass, em, gravity, sf, wf, lvl);
        SceneManager.LoadScene("Map");
    }

    public void setPrefs(string aetherClass, float em, float gr, float sf, float wf, int lvl)
    {
        PlayerPrefs.SetString("Class", aetherClass);
        PlayerPrefs.SetFloat("em", em);
        PlayerPrefs.SetFloat("gr", gr);
        PlayerPrefs.SetFloat("sf", sf);
        PlayerPrefs.SetFloat("wf", wf);
        PlayerPrefs.SetInt("lvl", lvl);
    }


}
