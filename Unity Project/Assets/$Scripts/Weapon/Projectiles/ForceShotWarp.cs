﻿using UnityEngine;
using System.Collections;
[RequireComponent(typeof(ObjectCleanUp))]

public class ForceShotWarp : MonoBehaviour {

	public int damage;

	public float push;

	public float xWarp;
	public float yWarp;

	void Start(){
		transform.Translate(new Vector3(0, 0, 1), Space.Self);
	}

	void FixedUpdate(){
		transform.localScale = new Vector3( transform.localScale.x + (xWarp * Time.deltaTime), transform.localScale.y + (yWarp * Time.deltaTime), transform.localScale.z);
	}

	void OnTriggerEnter(Collider input){

		bool push = true;

		if(input.CompareTag("BonusPickup")){ // If this hits a bonus, kill it
			input.GetComponent<DestroyOnNextFrame>().DestroyMe();
		}
        if (input.CompareTag("MineGrenade")) {
            input.GetComponent<MineDetonation>().ForceDetonate();
        }

		if(input.CompareTag("Player")){ // Hit a player!
			//If you hit yourself, don't do anything
			if(input.networkView.owner == GetComponent<ProjectileOwnerName>().ProjectileOwner){
				push = false;
			}else{
				DamagePlayer(input.GetComponent<PlayerResources>());
			}
		}
		if(input.rigidbody && push){
			PushObject(input.rigidbody);
		}
	}

	void DamagePlayer(PlayerResources input){
		input.TakeDamage(damage, GetComponent<ProjectileOwnerName>().ProjectileOwner);
	}

	void PushObject(Rigidbody rigid){
		rigid.AddForce(transform.forward * push, ForceMode.Impulse);
	}

}