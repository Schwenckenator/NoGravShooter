using UnityEngine;
using System.Collections;

public class AimingFOVChanger : MonoBehaviour {
    private GameManager gameManager;

    private float maxFOV;

    [SerializeField]
    private float minFOV;
    [SerializeField]
    private float zoomSpeed;
    [SerializeField]
    private float sniperFOV;
	
	private FireWeapon fireWeapon;

    private SettingsManager settingsManager;

    void Start() {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        settingsManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<SettingsManager>();
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
				if(Camera.main.fieldOfView > sniperFOV){
					Camera.main.fieldOfView -= zoomSpeed;
				} else {
					showscope = true;
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
			showscope = false;
			//bug fix for changing field of view between games without closing
			if(Camera.main.fieldOfView > maxFOV){
				Camera.main.fieldOfView = maxFOV;
			}
			if(Camera.main.fieldOfView < maxFOV){
				Camera.main.fieldOfView += zoomSpeed;
			}
		}
	}
	
	public float zoomRotationRatio(){
		return Camera.main.fieldOfView/maxFOV;
	}
	
	
	public bool showscope = false;
	void OnGUI(){
		if(!showscope) return;
		GUI.DrawTexture(new Rect(0, 0, Screen.width/2 - Screen.height/2, Screen.height), gameManager.GetComponent<GuiManager>().SniperScopeBorder);
		GUI.DrawTexture(new Rect(Screen.width/2 - Screen.height/2, 0, Screen.height, Screen.height), gameManager.GetComponent<GuiManager>().SniperScope);
		GUI.DrawTexture(new Rect(Screen.width/2 + Screen.height/2, 0, Screen.width/2 - Screen.height/2, Screen.height), gameManager.GetComponent<GuiManager>().SniperScopeBorder);
	}
}
