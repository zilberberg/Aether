using UnityEngine;
using System.Collections;

public class ChangeP : MonoBehaviour {
    [SerializeField]
    Transform zoomObj;
	// Use this for initialization
	void Start () {
        this.transform.SetParent(zoomObj, false);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
