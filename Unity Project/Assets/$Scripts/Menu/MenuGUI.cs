using UnityEngine;
using System.Collections;

public class MenuGUI : MonoBehaviour {
	//
	private GameManagerScript manager;

	private int currentWindow = 0;
	private bool displayGameSettingsWindow = false;

	//private Rect winRect = new Rect(Screen.width/3, Screen.height/3, Screen.width/3, Screen.height/3);
	private Rect rectMainMenu = new Rect(Screen.width/3, Screen.height/3, Screen.width/3, Screen.height/3);
	private Rect rectCreateGame = new Rect(Screen.width/8, Screen.height/8, Screen.width*6/8, Screen.height*6/8);
	private Rect rectJoinGame = new Rect(Screen.width/8, Screen.height/8, Screen.width*6/8, Screen.height*6/8);
	private Rect rectOptions = new Rect(Screen.width/3, Screen.height/3, Screen.width/3, Screen.height/3);
	private Rect rectLobby = new Rect(Screen.width/8, Screen.height/8, Screen.width*6/8, Screen.height*6/8);
	private Rect rectGameSettings = new Rect(Screen.width/3, Screen.height/3, Screen.width/3, Screen.height/3);

	private enum Menu {MainMenu, CreateGame, JoinGame, Options, Quit, Lobby, GameSettings}

	private int yPosBase = 20;
	private int yPosInc = 50;
	private int buttonNum = 0;

	private string strPortNum = "";

	// For lobby chat
	private string submittedChat = "";
	private string currentChat = "";
	private int numOfPlayers = 0;

	private const int MAX_PLAYERS = 32;
	private string[] connectedPlayers = new string[MAX_PLAYERS];

	private string serverName = "";
	private string playerName = "";
	private string ipAddress = "";

	// For Game Settings
	public string levelName = "TestShip";
	public string gameMode = "DeathMatch";
	public int killsToWin = 20;

	private int levelSelectInt = 0;


	void Start(){
		manager = GetComponent<GameManagerScript>();

		currentWindow = (int) Menu.MainMenu;

		serverName = PlayerPrefs.GetString("serverName");
		if(serverName == ""){
			serverName = "Server";
		}

		playerName = PlayerPrefs.GetString("playerName");
		if(playerName == ""){
			playerName = "Player";
		}

		strPortNum = PlayerPrefs.GetString("portNumber");
		if(strPortNum == ""){
			strPortNum = "25000";
		}

		ipAddress = PlayerPrefs.GetString("ipAddress");
		if(ipAddress == ""){
			ipAddress = "127.0.0.1";
		}

	}

	void OnGUI(){

		if(GameManagerScript.IsSceneMenu()){
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
			if(displayGameSettingsWindow){
				rectGameSettings =GUI.ModalWindow((int) Menu.GameSettings, rectGameSettings, GameSettingsWindow, "Settings");
			}
		}

		if(GUI.changed){
			Debug.Log (currentWindow.ToString());
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


		currentY += 20;		GUI.Label(new Rect((rectCreateGame.width/4), currentY, (rectCreateGame.width/2), 30), "Server Name");
		currentY += 30;		serverName = GUI.TextField(new Rect((rectCreateGame.width/4), currentY, (rectCreateGame.width/2), 30), serverName);


		currentY += 50;		GUI.Label(new Rect((rectCreateGame.width/4), currentY, (rectCreateGame.width/2), 30), "Player Name");
		currentY += 30;		playerName = GUI.TextField(new Rect((rectCreateGame.width/4), currentY, (rectCreateGame.width/2), 30), playerName);

		currentY += 50;		GUI.Label(new Rect((rectCreateGame.width/4), currentY, (rectCreateGame.width/2), 30), "Port Number");
		currentY += 30;		strPortNum = GUI.TextField(new Rect((rectCreateGame.width/4), currentY, (rectCreateGame.width/2), 30), strPortNum);

		currentY += 50;
		if(GUI.Button(new Rect((rectCreateGame.width/4), currentY, (rectCreateGame.width/2), 30), "Create Game")){
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
				PlayerPrefs.SetString("serverName", serverName);
				PlayerPrefs.SetString("playerName", playerName);
				PlayerPrefs.SetString ("portNumber", strPortNum);
				currentWindow = (int) Menu.Lobby;
			}

		}

		currentY += 50;
		if(GUI.Button(new Rect((rectCreateGame.width/4), currentY, (rectCreateGame.width/2), 30), "Back")){
			currentWindow = (int) Menu.MainMenu;
		}
	}

