using UnityEngine;
using System.Collections;

public class ShotLifeTime : MonoBehaviour {

	public float lifeTime;
	private float deathTime;
    private bool cleanUp = false;
    private bool killed = false;

	// Use this for initialization
	void Start () {
        if (GetComponent<ObjectCleanUp>() != null) {
            deathTime = Time.time + lifeTime;
            cleanUp = true;
        }else{
            Destroy(gameObject, lifeTime);
        }
	}
	
	// Update is called once per frame
	void Update () {
        if(cleanUp && Time.time > deathTime && !killed){
            killed = true;
            GetComponent<ObjectCleanUp>().KillMe();
        }
	}
}
