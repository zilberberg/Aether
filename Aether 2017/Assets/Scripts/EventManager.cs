using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EventManager : MonoBehaviour {
    private TileManager tileManager;
    [SerializeField]
    private float waitSpawnTime, minIntervalTime, maxIntervalTime;

    private List<AlienEvent> alienEvents = new List<AlienEvent>();

    private float newLat = 31.532274f, newLon = 34.587860f;
	// Use this for initialization
	void Start () {
        tileManager = GameObject.FindGameObjectWithTag("TileManager").GetComponent<TileManager>();
        //SpawnEvent();

    }
	
	// Update is called once per frame
	void Update () {
        UpdateEventPosition();
        //if(waitSpawnTime < Time.time) {
        //    waitSpawnTime = Time.time + UnityEngine.Random.Range(minIntervalTime, maxIntervalTime);
        //    SpawnEvent();
        //}

    }

    void SpawnEvent()
    {
        //AlienEventType type = (AlienEventType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(AlienEventType)).Length);
        //newLat = tileManager.getLat + 31.532274f;
        //newLon = tileManager.getLat + 34.587860f;
        //+type.ToString()
        AlienEvent prefab = Resources.Load("MapEvents/Cube"  , typeof(AlienEvent)) as AlienEvent;
        AlienEvent alienEvent = Instantiate(prefab, Vector3.zero, Quaternion.identity) as AlienEvent;
        alienEvent.tileManager = tileManager;
        alienEvent.Init(newLat,newLon);
        alienEvents.Add(alienEvent);
    }

    public void UpdateEventPosition()
    {
        if (alienEvents.Count == 0)
            return;
        AlienEvent[] alienEvent = alienEvents.ToArray();
        alienEvent[0].UpdatePosition();
    }
}
