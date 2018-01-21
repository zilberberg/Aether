using UnityEngine;
using System.Collections;

public class testCamera : MonoBehaviour {

    [Tooltip("The distance in the local x-z plane to the target")]
    public float distance = 7.0f;

    [Tooltip("The height we want the camera to be above the target")]
    public float height = 3.0f;

    [Tooltip("The Smooth time lag for the height of the camera.")]
    public float heightSmoothLag = 0.3f;

    [Tooltip("Allow the camera to be offseted vertically from the target, for example giving more view of the sceneray and less ground.")]
    public Vector3 centerOffset = Vector3.zero;

    public float compassDegree = 0;

    [SerializeField]
    public GameObject webCameraPlane;
    //public Button fireButton;
    Transform cameraTransform;

    private WebCamTexture webCameraTexture;
    private Vector3 euler;
    private Quaternion quant;
    private float cwNeeded = -90;

    // Use this for initialization
    void Start()
    {
        cameraTransform = Camera.main.transform;
        if (Application.isMobilePlatform)
        {
            GameObject cameraParent = new GameObject("camParent");
            cameraParent.transform.position = this.transform.position;
            this.transform.parent = cameraParent.transform;
            cameraParent.transform.Rotate(Vector3.right, 90);
            cameraParent.transform.Rotate(Vector3.forward, compassDegree);

        }

        Input.gyro.enabled = true;

        //fireButton.onClick.AddListener(OnButtonDown);


        webCameraTexture = new WebCamTexture();
        webCameraPlane.GetComponent<MeshRenderer>().material.mainTexture = webCameraTexture;
        webCameraTexture.Play();




    }


    // Update is called once per frame
    void Update()
    {

        Quaternion cameraRotation = new Quaternion(Input.gyro.attitude.x, Input.gyro.attitude.y, -Input.gyro.attitude.z, -Input.gyro.attitude.w);
        this.transform.localRotation = cameraRotation;

        //float cwNeeded = -webCameraTexture.videoRotationAngle;
        //Debug.Log("cwNeeded: " + cwNeeded);
        if (webCameraTexture != null)
        {
            if (webCameraTexture.videoVerticallyMirrored)
            {
                cwNeeded += 180f;
                //Transform planeTransform = webCameraPlane.GetComponent<Transform>();
                //planeTransform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                //webCameraPlane.GetComponent<Transform>().localRotation = planeTransform.localRotation;
                //planeTransform.Rotate(180f, 0, 0);
                euler = webCameraPlane.GetComponent<Transform>().eulerAngles;
                quant = webCameraPlane.transform.rotation;
                euler.x += 180f;
                //euler.z += 180f;
                //webCameraPlane.transform.rotation = Quaternion.Euler(euler.x, euler.y, euler.z);
                //webCameraPlane.transform.eulerAngles = euler;
                webCameraPlane.transform.rotation = Quaternion.Euler(-90f, 0, 0);
            }
        }
        
        if (cwNeeded != 0)
        {
            //webCameraPlane.transform.rotation = Quaternion.Euler(cwNeeded, 0f, 0f);
        }
        
        /*
        float cwNeeded = -cam.videoRotationAngle;
        if (cam.videoVerticallyMirrored) {
            cwNeeded += 180f;
        }
        image.rectTransform.localEulerAngles = new Vector3(0f, 0f, cwNeeded);
        float videoRatio = (float)cam.width / (float)cam.height;
        arf.aspectRatio = videoRatio;
        
        if (cam.videoVerticallyMirrored)
        {
            image.uvRect = new Rect(1, 0, -1, 1);
        } else
        {
            image.uvRect = new Rect(0, 0, 1, 1);
        }
        */

    }
    public IEnumerator killCamTexture()
    {
        if (webCameraTexture != null)
            webCameraTexture.Stop();

        yield return new WaitForSeconds(0.2f);

        if (webCameraTexture != null)
            Destroy(webCameraTexture);


        yield return new WaitForSeconds(0.2f);
    }

    public void ChangePos(Vector3 v3)
    {
        //transform.position = v3;
        //cameraTransform.position = v3;
    }
}
