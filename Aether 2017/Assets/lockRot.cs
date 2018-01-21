using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lockRot : MonoBehaviour {
    private Quaternion iniRot;
	// Use this for initialization
	void Start () {
        iniRot = transform.rotation;
    }
	
	// Update is called once per frame
	void LateUpdate () {
        transform.rotation = iniRot;
	}
}
