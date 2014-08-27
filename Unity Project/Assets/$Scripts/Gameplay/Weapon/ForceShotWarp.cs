using UnityEngine;
using System.Collections;

public class ForceShotWarp : MonoBehaviour {

	public int damage;

	public float push;

	public float xWarp;
	public float yWarp;
	
	private float endTime;

	void Start(){
		transform.Translate(new Vector3(0, 0, 1), Space.Self);
		endTime = Time.time + GetComponent<ShotLifeTime>().lifeTime;
	}

	void FixedUpdate(){
		if(Time.time > endTime){
			GetComponent<ObjectCleanUp>().KillMe();
		}
		transform.localScale = new Vector3( transform.localScale.x + (xWarp * Time.deltaTime), transform.localScale.y + (yWarp * Time.deltaTime), transform.localScale.z);
	}

	void OnTriggerEnter(Collider input){
		if(input.CompareTag("Player")){ // Hit a player!
			DamagePlayer(input.GetComponent<PlayerResources>());
		}
		if(input.rigidbody){
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