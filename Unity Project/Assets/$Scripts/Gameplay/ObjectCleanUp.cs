using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]

public class ObjectCleanUp : MonoBehaviour {


	public void KillMe(){
        Debug.Log("I called KillMe() on" + gameObject.ToString());
		networkView.RPC("KillThisObject", RPCMode.AllBuffered);
	}

	[RPC]
	void KillThisObject(){
		Debug.Log (gameObject.ToString() + " is being killed.");
        Network.RemoveRPCs(networkView.viewID);
		Destroy (gameObject);
	}
}
