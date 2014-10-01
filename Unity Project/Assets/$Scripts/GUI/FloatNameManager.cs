using UnityEngine;
using System.Collections;

public class FloatNameManager : MonoBehaviour {

	GameManager manager;


	void Start(){
		if(networkView.isMine){
			manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
			networkView.RPC("SetName", RPCMode.AllBuffered, manager.playerCurrentName);
		}
	}

	void Update(){
//		if(networkView.isMine){
//			return;
//		}
		// This code now messes with the floating text names of other players
		// We will now set the rotation of the text toward the current player.
		transform.FindChild("NameText").transform.LookAt(Camera.main.transform, Camera.main.transform.up);
	}

	[RPC]
	void SetName(string newName){
		TextMesh thisText = transform.FindChild("NameText").GetComponent<TextMesh>();
		thisText.text = newName;
	}
}
