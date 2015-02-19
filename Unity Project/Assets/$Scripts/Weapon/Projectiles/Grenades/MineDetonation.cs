using UnityEngine;
using System.Collections;
[RequireComponent(typeof(ObjectCleanUp))]

public class MineDetonation : MonoBehaviour {

	public float detectionRadius; 
	public GameObject explosion;
	public float initialWaitTime;
	public float tickWaitTime;

    private bool detonated;


	// Use this for initialization
	void Start () {
		StartCoroutine(CheckForPlayers(initialWaitTime, tickWaitTime));
	}

	IEnumerator CheckForPlayers(float initWait, float tickWait){
		yield return new WaitForSeconds(initWait);
		while(!detonated){
			Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);
			foreach(Collider hit in hits){
				if(hit.CompareTag("Player")){
					Detonate(false);
					break;
				}
			}
			yield return new WaitForSeconds(tickWait);
		}
	}

	public void ForceDetonate(){
		Detonate(true);
	}

    public void OnCollisionEnter(Collision info) {
        if (info.collider.CompareTag("Player")) {
            Detonate(false);
        }
    }

	void Detonate(bool isForced){
        if (detonated) return;
        detonated = true;

        if (isForced || Network.isServer) {
            ChatManager.DebugMessage(NetworkManager.GetPlayer(Network.player).Name + " says " + gameObject.ToString() + " goes boom.");

            SpawnExplosion(transform.position, Quaternion.identity, GetComponent<Owner>().ID);
            GetComponent<ObjectCleanUp>().KillMe();
        }
	}

    [RPC]
    void SpawnExplosion(Vector3 position, Quaternion rotation, NetworkPlayer owner) {
        if (Network.isServer) {
            GameObject newObj = Network.Instantiate(explosion, position, rotation, 0) as GameObject;
            if (newObj.GetComponent<Owner>() != null) {
                newObj.GetComponent<Owner>().ID = owner;
            }

        } else {
            networkView.RPC("SpawnExplosion", RPCMode.Server, position, rotation, owner);
        }
    }
}
