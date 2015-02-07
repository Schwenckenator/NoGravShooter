using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]

public class ObjectCleanUp : MonoBehaviour {
    
	public void KillMe(){
        GameObject.FindGameObjectWithTag("GameController").GetComponent<NetworkManager>().Destroy(gameObject);
	}

    public void ClientKillMe() {
        Network.RemoveRPCs(gameObject.networkView.viewID);
        Network.Destroy(gameObject);
    }
}
