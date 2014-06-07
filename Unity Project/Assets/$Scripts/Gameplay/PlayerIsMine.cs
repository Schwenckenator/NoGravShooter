using UnityEngine;
using System.Collections;

public class PlayerIsMine : MonoBehaviour {
	
	void Awake(){
		if(!networkView.isMine){
			GetComponent<NoGravCharacterMotor>().enabled = false;

			GetComponent<MouseLook>().enabled = false;

			transform.FindChild("CameraPos").GetComponent<MouseLook>().enabled = false;

			GetComponentInChildren<FireWeapon>().enabled = false;
		}
	}
}
