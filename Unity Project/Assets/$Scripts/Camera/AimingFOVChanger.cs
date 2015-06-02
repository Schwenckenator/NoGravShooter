using UnityEngine;
using System.Collections;

public class AimingFOVChanger : MonoBehaviour {
    private float maxFOV;

    [SerializeField]
    private float minFOV;
    [SerializeField]
    private float zoomSpeed;
    [SerializeField]
    private float sniperFOV;
	
	private WeaponInventory inventory;

    private Camera myCamera;

    

    void Start() {
        myCamera = GetComponent<Camera>();
    }
	
	void FixedUpdate(){
	maxFOV = SettingsManager.instance.FieldOfView;
	GameObject[] list = GameObject.FindGameObjectsWithTag("Player");
		foreach(GameObject player in list){
			if(player.networkView.isMine){
				inventory = player.GetComponent<WeaponInventory>();
			}
		}
		
		if(Input.GetMouseButton(1)){
			if (inventory.IsCurrentWeapon(2)){//check if current weapon is sniper
                if (myCamera.fieldOfView > sniperFOV) {
					myCamera.fieldOfView -= zoomSpeed;
				} else {
                    UIPlayerHUD.ShowSniperScope(true);
				}
			} else {
                UIPlayerHUD.ShowSniperScope(false);
				//bugfix for zooming with sniper then changing weapons
				if(myCamera.fieldOfView < minFOV){
					myCamera.fieldOfView = minFOV;
				}
				if(myCamera.fieldOfView > minFOV){
					myCamera.fieldOfView -= zoomSpeed;
				}
			}
		} else {
            UIPlayerHUD.ShowSniperScope(false);
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
