using UnityEngine;
using System.Collections;

public class MenuGUI : MonoBehaviour {

	private int currentWindow = 0;

	//private Rect winRect = new Rect(Screen.width/3, Screen.height/3, Screen.width/3, Screen.height/3);
	private Rect rectMainMenu = new Rect(Screen.width/3, Screen.height/3, Screen.width/3, Screen.height/3);
	private Rect rectCreateGame = new Rect(Screen.width/8, Screen.height/8, Screen.width*6/8, Screen.height*6/8);
	private Rect rectJoinGame = new Rect(Screen.width/3, Screen.height/3, Screen.width/3, Screen.height/3);
	private Rect rectOptions = new Rect(Screen.width/3, Screen.height/3, Screen.width/3, Screen.height/3);
	private Rect rectLobby = new Rect(Screen.width/8, Screen.height/8, Screen.width*6/8, Screen.height*6/8);

	private enum Menu {MainMenu, CreateGame, JoinGame, Options, Quit, Lobby}

	private int yPosBase = 20;
	private int yPosInc = 50;
	private int buttonNum = 0;

	private string strPortNum = "25000";

	// For lobby chat
	private string submittedChat = "";
	private string currentChat = "";

	private string serverName = "";
	private string playerName = "";

	void Start(){
		currentWindow = (int) Menu.MainMenu;

		serverName = PlayerPrefs.GetString("serverName");
		if(serverName == ""){
			serverName = "Server";
		}
		playerName = PlayerPrefs.GetString("playerName");
		if(playerName == ""){
			playerName = "Player";
		}
	}

	void OnGUI(){


		if(Application.loadedLevelName == "MenuScene"){
			switch(currentWindow){

			case (int) Menu.MainMenu:
				rectMainMenu = GUI.Window ((int) Menu.MainMenu, rectMainMenu, MainMenuWindow, "Main Menu");
				break;
			case (int) Menu.CreateGame:
				rectCreateGame = GUI.Window ((int) Menu.CreateGame, rectCreateGame, CreateGameWindow, "Create Game");
				break;
			case (int) Menu.JoinGame:
				rectJoinGame = GUI.Window ((int) Menu.JoinGame, rectJoinGame, JoinGameWindow, "Join Game");
				break;
			case (int) Menu.Options:
				rectOptions = GUI.Window ((int) Menu.Options, rectOptions, OptionsWindow, "Options");
				break;
			case (int) Menu.Lobby:
				rectLobby = GUI.Window ((int) Menu.Lobby, rectLobby, LobbyWindow, "Lobby");
				break;
			}
		}
	}

	void MainMenuWindow(int windowId){ //
		buttonNum = 0;
		if(GUI.Button(new Rect(20, yPosBase+(yPosInc*buttonNum++), -40+Screen.width/3, 30), "Create Game")){
			currentWindow = (int) Menu.CreateGame;
		}
		if(GUI.Button(new Rect(20, yPosBase+(yPosInc*buttonNum++), -40+Screen.width/3, 30), "Join Game")){
			currentWindow = (int) Menu.JoinGame;
		}
		if(GUI.Button(new Rect(20, yPosBase+(yPosInc*buttonNum++), -40+Screen.width/3, 30), "Options")){
			currentWindow = (int) Menu.Options;
		}
		if(!Application.isWebPlayer && !Application.isEditor){
			if(GUI.Button(new Rect(20, yPosBase+(yPosInc*buttonNum++), -40+Screen.width/3, 30), "Quit")){
				Application.Quit();
			}
		}
	}

	void CreateGameWindow(int windowId){
		buttonNum = 0; // Ordering made easy!
		int currentY = 0;


		currentY += 20;		GUI.Label(new Rect(20, currentY, -40+rectCreateGame.width, 30), "Server Name");
		currentY += 30;		serverName = GUI.TextField(new Rect(20, currentY, -40+rectCreateGame.width, 30), serverName);


		currentY += 50;		GUI.Label(new Rect(20, currentY, -40+rectCreateGame.width, 30), "Player Name");
		currentY += 30;		playerName = GUI.TextField(new Rect(20, currentY, -40+rectCreateGame.width, 30), playerName);

		currentY += 50;		GUI.Label(new Rect(20, currentY, -40+rectCreateGame.width, 30), "Port Number");
		currentY += 30;		strPortNum = GUI.TextField(new Rect(20, currentY, -40+rectCreateGame.width, 30), strPortNum);

		currentY += 50;
		if(GUI.Button(new Rect(20, currentY, -40+rectCreateGame.width, 30), "Create Game")){
			//Get port number, create Server
			//Sanitise Port number input
			bool error = false;
			int portNum = 0;

			try{
				portNum = int.Parse(strPortNum);
			}catch{
				error = true;
			}

			if(!error){
				Network.InitializeServer(32, portNum, !Network.HavePublicAddress());
				currentWindow = (int) Menu.Lobby;
			}

		}

		currentY += 50;
		if(GUI.Button(new Rect(20, currentY, -40+rectCreateGame.width, 30), "Back")){
			currentWindow = (int) Menu.MainMenu;
		}
	}

	void JoinGameWindow(int windowId){
		buttonNum = 0;
		if(GUI.Button(new Rect(20, yPosBase+(yPosInc*buttonNum++), -40+Screen.width/3, 30), "Back")){
			currentWindow = (int) Menu.MainMenu;
		}
	}

	void OptionsWindow(int windowId){
		buttonNum = 0;
		if(GUI.Button(new Rect(20, yPosBase+(yPosInc*buttonNum++), -40+Screen.width/3, 30), "Back")){
			currentWindow = (int) Menu.MainMenu;
		}
	}
	void LobbyWindow(int windowId){
		buttonNum = 0;
		if(GUI.Button(new Rect(20, 20, rectLobby.width/3, 30), "Start Game")){
			// Start game
		}
		if(GUI.Button(new Rect(20, 60, rectLobby.width/3, 30), "Settings")){
			// Set map, kills to win, game mode etc etc here
			// You can change settings if you are host,
			// You can view the settings if you are client
			// FUUUUUUUUUUUUUUUUck
		}
		//Generate player List
		int numOfPlayers = Network.maxConnections;
		string connectedPlayersString = "";

		for(int i = 0; i < numOfPlayers; i++){
			connectedPlayersString = connectedPlayersString + "PlayerName" + "\n";
		}
		GUIStyle leftTextAlign = new GUIStyle(GUI.skin.box);
		leftTextAlign.alignment = TextAnchor.UpperLeft;
		GUI.Box(new Rect(20, 100, rectLobby.width/3, rectLobby.height-110), connectedPlayersString, leftTextAlign);

		GUI.Box(new Rect( (rectLobby.width/3) + 40, 20, (rectLobby.width*2/3)-60, rectLobby.height - 80), submittedChat, leftTextAlign);

		currentChat = GUI.TextField(new Rect( (rectLobby.width/3) + 40, rectLobby.height - 40 , (rectLobby.width*2/3)-160, 20), currentChat);

		// Choo choo, all aboard the dodgy train
		if(GUI.Button(new Rect(rectLobby.width-100, rectLobby.height - 40, 80, 20), "Enter")){
			SubmitTextToChat();
		}

		if(Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return){
			SubmitTextToChat();
		}
	}

	void SubmitTextToChat(){
		submittedChat += currentChat +"\n";
		currentChat = "";
	}
}
