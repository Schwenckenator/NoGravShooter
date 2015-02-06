using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]

public class ObjectCleanUp : MonoBehaviour {

	public void KillMe(){
        if (DebugManager.IsDebugMode()) ChatManager.DebugMessagePrint("I called KillMe() on " + gameObject.ToString());

        GameObject.FindGameObjectWithTag("GameController").GetComponent<NetworkManager>().Destroy(networkView.viewID);
	}


}
