using UnityEngine;
using System.Collections;

public class GUIScript : MonoBehaviour {
	public Texture empty;
	public Texture full;

	private PlayerResources res;

	private GameManagerScript manager;
	private bool myPlayerSpawned = false;

	private Rect pauseRect = new Rect(Screen.width/4, Screen.height/4, Screen.width/2, Screen.height/2);

	void Start(){
		manager = GetComponent<GameManagerScript>();
	}
	
	void PlayerSpawned(){
		myPlayerSpawned = true;
		res = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerResources>();
	}

	void OnGUI(){
		if(manager.IsPaused()){
			pauseRect = GUI.Window(0, pauseRect, PauseWindow, "MENU");
		}else if(myPlayerSpawned){
			GUI.DrawTexture(new Rect(10, 10, 300, 40), empty);
			GUI.DrawTexture(new Rect(10, 10, res.GetFuel()*3, 40), full);
		}

	}
	void PauseWindow(int windowId){
		if(GUI.Button(new Rect(20, 50, pauseRect.width-40, 30), "Resume")){
			manager.Pause(false);
		}
		if(Network.isServer){
			if(GUI.Button (new Rect(20, 100, pauseRect.width-40, 30), "Shutdown Server")){
				BackToMainMenu();
			}
		}else{
			if(GUI.Button (new Rect(20, 100, pauseRect.width-40, 30), "Disconnect")){
				BackToMainMenu();
			}
		}
	}

	void BackToMainMenu(){
		myPlayerSpawned = false;
		Network.Disconnect();
		Application.LoadLevel("MenuScene");
		Destroy(gameObject);
	}
}
