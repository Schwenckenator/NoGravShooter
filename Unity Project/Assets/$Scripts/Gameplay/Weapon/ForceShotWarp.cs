using UnityEngine;
using System.Collections;

public class ForceShotWarp : MonoBehaviour {

	public int maxDamage;
	public int minDamage;

	public float maxPush;
	public float minPush;

	public float xWarp;
	public float yWarp;

	private float shotLifeTime;
	private float startTime;
	private float endTime;

	void Start(){
		transform.Translate(new Vector3(0, 0, 1), Space.Self);
		shotLifeTime = GetComponent<ShotLifeTime>().lifeTime;
		startTime = Time.time;
		endTime = Time.time + shotLifeTime;
	}

	void FixedUpdate(){
		if(Time.time > endTime){
			Network.Destroy (gameObject);
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
		int damage = (int)Mathf.Lerp(maxDamage, minDamage, (Time.time - startTime) / shotLifeTime);
		input.TakeDamage(damage);
	}

	void PushObject(Rigidbody rigid){
		float push = Mathf.Lerp(maxPush, minPush, (Time.time - startTime) / shotLifeTime);
		rigid.AddForce(transform.forward * push, ForceMode.Impulse);
	}

}