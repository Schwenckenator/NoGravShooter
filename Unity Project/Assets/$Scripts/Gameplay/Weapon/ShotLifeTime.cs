using UnityEngine;
using System.Collections;

public class ShotLifeTime : MonoBehaviour {

	public float lifeTime;
	private float deathTime;

	// Use this for initialization
	void Start () {
		deathTime = Time.time + lifeTime;
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time > deathTime){
			if(GetComponent<ObjectCleanUp>() != null){
				GetComponent<ObjectCleanUp>().KillMe(true);
			}else{
				Destroy(gameObject);
			}

		}
	}
}
