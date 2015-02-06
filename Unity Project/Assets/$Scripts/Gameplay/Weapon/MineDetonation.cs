using UnityEngine;
using System.Collections;
[RequireComponent(typeof(ObjectCleanUp))]

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

	public void Shot(){
		Detonate();
	}

	void Detonate(){

		//Make something depending on what you are
		GameObject boom = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
        boom.GetComponent<ProjectileOwnerName>().ProjectileOwner = GetComponent<ProjectileOwnerName>().ProjectileOwner;
		//Then destroy this
        GetComponent<ObjectCleanUp>().KillMe();
	}
}
