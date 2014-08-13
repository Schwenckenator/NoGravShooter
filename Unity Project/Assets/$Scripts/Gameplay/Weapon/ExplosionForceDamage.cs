using UnityEngine;
using System.Collections;

public class ExplosionForceDamage : MonoBehaviour {

	public float explosionPower;
	public float explosionRadius;
	public float maxDamage;


	void Start(){
		Bang ();
	}

	void Bang(){
		Collider[] hits;
		hits = Physics.OverlapSphere(transform.position, explosionRadius);

		foreach(Collider hit in hits){
			if(hit.rigidbody){
				hit.rigidbody.AddExplosionForce(explosionPower, transform.position, explosionRadius, 1.0f);
			}
			if(hit.CompareTag("Player")){
				//Find distance
				float distance = (hit.transform.position - transform.position).magnitude;
				float damage = maxDamage / Mathf.Max(distance, 1);

				hit.GetComponent<PlayerResources>().TakeDamage((int)damage);
			}
		}
	}
}
