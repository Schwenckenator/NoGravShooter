using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]

public class ObjectCleanUp : MonoBehaviour {


	public void KillMe(){
		networkView.RPC("KillThisObject", RPCMode.AllBuffered);
	}

	[RPC]
	void KillThisObject(){
		Debug.Log (gameObject.ToString() + " is being killed.");
		Destroy (gameObject);
	}
}
