using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]

public class ObjectCleanUp : MonoBehaviour {
    
	public void KillMe(){
        ChatManager.DebugMessage("KillMe() called on " + gameObject.ToString() + ", view ID: "+ gameObject.GetComponent<NetworkView>().viewID.ToString());
        DestroyManager.instance.CleanUp(gameObject);
	}

    public void ClientKillMe() {
        ChatManager.DebugMessage("ClientKillMe() called on " + gameObject.ToString() + ", view ID: "+ gameObject.GetComponent<NetworkView>().viewID.ToString());
        DestroyManager.instance.Destroy(gameObject.GetComponent<NetworkView>().viewID);
    }
}