	void JoinGameWindow(int windowId){
		buttonNum = 0; // Ordering made easy!
		int currentY = 0;
		
		
		currentY += 20;		GUI.Label(new Rect((rectJoinGame.width/4), currentY, (rectJoinGame.width/2), 30), "Server IP Address");
		currentY += 30;		ipAddress = GUI.TextField(new Rect((rectJoinGame.width/4), currentY, (rectJoinGame.width/2), 30), ipAddress);

		
		currentY += 50;		GUI.Label(new Rect((rectJoinGame.width/4), currentY, (rectJoinGame.width/2), 30), "Port Number");
		currentY += 30;		strPortNum = GUI.TextField(new Rect((rectJoinGame.width/4), currentY, (rectJoinGame.width/2), 30), strPortNum);
		
		currentY += 50;		GUI.Label(new Rect((rectJoinGame.width/4), currentY, (rectJoinGame.width/2), 30), "Player Name");
		currentY += 30;		playerName = GUI.TextField(new Rect((rectJoinGame.width/4), currentY, (rectJoinGame.width/2), 30), playerName);
		
		currentY += 50;
		if(GUI.Button(new Rect((rectJoinGame.width/4), currentY, (rectJoinGame.width/2), 30), "Join Game")){
			bool error = false;
			int portNum = 0;
			
			try{
				portNum = int.Parse(strPortNum);
			}catch{
				error = true;
			}
			
			if(!error){
				Network.Connect(ipAddress, portNum);

				PlayerPrefs.SetString("ipAddress", ipAddress);
				PlayerPrefs.SetString("playerName", playerName);
				PlayerPrefs.SetString ("portNumber", strPortNum);

				currentWindow = (int) Menu.Lobby;
			}
			
		}
		
		currentY += 50;
		if(GUI.Button(new Rect((rectJoinGame.width/4), currentY, (rectJoinGame.width/2), 30), "Back")){
			currentWindow = (int) Menu.MainMenu;
		}
	}

	void OptionsWindow(int windowId){
		buttonNum = 0;
		if(GUI.Button(new Rect(20, yPosBase+(yPosInc*buttonNum++), -40+Screen.width/3, 30), "Back")){
			currentWindow = (int) Menu.MainMenu;
		}
	}

	void GameSettingsWindow(int windowId){
		string[] levelList = {"TestShip", "TestScene"};
		levelSelectInt = GUI.Toolbar(new Rect(20, 20, rectGameSettings.width-40, 30), levelSelectInt, levelList);
		if(GUI.Button(new Rect(20, rectGameSettings.height-50, rectGameSettings.width-40, 30), "Close")){
			levelName = levelList[levelSelectInt];
			displayGameSettingsWindow = false;
		}
	}

	void LobbyWindow(int windowId){
		buttonNum = 0;
		if(Network.isServer){
			if(GUI.Button(new Rect(20, 20, rectLobby.width/3, 30), "Start Game")){
				// Start game
				networkView.RPC("LoadLevel", RPCMode.AllBuffered, levelName);
			}
			if(GUI.Button(new Rect(20, 60, rectLobby.width/3, 30), "Settings")){
				displayGameSettingsWindow = true;
			}
		}

		string strPlayers = "";
		foreach(string player in connectedPlayers){
			strPlayers += player + "\n";
		}

		GUIStyle leftTextAlign = new GUIStyle(GUI.skin.box);
		leftTextAlign.alignment = TextAnchor.UpperLeft;
		GUI.Box(new Rect(20, 100, rectLobby.width/3, rectLobby.height-150), strPlayers, leftTextAlign);

		if(Network.isServer){
			if(GUI.Button(new Rect(20, rectLobby.height-40, rectLobby.width/3, 30), "Shutdown Server")){
				Network.Disconnect();
			}
		}else{
			if(GUI.Button(new Rect(20, rectLobby.height-40, rectLobby.width/3, 30), "Disconnect")){
				Network.Disconnect();
			}
		}

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
		if(currentChat != ""){
			string newChat = playerName+ ": "+currentChat +"\n";
			currentChat = "";

			networkView.RPC("UpdateChat", RPCMode.AllBuffered, newChat);
		}
	}

	[RPC]
	void UpdateChat(string newChat){
		submittedChat += newChat;
	}

	[RPC]
	void UpdateConnectedPlayers(string newPlayers, NetworkPlayer netPlayer){
		connectedPlayers[int.Parse(netPlayer.ToString())] = newPlayers;
		numOfPlayers++;
	}

	[RPC]
	void LoadLevel(string level){
		Application.LoadLevel(level);
	}

	
	void OnServerInitialized(){
		networkView.RPC ("UpdateConnectedPlayers", RPCMode.AllBuffered, playerName, Network.player);
	}
	void OnConnectedToServer(){
		networkView.RPC ("UpdateConnectedPlayers", RPCMode.AllBuffered, playerName, Network.player);
	}

	void OnDisconnectedFromServer(){

		manager.CursorVisible(true);

		currentWindow = (int) Menu.MainMenu;
		numOfPlayers = 0;
		for(int i=0; i<MAX_PLAYERS; i++){
			connectedPlayers[i] = "";
		}
		submittedChat = "";
		currentChat = "";
	}

	void OnPlayerDisconnected(NetworkPlayer netPlayer){

		networkView.RPC("UpdateConnectedPlayers", RPCMode.AllBuffered, "", netPlayer);
	}
}
