using UnityEngine;
using System.Collections;

public class PlayerIsMine : MonoBehaviour {
	
	void Awake(){
		if(!networkView.isMine){
			GetComponent<NoGravCharacterMotor>().enabled = false;

			GetComponent<MouseLook>().enabled = false;

			GetComponentInChildren<MouseLook>().enabled = false;

			GetComponent<PlayerResources>().enabled = false;

		}
	}
}
