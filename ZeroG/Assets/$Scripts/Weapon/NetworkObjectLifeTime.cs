using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NetworkObjectLifeTime : NetworkBehaviour {

	public float lifeTime;
	private float deathTime;
    private bool cleanUp = false;
    private bool killed = false;

	// Use this for initialization
	void Start () {
        deathTime = Time.time + lifeTime;
	}
	
	// Update is called once per frame
	void Update () {
        if (!NetworkServer.active) return;

        if(Time.time > deathTime && !killed){
            killed = true;
            NetworkServer.Destroy(gameObject);
        }
	}
}
