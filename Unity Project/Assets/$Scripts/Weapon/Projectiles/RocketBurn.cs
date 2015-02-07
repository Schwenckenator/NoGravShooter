using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (NetworkView))]
[RequireComponent (typeof (ProjectileOwnerName))]

public class RocketBurn : MonoBehaviour {

	public float rocketAccel;
	public float startVelocity;


	void Start(){
		if(!Network.isServer) return;

		Vector3 playerVel = Vector3.zero;
		foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player")){
			if(player.networkView.owner == GetComponent<ProjectileOwnerName>().ProjectileOwner){
				playerVel = player.rigidbody.velocity;
			}
		}

		Vector3 vel = transform.forward * startVelocity;
		vel += playerVel;

		rigidbody.AddForce(vel, ForceMode.VelocityChange);
	}

	void FixedUpdate(){
        if (!Network.isServer) return;

		Vector3 force = Vector3.forward * rocketAccel;
		rigidbody.AddRelativeForce(force);

	}

	
}
