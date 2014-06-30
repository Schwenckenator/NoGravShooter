using UnityEngine;
using System.Collections;

public class BlackHolePhysics : MonoBehaviour {

	public float strength;
	public float radius;

	private Rigidbody[] hitsRigid;

	// Use this for initialization
	void Start () {
		transform.localScale = new Vector3(radius, radius, radius);
		GetComponent<ParticleSystem>().startSpeed = -radius;
		GetComponent<ParticleSystem>().emissionRate = radius;
		//Find Moveable objects in range
		Collider[] hits = Physics.OverlapSphere(transform.position, radius);
		int index = 0;
		Rigidbody[] sorting = new Rigidbody[hits.Length];
		foreach(Collider hit in hits){
			if(hit.CompareTag("Player")){
				sorting[index] = hit.rigidbody;
				index++;
			}
		}
		hitsRigid = new Rigidbody[index];

		for(int i=0; i < index; i++){
			hitsRigid[i] = sorting[i];
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//Move said captured objects towards me
		foreach(Rigidbody hit in hitsRigid){
			Vector3 forceDir = transform.position - hit.position;
			float forceMag = forceDir.magnitude;
			forceDir.Normalize();
			Vector3 totalForce = Vector3.ClampMagnitude((forceDir * strength)/forceMag, 100f);

			hit.AddForce(totalForce, ForceMode.Acceleration);
		}
	}
}
