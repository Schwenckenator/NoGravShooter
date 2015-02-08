using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]

public class ObjectCleanUp : MonoBehaviour {
    bool debug = true;

	public void KillMe(){
        if (debug) DebugMessagePrint("I called KillMe() on " + gameObject.ToString());

		networkView.RPC("KillThisObject", RPCMode.AllBuffered);
	}
    public void ServerKillMe() {
        if (debug) DebugMessagePrint("Server must kill " + gameObject.ToString());

        if (Network.isServer) {
            if (debug) DebugMessagePrint("I am server, will kill " + gameObject.ToString());

            KillMe();
        }
    }

	[RPC]
	void KillThisObject(){
        
        if (debug) DebugMessagePrint(gameObject.ToString() + " is being killed.");
        
        Network.RemoveRPCs(networkView.viewID);
		Destroy (gameObject);
	}


    void DebugMessagePrint(string message) {
        Debug.Log(message);
        ChatManager.SubmitTextToLocalChat(message);
    }
}
