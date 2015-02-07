using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {
	private Transform myPlayer;

	public void PlayerSpawned(){
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach(GameObject player in players){
			if(player.networkView.isMine){
				myPlayer = player.transform.GetChild(0);
				AttachCamera();
			}
		}
	}

	// Update is called once per frame
	void AttachCamera(){

		transform.position = myPlayer.position;
		transform.rotation = myPlayer.rotation;

		transform.parent = myPlayer;
	}

	public void DetachCamera(){
		transform.parent = null;
		transform.position = new Vector3(0, 30, 0);
	}
}
