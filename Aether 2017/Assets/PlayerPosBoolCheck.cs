using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPosBoolCheck : Photon.PunBehaviour, IPunObservable
{
    public bool occupied = false;
	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    [PunRPC]
    public void setOccupiedTrue()
    {        
        occupied = true;
    }
    [PunRPC]
    public void setOccupiedFalse()
    {
        occupied = false;
    }
    public bool getOccupiedBool()
    {
        return occupied;
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(occupied);
        }
        else
        {
            occupied = (bool)stream.ReceiveNext();
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(occupied);
        }
        else
        {
            occupied = (bool)stream.ReceiveNext();
        }
    }
}
