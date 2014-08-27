using UnityEngine;
using System.Collections;


public class ObjectCleanUp : MonoBehaviour {


	public void KillMe(){
		networkView.RPC("KillThisObject", RPCMode.All);
	}

	[RPC]
	void KillThisObject(){
		Destroy (gameObject);
	}
}
