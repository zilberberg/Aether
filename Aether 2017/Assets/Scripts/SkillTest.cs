using UnityEngine;
using System.Collections;

public class SkillTest : MonoBehaviour {
    private bool released, holding;
    [SerializeField]
    
    public GameObject effect;
    public GameObject curObj;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        
        
        if (holding)
        {
            for (var i = 0; i < Input.touchCount; i++)
            {
                if (Input.GetTouch(i).phase == TouchPhase.Began)
                {
                    if (Input.GetTouch(i).position.x == GameObject.Find("Sphere").transform.position.x && Input.GetTouch(i).position.y == GameObject.Find("Sphere").transform.position.y)
                    {
                        Vector3 vec = Input.GetTouch(i).position;
                        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit, 1000))
                        {
                            Instantiate(effect, hit.point, Quaternion.identity);
                        }
                    }
                    
                }
            }
        }
        if (released)
        {
            return;
        }
        if (Input.GetTouch(0).phase == TouchPhase.Began)
        {
            holding = true;
        }
        if (Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            released = true;
        }
        
	
	}
    public void InstantiateEffect()
    {
        for (var i = 0; i < Input.touchCount; i++)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                Vector3 vec = Input.GetTouch(i).position;

                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                //var hit = RaycastHit;
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000))
                {
                    Instantiate(effect, hit.point, Quaternion.identity);
                }
                //var touchPos : Vector3 = Input.GetTouch(i).position;
                //touchPos.z = 4.0f;
                //var createPos = myCam.ScreenToWorldPoint(touchPos);

                //Instantiate(effect, Vec, Quaternion.identity);
            }
        }
    }
    /*
    void Reset()
    {

    }

    void OnTouch()
    {
        GetComponent<GUIElement>().GetComponent();
    }
    */

}
