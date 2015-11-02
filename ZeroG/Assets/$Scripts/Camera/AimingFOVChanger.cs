using UnityEngine;
using System.Collections;

public class AimingFOVChanger : MonoBehaviour {
    private float maxFOV;

    [SerializeField]
    private float zoomSpeed;
	
	private WeaponInventory inventory;

    private Camera myCamera;

    void Start() {
        myCamera = GetComponent<Camera>();
    }
	
	void FixedUpdate(){
        if (OldUIManager.IsCurrentMenuWindow(Menu.PauseMenu)) return;
        // What is all this doing in an update loop?
        // TODO: Cache the results
	    maxFOV = SettingsManager.singleton.FieldOfView;

	    GameObject[] list = GameObject.FindGameObjectsWithTag("Player");
		//foreach(GameObject player in list){
  //          //if (player.GetComponent<NetworkView>().isMine) {
		//	//	inventory = player.GetComponent<WeaponInventory>();
		//	//}
		//}

        float zoomFov = GetZoomFOV();
        if (Input.GetMouseButton(1)) {
            ZoomIn(zoomFov);
        } else {
            ZoomOut();
        }
        ClampFOV(zoomFov, maxFOV);
	}

    private void ZoomOut() {
        UIPlayerHUD.ShowSniperScope(false);
        if (myCamera.fieldOfView < maxFOV) {
            myCamera.fieldOfView += zoomSpeed;
        }
        if (inventory != null && inventory.currentWeapon != null) {
            inventory.currentWeapon.Aim(false);
        }
    }

    private void ZoomIn(float zoomFov) {
        // Zoom in
        if (myCamera.fieldOfView > zoomFov) {
            myCamera.fieldOfView -= zoomSpeed;
        }

        if (inventory != null && inventory.currentWeapon != null) {
            inventory.currentWeapon.Aim(true);
            if (inventory.currentWeapon.isScoped) {
                if (myCamera.fieldOfView == zoomFov) {
                    UIPlayerHUD.ShowSniperScope(true);
                    return;
                }
            }
        }
        UIPlayerHUD.ShowSniperScope(false);
    }

    private float GetZoomFOV() {
        float divisor;
        if (inventory != null && inventory.currentWeapon != null) {
            divisor = inventory.currentWeapon.zoomFactor;
        } else {
            divisor = 2f; // Default
        }
         
        return maxFOV / divisor;
    }

    private void ClampFOV(float min, float max) {
        if (myCamera.fieldOfView > max) {
            myCamera.fieldOfView = max;
        } else if (myCamera.fieldOfView < min) {
            myCamera.fieldOfView = min;
        }
    }
	
	public float zoomRotationRatio(){
		return myCamera.fieldOfView/maxFOV;
	}
}
