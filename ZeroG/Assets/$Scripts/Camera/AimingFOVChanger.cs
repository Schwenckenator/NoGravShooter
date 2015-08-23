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
        
        // What is all this doing in an update loop?
        // TODO: Cache the results
	    maxFOV = SettingsManager.instance.FieldOfView;

	    GameObject[] list = GameObject.FindGameObjectsWithTag("Player");
		foreach(GameObject player in list){
            if (player.GetComponent<NetworkView>().isMine) {
				inventory = player.GetComponent<WeaponInventory>();
			}
		}

        float zoomFov = GetZoomFOV();
        if (Input.GetMouseButton(1)) {
            
            if (inventory.currentWeapon.isScoped) {
                if (myCamera.fieldOfView == zoomFov) {
                    UIPlayerHUD.ShowSniperScope(true);
                }
            } else {
                UIPlayerHUD.ShowSniperScope(false);
            }

            if (myCamera.fieldOfView > zoomFov) {
                myCamera.fieldOfView -= zoomSpeed;
            }
        } else {
            UIPlayerHUD.ShowSniperScope(false);
            if (myCamera.fieldOfView < maxFOV) {
                myCamera.fieldOfView += zoomSpeed;
            }
        }
        ClampFOV(zoomFov, maxFOV);
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
