using UnityEngine;
using System.Collections;

public class ThrowGrenade : MonoBehaviour {
	public GameObject[] grenade;
	public Transform grenadeSpawn;

	ActorGrenades actorGrenade;

	float nextThrow = 0;
	float throwDelay = 1.5f;

    NetworkView networkView;
	// Use this for initialization
	void Start () {
        networkView = GetComponent<NetworkView>();
        actorGrenade = GetComponent<ActorGrenades>();
	}

	// Update is called once per frame
	void Update () {
        if (InputConverter.GetKeyDown(KeyBind.Grenade) && !GameManager.IsPlayerMenu() && Time.time > nextThrow && networkView.isMine) {
			if(actorGrenade.CanThrowGrenade()){
                SpawnGrenade(actorGrenade.GetCurrentGrenadeType(), grenadeSpawn.position, grenadeSpawn.rotation, Network.player);
				nextThrow = Time.time + throwDelay;
			}
		}
	}

    [RPC]
    private void SpawnGrenade(int grenadeID, Vector3 position, Quaternion rotation, NetworkPlayer owner) {
        if (Network.isServer) {
            GameObject newObj = Network.Instantiate(grenade[grenadeID], position, rotation, 0) as GameObject;
            if (newObj.GetComponent<Owner>() != null) {
                newObj.GetComponent<Owner>().ID = owner;
                
            }

        } else {
            networkView.RPC("SpawnGrenade", RPCMode.Server, grenadeID, position, rotation, owner);
        }
    }
}
