using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (NetworkView))]

public class RocketBurn : MonoBehaviour {

	public float rocketAccel;
	public float startVelocity;
	public GameObject rocketBlast;

	void Start(){
		if(!networkView.isMine) return;

		Vector3 playerVel = Vector3.zero;
		foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player")){
			if(player.networkView.isMine){
				playerVel = player.rigidbody.velocity;
			}
		}

		Vector3 vel = transform.forward * startVelocity;
		vel += playerVel;

		rigidbody.AddForce(vel, ForceMode.VelocityChange);
	}

	void FixedUpdate(){
		if(Network.isServer){
			Vector3 force = Vector3.forward * rocketAccel;
			rigidbody.AddRelativeForce(force);
		}
	}

	void OnCollisionEnter(){
		if(Network.isServer){

			GameObject explosion = Network.Instantiate(rocketBlast, transform.position, Quaternion.identity, 0) as GameObject;
            explosion.GetComponent<ProjectileOwnerName>().ProjectileOwner = GetComponent<ProjectileOwnerName>().ProjectileOwner;

			GetComponent<ObjectCleanUp>().KillMe();
		}
	}
}
