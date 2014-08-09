using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (NetworkView))]

public class RocketBurn : MonoBehaviour {

	public float rocketAccel;
	public GameObject rocketBlast;

	void FixedUpdate(){
		Vector3 force = Vector3.forward * rocketAccel;
		rigidbody.AddRelativeForce(force);
	}

	void OnCollisionEnter(){
		Network.Instantiate(rocketBlast, transform.position, Quaternion.identity, 0);
		Network.Destroy(gameObject);
	}
}
