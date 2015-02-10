using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (NetworkView))]
[RequireComponent (typeof (ProjectileOwnerName))]

public class RocketBurn : MonoBehaviour {

	public float rocketAccel;
	public float startVelocity;

    private bool moving = true;

	void Start(){
		if(!Network.isServer) return;

		Vector3 vel = transform.forward * startVelocity;
        vel += GetPlayerZVelocity();

		rigidbody.AddForce(vel, ForceMode.VelocityChange);
	}

	void FixedUpdate(){
        if (!Network.isServer || !moving) return;

        Rotate();
        Push();

	}

    private void Rotate() {
        Quaternion newRotation = Quaternion.LookRotation(rigidbody.velocity);
        transform.rotation = newRotation;
    }
    private void Push() {
        if (rocketAccel <= 0) return;
        Vector3 force = Vector3.forward * rocketAccel;
        rigidbody.AddRelativeForce(force);
    }

    public void Disable() {
        moving = false;
    }

    private Vector3 GetPlayerZVelocity() {
        Vector3 playerVel = Vector3.zero;
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
            if (player.networkView.owner == GetComponent<ProjectileOwnerName>().ProjectileOwner) {
                playerVel = StripXYaxisFromPlayerVelocity(player);
            }
        }
        return playerVel;
    }

    private Vector3 StripXYaxisFromPlayerVelocity(GameObject player) {
        
        Vector3 velocity = player.rigidbody.velocity;
        velocity = player.transform.InverseTransformVector(velocity);
        velocity = new Vector3(0, 0, velocity.z);
        
        ChatManager.PrintMessageIfDebug("Local Z Velocity: " + velocity.ToString());

        velocity = player.transform.TransformVector(velocity);

        return velocity;
    }
}
