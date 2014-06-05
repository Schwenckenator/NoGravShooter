using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {
	private Transform myPlayer;

	// Use this for initialization
	void Start () {
		GameStart();
	}


	void GameStart(){
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach(GameObject player in players){
			Debug.Log ("Checking player");
			if(player.networkView.isMine){
				myPlayer = player.transform.GetChild(0);
				MoveCamera();
			}
		}
	}
	// Update is called once per frame
	void MoveCamera(){

		transform.position = myPlayer.position;
		transform.rotation = myPlayer.rotation;

		transform.parent = myPlayer;
	}
}
