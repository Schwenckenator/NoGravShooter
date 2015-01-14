using UnityEngine;
using System.Collections;

public class FloatNameManager : MonoBehaviour {

	GameManager manager;


	void Start(){
		if(networkView.isMine){
			manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
			networkView.RPC("SetName", RPCMode.AllBuffered, manager.CurrentPlayerName);
			StartCoroutine(SelfNameDelete());
		}
	}

	IEnumerator SelfNameDelete(){
		yield return new WaitForSeconds(0.5f);
		transform.FindChild("NameText").GetComponent<TextMesh>().text = "";
	}

	void Update(){
		if(networkView.isMine){
			return;
		}
		// This code now messes with the floating text names of other players
		// We will now set the rotation of the text toward the current player.
		transform.FindChild("NameText").transform.LookAt(Camera.main.transform, Camera.main.transform.up);
	}

	[RPC]
	void SetName(string newName){
		transform.FindChild("NameText").GetComponent<TextMesh>().text = newName;
	}
}
