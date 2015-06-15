using UnityEngine;
using System.Collections;

public class PlayerIsMine : MonoBehaviour {
	
	void Awake(){
		if(!GetComponent<NetworkView>().isMine){
			GetComponent<NoGravCharacterMotor>().enabled = false;
			GetComponent<MouseLook>().enabled = false;
			transform.FindChild("CameraPos").GetComponent<MouseLook>().enabled = false;
			GetComponentInChildren<FireWeapon>().enabled = false;
            GetComponent<Assassinations>().enabled = false;
            GetComponent<PlayerResources>().enabled = false;
		}
	}
}
