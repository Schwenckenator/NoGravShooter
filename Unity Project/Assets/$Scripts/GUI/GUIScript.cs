using UnityEngine;
using System.Collections;

public class GUIScript : MonoBehaviour {
	public Texture empty;
	public Texture full;

	private PlayerResources res;

	private GameManagerScript manager;

	private Rect smallRect = new Rect(Screen.width/4, Screen.height/4, Screen.width/2, Screen.height/2);
	private Rect largeRect = new Rect(Screen.width/8, Screen.height/8, Screen.width*6/8, Screen.height*6/8);

	void Start(){
		manager = GetComponent<GameManagerScript>();
	}

	void OnGUI(){
		if(manager.IsPaused()){
			GUI.Window(0, smallRect, PauseWindow, "MENU");

		}else if(manager.IsPlayerSpawned()){
			GUI.DrawTexture(new Rect(10, 10, 300, 40), empty);
			GUI.DrawTexture(new Rect(10, 10, res.GetFuel()*3, 40), full);

		}else if(!GameManagerScript.SceneIsMenu()){
			GUI.Window(1, largeRect, SpawnWindow, "");

		}

	}
	void PauseWindow(int windowId){
		Rect standard = new Rect(20, 50, smallRect.width-40, 30);

		standard.y = 50;
		if(GUI.Button(standard, "Return to Game")){
			manager.Pause(false);
			manager.CursorVisible(false);
		}
		standard.y = 100;
		if(Network.isServer){
			if(GUI.Button (standard, "Shutdown Server")){
				BackToMainMenu();
			}
		}else{
			if(GUI.Button (standard, "Disconnect")){
				BackToMainMenu();
			}
		}
	}

	void SpawnWindow(int windowId){
		if(GUI.Button(new Rect(20, 50, smallRect.width-40, 30), "Spawn")){
			manager.Spawn();
			res = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerResources>();
		}

//		buttonNum = 0;
//		if(Network.isServer){
//			if(GUI.Button(new Rect(20, 20, largeMenuRect.width/3, 30), "Start Game")){
//				// Start game
//				networkView.RPC("LoadLevel", RPCMode.AllBuffered, levelName);
//			}
//			if(GUI.Button(new Rect(20, 60, largeMenuRect.width/3, 30), "Settings")){
//				displayGameSettingsWindow = true;
//			}
//		}
//		
//		string strPlayers = "";
//		foreach(string player in connectedPlayers.Values){
//			strPlayers += player + "\n";
//		}
//		
//		GUIStyle leftTextAlign = new GUIStyle(GUI.skin.box);
//		leftTextAlign.alignment = TextAnchor.UpperLeft;
//		GUI.Box(new Rect(20, 100, largeMenuRect.width/3, largeMenuRect.height-150), strPlayers, leftTextAlign);
//		
//		if(Network.isServer){
//			if(GUI.Button(new Rect(20, largeMenuRect.height-40, largeMenuRect.width/3, 30), "Shutdown Server")){
//				Network.Disconnect();
//			}
//		}else{
//			if(GUI.Button(new Rect(20, largeMenuRect.height-40, largeMenuRect.width/3, 30), "Disconnect")){
//				Network.Disconnect();
//			}
//		}
//		
//		GUI.Box(new Rect( (largeMenuRect.width/3) + 40, 20, (largeMenuRect.width*2/3)-60, largeMenuRect.height - 80), submittedChat, leftTextAlign);
//		
//		currentChat = GUI.TextField(new Rect( (largeMenuRect.width/3) + 40, largeMenuRect.height - 40 , (largeMenuRect.width*2/3)-160, 20), currentChat);
//		
//		// Choo choo, all aboard the dodgy train
//		if(GUI.Button(new Rect(largeMenuRect.width-100, largeMenuRect.height - 40, 80, 20), "Enter")){
//			SubmitTextToChat();
//		}
//		
//		if(Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return){
//			SubmitTextToChat();
//		}
	}

	void BackToMainMenu(){
		Network.Disconnect();
		Application.LoadLevel("MenuScene");
		Destroy(gameObject);
	}
}
