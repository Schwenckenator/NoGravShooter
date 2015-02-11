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

	public void ForceDetonate(){
		Detonate();
	}

	void Detonate(){
        ChatManager.PrintMessageIfDebug(gameObject.ToString() + " goes boom");
        
        //Make something depending on what you are
        SpawnExplosion(transform.position, Quaternion.identity, GetComponent<ProjectileOwnerName>().ProjectileOwner);
		
		//Then destroy this
        GetComponent<ObjectCleanUp>().KillMe();
	}

    [RPC]
    void SpawnExplosion(Vector3 position, Quaternion rotation, NetworkPlayer owner) {
        if (Network.isServer) {
            GameObject newObj = Network.Instantiate(explosion, position, rotation, 0) as GameObject;
            if (newObj.GetComponent<ProjectileOwnerName>() != null) {
                newObj.GetComponent<ProjectileOwnerName>().ProjectileOwner = owner;
            }

        } else {
            networkView.RPC("SpawnExplosion", RPCMode.Server, position, rotation, owner);
        }
    }
}
