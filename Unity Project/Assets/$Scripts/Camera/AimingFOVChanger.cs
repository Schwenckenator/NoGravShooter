using UnityEngine;
using System.Collections;

public class AimingFOVChanger : MonoBehaviour {
    private SettingsManager settingsManager;
    private GuiManager guiManager;
    private float maxFOV;

    [SerializeField]
    private float minFOV;
    [SerializeField]
    private float zoomSpeed;
    [SerializeField]
    private float sniperFOV;
	
	private FireWeapon fireWeapon;

    private Camera myCamera;

    

    void Start() {
        GameObject manager = GameObject.FindGameObjectWithTag("GameController");
        settingsManager = manager.GetComponent<SettingsManager>();
        guiManager = manager.GetComponent<GuiManager>();
        myCamera = GetComponent<Camera>();
    }
	
	void FixedUpdate(){
	maxFOV = settingsManager.FieldOfView;
	GameObject[] list = GameObject.FindGameObjectsWithTag("Player");
		foreach(GameObject player in list){
			if(player.networkView.isMine){
				fireWeapon = player.GetComponent<FireWeapon>();
			}
		}
		
		if(Input.GetMouseButton(1)){
			if (fireWeapon.IsCurrentWeapon(2)){//check if current weapon is sniper
                if (myCamera.fieldOfView > sniperFOV) {
					myCamera.fieldOfView -= zoomSpeed;
				} else {
					guiManager.showSniperScope = true;
				}
			} else {
				guiManager.showSniperScope = false;
				//bugfix for zooming with sniper then changing weapons
				if(myCamera.fieldOfView < minFOV){
					myCamera.fieldOfView = minFOV;
				}
				if(myCamera.fieldOfView > minFOV){
					myCamera.fieldOfView -= zoomSpeed;
				}
			}
		} else {
			guiManager.showSniperScope = false;
			//bug fix for changing field of view between games without closing
			if(myCamera.fieldOfView > maxFOV){
				myCamera.fieldOfView = maxFOV;
			}
			if(myCamera.fieldOfView < maxFOV){
				myCamera.fieldOfView += zoomSpeed;
			}
		}
	}
	
	public float zoomRotationRatio(){
		return myCamera.fieldOfView/maxFOV;
	}
}
