using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]

public class ObjectCleanUp : MonoBehaviour {
    
	public void KillMe(){
        GameObject.FindGameObjectWithTag("GameController").GetComponent<DestroyManager>().CleanUp(gameObject);
	}

    public void ClientKillMe() {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<DestroyManager>().Destroy(gameObject.networkView.viewID);
    }
}
