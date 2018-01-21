﻿using UnityEngine;
using System.Collections;

public class GyroCamera : MonoBehaviour
{
    private Gyroscope gyro;
    private bool gyroSupported;
    private Quaternion rotFix;

    [SerializeField]
    private Transform worldObj;
    private float startY;
    
    [SerializeField]
    private Transform zoomObj;

    //[SerializeField]
    //private Transform cameraTransform;
    // Use this for initialization
    void Start() {
        //cameraTransform = Camera.main.transform;

        gyroSupported = SystemInfo.supportsGyroscope;

        GameObject camParent = new GameObject("camParent");
        camParent.transform.position = transform.position;
        transform.parent = camParent.transform;

        if (gyroSupported) {
            gyro = Input.gyro;
            gyro.enabled = true;

            camParent.transform.rotation = Quaternion.Euler(90f, 180f, 0f);
            rotFix = new Quaternion(0, 0, 1, 0);

            //ResetGyroRotation();
        }
    }

    // Update is called once per frame
    void Update() {
        if(gyroSupported && startY == 0) {
            ResetGyroRotation();
        }
        transform.localRotation = gyro.attitude * rotFix;
    }

    public void ResetGyroRotation() {
        
        int x = Screen.width / 2;
        int y = Screen.height / 2;

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(x, y));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 500))
        {
            Vector3 hitPoint = hit.point;
            hitPoint.y = 0;

            float z = Vector3.Distance(Vector3.zero, hitPoint);
            zoomObj.localPosition = new Vector3(0f, zoomObj.localPosition.y, Mathf.Clamp(z,2f,10f));
        }
        
        startY = transform.eulerAngles.y;
        worldObj.rotation = Quaternion.Euler(0f, startY, 0f);
    }
}
