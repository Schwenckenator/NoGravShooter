using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (NetworkView))]
[RequireComponent (typeof (Owner))]

public class RocketBurn : MonoBehaviour {

	public float rocketAccel;
	public float startVelocity;
    public bool fullyRelative; // Is fully relative, or just Z relative?

    private bool moving = true;

	void Start(){
		if(!Network.isServer) return;

		Vector3 vel = transform.forward * startVelocity;
        vel += GetPlayerVelocity(fullyRelative);

		GetComponent<Rigidbody>().AddForce(vel, ForceMode.VelocityChange);
	}

	void FixedUpdate(){
        if (!Network.isServer || !moving || (rocketAccel <= 0)) return;
        Push();
        //Rotate();
	}

    private void Rotate() {
        Quaternion newRotation = Quaternion.LookRotation(GetComponent<Rigidbody>().velocity);
        transform.rotation = newRotation;
    }
    private void Push() {
        Vector3 force = Vector3.forward * rocketAccel;
        GetComponent<Rigidbody>().AddRelativeForce(force);
    }

    public void Disable() {
        moving = false;
    }

    private Vector3 GetPlayerVelocity(bool fullRelative) {
        Vector3 playerVel = Vector3.zero;
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
            if (player.GetComponent<NetworkView>().owner == GetComponent<Owner>().ID) {
                playerVel = fullRelative ? player.GetComponent<Rigidbody>().velocity : StripXYaxisFromPlayerVelocity(player);
            }
        }
        return playerVel;
    }

    private Vector3 StripXYaxisFromPlayerVelocity(GameObject player) {
        
        Vector3 velocity = player.GetComponent<Rigidbody>().velocity;

        velocity = player.transform.InverseTransformVector(velocity); // Convert to local space
        velocity = new Vector3(0, 0, velocity.z);

        if (IsNegativeZVelocity(velocity)) return Vector3.zero; // Don't go backwards

        velocity = player.transform.TransformVector(velocity); // Convert to world space

        return velocity;
    }

    private bool IsNegativeZVelocity(Vector3 value){
        return (value.z < 0);
    }
}
