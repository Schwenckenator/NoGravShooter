using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlackHolePhysics : MonoBehaviour {

	public float strength;
	public float maxStrength;
	public float radius;

	private List<Rigidbody> hitsRigid;
    private ActorMotorManager actor;
    private Rigidbody actorRigid;

	// Use this for initialization
	void Start () {
		transform.localScale = new Vector3(radius, radius, radius);
		GetComponent<ParticleSystem>().startSpeed = -radius;
		GetComponent<ParticleSystem>().emissionRate = radius;
		hitsRigid = new List<Rigidbody>();
		hitsRigid.Clear();
		//Find Moveable objects in range
		StartCoroutine(CheckForNewObjects());
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//Move said captured objects towards me
		for(int i=0; i<hitsRigid.Count; i++){
			if(hitsRigid[i] == null || hitsRigid[i].CompareTag("ForceProjectile")){
				continue;
			}
			Vector3 forceDir = transform.position - hitsRigid[i].position;
			float forceMag = forceDir.magnitude;
			forceDir.Normalize();
			Vector3 totalForce = Vector3.ClampMagnitude((forceDir * strength)/forceMag, maxStrength);

			hitsRigid[i].AddForce(totalForce, ForceMode.Acceleration);
            if (hitsRigid[i] == actorRigid) {
                if (totalForce.sqrMagnitude > 1) {
                    actor.PushOffGround();
                }
            }
		}
	}

	IEnumerator CheckForNewObjects(){
        yield return null; // Wait for a frame
		while(true){
			Collider[] hits = Physics.OverlapSphere(transform.position, radius);
			foreach(Collider hit in hits){
				if(hit.GetComponent<Rigidbody>()){
					if(!hitsRigid.Contains(hit.GetComponent<Rigidbody>())){
						hitsRigid.Add(hit.GetComponent<Rigidbody>());
					}
				}
                //if (hit.CompareTag("Player") && hit.GetComponent<NetworkView>().isMine) {
                    actorRigid = hit.GetComponent<Rigidbody>();
                    actor = hit.GetComponent<ActorMotorManager>();
                //}
			}

			yield return new WaitForSeconds(0.5f);
		}
	}
}
