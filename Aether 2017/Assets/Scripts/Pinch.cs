using UnityEngine;
using System.Collections;

public class Pinch : MonoBehaviour {

    private float orthoOrg; 
    private float orthoCurr;
    private Vector3 scaleOrg ;
    private Vector3 posOrg;
  
    void Start()
    {
        orthoOrg = Camera.main.orthographicSize;
        orthoCurr = orthoOrg;
        scaleOrg = transform.localScale;
        posOrg = Camera.main.WorldToViewportPoint(transform.position);
    }

    void Update()
    {
        var osize = Camera.main.orthographicSize;
        if (orthoCurr != osize)
        {

            transform.localScale = scaleOrg * osize / orthoOrg;
            orthoCurr = osize;
            transform.position = Camera.main.ViewportToWorldPoint(posOrg);
        }
    }
}