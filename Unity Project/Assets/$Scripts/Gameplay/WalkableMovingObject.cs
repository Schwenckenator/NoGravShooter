/*
 * This script responds to player collisions by making the player a child of this object
 * When the player leave the collision, it de-parents it.
 */

using UnityEngine;
using System.Collections;

public class WalkableMovingObject : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision info){
		if(info.collider.CompareTag("Player")){
			info.transform.parent = transform;
		}
	}
	void OnCollisionExit(Collision info){
		if(info.collider.CompareTag("Player")){
			info.transform.parent = null;
		}
	}
}
