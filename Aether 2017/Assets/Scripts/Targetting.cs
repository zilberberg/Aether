using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Targetting : MonoBehaviour {
    public List<Transform> targets;
    public Transform selectedTarget;

	// Use this for initialization
	void Start () {
        targets = new List<Transform>();
        selectedTarget = null;

        AddAllEnemies();
	
	}
    public void AddAllEnemies()
    {
        GameObject[] go = GameObject.FindGameObjectsWithTag("Enemies");
        foreach(GameObject enemy in go)
        {
            AddTarget(enemy.transform);
        }
    }
    public void AddTarget(Transform enemy)
    {
        targets.Add(enemy);
    }
	
    private void TargetEnemy()
    {
        selectedTarget = targets[0];
    }
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TargetEnemy();
        }
	
	}
}
