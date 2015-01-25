using UnityEngine;
using System.Collections;

public class EMPExplosion : MonoBehaviour {
	// An EMP mine shuts down the jetpack
	
	public float radius;
	public int strength;

	// Use this for initialization
	void Start () {
		Bang ();
	}
	
	void Bang(){
		Collider[] hits;
		hits = Physics.OverlapSphere(transform.position, radius);
		
		foreach(Collider hit in hits){
			
			if(hit.CompareTag("Player")){

				// Nuke the jetpack!
				hit.GetComponent<PlayerResources>().SpendFuel(strength, true);
			}
		}
	}
}
