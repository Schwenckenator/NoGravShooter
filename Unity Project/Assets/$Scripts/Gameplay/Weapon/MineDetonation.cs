using UnityEngine;
using System.Collections;

public class MineDetonation : MonoBehaviour {

	public float detectionRadius; 
	public GameObject explosion;
	public float initialWaitTime;
	public float tickWaitTime;

	// Use this for initialization
	void Start () {
		StartCoroutine(CheckForPlayers(initialWaitTime, tickWaitTime));
	}

	IEnumerator CheckForPlayers(float initWait, float tickWait){
		yield return new WaitForSeconds(initWait);
		while(true){
			Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);
			foreach(Collider hit in hits){
				if(hit.CompareTag("Player")){
					Detonate();
					break;
				}
			}
			yield return new WaitForSeconds(tickWait);
		}
	}

	void Detonate(){

		//Make something depending on what you are
		Instantiate(explosion, transform.position, Quaternion.identity);

		//Then destroy this
		Destroy(gameObject);
	}
}
