using UnityEngine;
using System.Collections;

public class AimingFOVChanger : MonoBehaviour {

	bool flag = false;

	public float minFOV;

	void FixedUpdate(){
		if(Input.GetMouseButton(1)){
			flag = true;
			if(Camera.main.fieldOfView > minFOV){
				Camera.main.fieldOfView -= 1;
			}
		}else if(flag){
			Camera.main.fieldOfView = 60;
			flag = false;
		}
	}
}
