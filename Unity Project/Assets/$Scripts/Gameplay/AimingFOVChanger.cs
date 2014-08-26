using UnityEngine;
using System.Collections;

public class AimingFOVChanger : MonoBehaviour {

	public float minFOV;
	public float maxFOV;
	public float zoomSpeed;
	public float sniperFOV;
	
	private FireWeapon fireWeapon;

	void FixedUpdate(){
	GameObject[] list = GameObject.FindGameObjectsWithTag("Player");
		foreach(GameObject player in list){
			if(player.networkView.isMine){
				fireWeapon = player.GetComponent<FireWeapon>();
			}
		}
		
		if(Input.GetMouseButton(1)){
			if (fireWeapon.IsCurrentWeapon(2)){//check if current weapon is sniper
				if(Camera.main.fieldOfView > sniperFOV){
					Camera.main.fieldOfView -= zoomSpeed;
				}
			} else {
				//bugfix for zooming with sniper then changing weapons
				if(Camera.main.fieldOfView < minFOV){
					Camera.main.fieldOfView = minFOV;
				}
				if(Camera.main.fieldOfView > minFOV){
					Camera.main.fieldOfView -= zoomSpeed;
				}
			}
		} else {
			if(Camera.main.fieldOfView < maxFOV){
				Camera.main.fieldOfView += zoomSpeed;
			}
		}
	}
	
	public float zoomRotationRatio(){
		return Camera.main.fieldOfView/maxFOV;
	}
}
