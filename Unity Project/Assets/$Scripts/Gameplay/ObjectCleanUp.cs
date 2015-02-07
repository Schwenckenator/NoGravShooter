using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]

public class ObjectCleanUp : MonoBehaviour {
    
	public void KillMe(){
        KillObject();
	}

    public void KillMe(bool onlyServerCanKill) {
        if (onlyServerCanKill) {
            KillIfServer();
        } else {
            KillObject();
        }
        
    }
    private void KillIfServer() {
        if (Network.isServer) {
            KillObject();
        }
    }
    private void KillObject() {

        if (DebugManager.IsDebugMode()) ChatManager.DebugMessagePrint("I will Kill " + gameObject.ToString());

        GameObject.FindGameObjectWithTag("GameController").GetComponent<NetworkManager>().Destroy(gameObject);
    }
}
