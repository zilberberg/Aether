using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RotateMap : MonoBehaviour {
    public float speed = 0.1F;
    private RawImage image;
    private Vector3 v3;
    private Vector3 CapVec;
    private GameObject Cap;

    // Use this for initialization
    void Start () {
        image = GetComponent<RawImage>();

        Cap = GetComponent<GameObject>();
        //CapVec = Cap.transform.;
        

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (Input.touchCount > 0 && Input.touchCount < 2 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            // Get movement of the finger since last frame
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;

            // Move object across XY plane
            image.transform.Rotate(Vector3.forward * -touchDeltaPosition.x * speed);
            //image.transform.RotateAround(Vector3.zero, Vector3.up, -touchDeltaPosition.x * speed);         
        }
        
    }
}
