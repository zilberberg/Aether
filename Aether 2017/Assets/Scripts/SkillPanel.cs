using UnityEngine;
using System.Collections;

public class SkillPanel : MonoBehaviour {
    [Tooltip("The Player's UI GameObject Prefab")]
    public GameObject EnemyPref;

    //public EnemyManager EnemyMgr;
    
    // Use this for initialization
    void Start () {
        //EnemyPref = GameObject.Find("Enemy(Clone)");
        //EnemyMgr = EnemyPref.GetComponent<EnemyManager>();


    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void testSkill()
    {
        
        //EnemyMgr.Health -= 0.2f;
    }
    

}
