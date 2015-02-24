using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]

public class ObjectCleanUp : MonoBehaviour {
    
	public void KillMe(){
        ChatManager.DebugMessage("KillMe() called on " + gameObject.ToString() + ", view ID: "+ gameObject.networkView.viewID.ToString());
        GameObject.FindGameObjectWithTag("GameController").GetComponent<DestroyManager>().CleanUp(gameObject);
	}

    public void ClientKillMe() {
        ChatManager.DebugMessage("ClientKillMe() called on " + gameObject.ToString() + ", view ID: "+ gameObject.networkView.viewID.ToString());
        GameObject.FindGameObjectWithTag("GameController").GetComponent<DestroyManager>().Destroy(gameObject.networkView.viewID);
    }
}
