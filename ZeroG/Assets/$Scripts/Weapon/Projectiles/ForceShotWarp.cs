using UnityEngine;
using System.Collections;
using System;


public class ForceShotWarp : MonoBehaviour, IOwnable {

	int damage;

	public float push;

	public float xWarp;
	public float yWarp;

    public LobbyPlayer owner {get; set;}

    void Start(){
		transform.Translate(new Vector3(0, 0, 1), Space.Self);
        damage = ForceCannon.instance.damage;
	}

	void FixedUpdate(){
		transform.localScale = new Vector3( transform.localScale.x + (xWarp * Time.deltaTime), transform.localScale.y + (yWarp * Time.deltaTime), transform.localScale.z);
	}

	void OnTriggerEnter(Collider hit){

		bool push = true;

		if(hit.CompareTag("BonusPickup")){ // If this hits a bonus, kill it
			hit.GetComponent<DestroyOnNextFrame>().DestroyMe();
		}

		if(hit.CompareTag("Player")){ // Hit a player!
			//If you hit yourself, don't do anything
            if (hit.GetComponent<LobbyPlayer>() == owner) {
				push = false;
			}else{
				DamagePlayer(hit.gameObject.GetInterface<IDamageable>());
                // Add Push off ground here
                hit.GetComponent<ActorMotorManager>().PushOffGround();
			}
		}
		if(push && hit.GetComponent<Rigidbody>()){
			PushObject(hit.GetComponent<Rigidbody>());
		}
	}

	void DamagePlayer(IDamageable input){
		input.TakeDamage(damage, owner,ForceCannon.instance.id);
	}

	void PushObject(Rigidbody rigid){
		rigid.AddForce(transform.forward * push, ForceMode.Impulse);
	}

}