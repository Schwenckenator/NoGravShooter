using UnityEngine;
using System.Collections;
using System.Linq;
[RequireComponent(typeof(ObjectCleanUp))]
[RequireComponent(typeof(StickyObject))]
public class MineDetonation : MonoBehaviour, IDamageable {

	public float detectionRadius; 
	public GameObject explosion;
	public float initialWaitTime;
	public float tickWaitTime;
    


    private bool detonated;
    private bool activated;

    ////NetworkView //NetworkView;
	// Use this for initialization
	void Start () {
        ////NetworkView = GetComponent<//NetworkView>();
		StartCoroutine(CheckForPlayers(initialWaitTime, tickWaitTime));
	}

	IEnumerator CheckForPlayers(float initWait, float tickWait){
		yield return new WaitForSeconds(initWait);
        while (!activated) {
			Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);
			foreach(Collider hit in hits){
				if(hit.CompareTag("Player")){
                    if (GetComponent<StickyObject>().IsFrozen()) {
                        StartCoroutine(PopUp());
                    } else {
                        Detonate(false);
                    }
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

    IEnumerator PopUp() {
        activated = true;
        GetComponent<StickyObject>().UnStick();
        Vector3 popUpDir = GetPopUpDirection();
        float popUpDistance = 3f;
        float boomDelay = 0.25f;
        float popUpForce = popUpDistance / boomDelay;

        GetComponent<Rigidbody>().AddForce(popUpDir * popUpForce, ForceMode.Impulse);
        
        yield return new WaitForSeconds(boomDelay);
       
        Detonate(false);
    }
    /// <summary>
    /// Adds every vector with no hit together. 
    /// </summary>
    /// <returns>Should return approximately the UP direction</returns>
    Vector3 GetPopUpDirection() {
        var triplets = from I in Enumerable.Range(-1, 3)// From -1 to 1
                       from J in Enumerable.Range(-1, 3)
                       from K in Enumerable.Range(-1, 3)
                       select new { I, J, K };

        Vector3 totalDir = Vector3.zero;
        float distance = 1.0f;

        foreach (var triplet in triplets) {
            Vector3 dir = new Vector3(triplet.I, triplet.J, triplet.K);
            if (!Physics.Raycast(transform.position, dir, distance)) {// if ray hits nothing
                totalDir += dir;
                Debug.Log(dir.ToString());
            }
        }

        return totalDir.normalized;
    }

	void Detonate(bool isForced){
        if (detonated) return;
        detonated = true;
        activated = true;

        if (isForced || NetworkManager.isServer) {
           // ChatManager.DebugMessage(NetworkManager.MyPlayer().NAme + " says " + gameObject.ToString() + " goes boom.");

            SpawnExplosion(transform.position, Quaternion.identity, GetComponent<Owner>().ID);
            ////GetComponent<ObjectCleanUp>().KillMe();
        }
	}

    //[RPC]
    void SpawnExplosion(Vector3 position, Quaternion rotation, NetworkPlayer owner) {
        if (Network.isServer) {
            GameObject newObj = Network.Instantiate(explosion, position, rotation, 0) as GameObject;
            if (newObj.GetComponent<Owner>() != null) {
                newObj.GetComponent<Owner>().ID = owner;
            }

        } else {
            ////NetworkView.RPC("SpawnExplosion", RPCMode.Server, position, rotation, owner);
        }
    }

    public int Health {
        get { return 1; }
    }

    public int MaxHealth {
        get { return 1; }
    }

    public bool IsFullHealth {
        get { return true; }
    }

    public void TakeDamage(int damage, int weaponId = -1) {
        ForceDetonate();
    }

    public void RestoreHealth(int restore) {
        // Do nothing
    }
}
