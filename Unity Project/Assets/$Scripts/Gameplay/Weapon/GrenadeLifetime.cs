using UnityEngine;
using System.Collections;

public class GrenadeLifetime : MonoBehaviour {

	public float lifeTime;
	public GameObject explosion;
	private float deathTime;
	
	// Use this for initialization
	void Start () {
		deathTime = Time.time + lifeTime;
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time > deathTime){
			//Make something depending on what you are
			Instantiate(explosion, transform.position, Quaternion.identity);
			//Then destroy this
			Destroy(gameObject);
		}
	}
}
