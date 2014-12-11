using UnityEngine;
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
		if(input.CompareTag("Player")){ // Hit a player!
			//If you hit yourself, don't do anything
			if(networkView.isMine && input.networkView.isMine){
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
		input.TakeDamage(damage);
	}

	void PushObject(Rigidbody rigid){
		rigid.AddForce(transform.forward * push, ForceMode.Impulse);
	}

}