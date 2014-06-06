using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuGUI : MonoBehaviour {
	//
	private GameManagerScript manager;

	private int currentWindow = 0;
	private bool displayGameSettingsWindow = false;
	private bool displayJoinByIpWindow = false;

	private Rect largeMenuRect = new Rect(Screen.width/8, Screen.height/8, Screen.width*6/8, Screen.height*6/8);
	private Rect smallMenuRect = new Rect(Screen.width/3, Screen.height/3, Screen.width/3, Screen.height/3);

	private enum Menu {MainMenu, CreateGame, JoinGame, Options, Quit, Lobby, GameSettings, JoinByIP, connecting}

	private int yPosBase = 20;
	private int yPosInc = 50;
	private int buttonNum = 0;

	private const string GAME_TYPE = "NoGravShooter";

	// For lobby chat
	private string submittedChat = "";
	private string currentChat = "";
	//private int numOfPlayers = 0;

	private const int MAX_PLAYERS = 32;
	private Dictionary<NetworkPlayer, string> connectedPlayers = new Dictionary<NetworkPlayer, string>();

	private string serverName = "";
	private string playerName = "";

	private string ipAddress = "";
	private bool useMasterServer = false;
	private HostData masterServerData;

	private string password = "";
	private string strPortNum = "";

	private bool connectionError = false;
	private bool connectingNow = false;

	// For Game Settings
	public string levelName = "TestShip";
	public string gameMode = "DeathMatch";
	public int killsToWin = 20;

	private int levelSelectInt = 0;


	void Start(){
		manager = GetComponent<GameManagerScript>();

		//Set Rect sizes


		currentWindow = (int) Menu.MainMenu;

		serverName = PlayerPrefs.GetString("serverName");

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
		if(connectingNow){
			if(currentWindow != (int) Menu.connecting){
				connectingNow = false;
			}
		}
		if(GameManagerScript.IsSceneMenu()){
			switch(currentWindow){

			case (int) Menu.MainMenu:
				GUI.Window ((int) Menu.MainMenu, largeMenuRect, MainMenuWindow, "Main Menu");
				break;
			case (int) Menu.CreateGame:
				GUI.Window ((int) Menu.CreateGame, largeMenuRect, CreateGameWindow, "Create Game");
				break;
			case (int) Menu.JoinGame:
				GUI.Window ((int) Menu.JoinGame, largeMenuRect, JoinGameWindow, "Join Game");
				break;
			case (int) Menu.Options:
				GUI.Window ((int) Menu.Options, largeMenuRect, OptionsWindow, "Options");
				break;
			case (int) Menu.Lobby:
				GUI.Window ((int) Menu.Lobby, largeMenuRect, LobbyWindow, serverName);
				break;
			case (int) Menu.connecting:
				GUI.Window ((int) Menu.connecting, smallMenuRect, ConnectingWindow, "");
				break;
			}
			if(displayGameSettingsWindow){
				GUI.ModalWindow((int) Menu.GameSettings, smallMenuRect, GameSettingsWindow, "Settings");
			}
			if(displayJoinByIpWindow){
				GUI.ModalWindow((int) Menu.JoinByIP, smallMenuRect, JoinByIpWindow, "Join By IP");
			}
		}
	}

	void MainMenuWindow(int windowId){
		Rect standard = new Rect(largeMenuRect.width/4, 20, largeMenuRect.width/2, 30);
			
		GUI.Label(standard, "Player Name");

		standard.y += 20;	
		playerName = GUI.TextField(standard, playerName); 

		standard.y += 50;
		if(GUI.Button(standard, "Create Game")){
			PlayerPrefs.SetString("playerName", playerName);
			//Check for preferred server name
			if(serverName == ""){
				serverName = playerName+"'s Server";
			}

			currentWindow = (int) Menu.CreateGame;
		}

		standard.y += 50;
		if(GUI.Button(standard, "Join Game")){

			MasterServer.RequestHostList(GAME_TYPE);

			PlayerPrefs.SetString("playerName", playerName);
			currentWindow = (int) Menu.JoinGame;
		}
		standard.y += 50;
		if(GUI.Button(standard, "Options")){
			currentWindow = (int) Menu.Options;
		}
		if(!Application.isWebPlayer && !Application.isEditor){
			standard.y += 50;
			if(GUI.Button(standard, "Quit")){
				Application.Quit();
			}
		}
	}

	void CreateGameWindow(int windowId){
		Rect standard = new Rect(largeMenuRect.width/4, 20, largeMenuRect.width/2, 30);

		GUI.Label(standard, "Server Name");

		standard.y += 30;	
		serverName = GUI.TextField(standard, serverName);

		standard.y += 50;		
		GUI.Label(standard, "Port Number");
		standard.y += 30;			
		strPortNum = GUI.TextField(standard, strPortNum);

		standard.y += 50;	
		GUI.Label(standard, "Password Required");
		standard.y += 30;			
		password = GUI.TextField(standard, password);

		standard.y += 50;		
		useMasterServer = GUI.Toggle(standard, useMasterServer, "Publish Server");

		standard.y += 50;
		if(GUI.Button(standard, "Create Game")){

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
				if(useMasterServer){
					MasterServer.RegisterHost(GAME_TYPE, serverName);
				}
				PlayerPrefs.SetString("serverName", serverName);
				PlayerPrefs.SetString ("portNumber", strPortNum);
				currentWindow = (int) Menu.Lobby;
			}

		}

		standard.y += 50;
		if(GUI.Button(standard, "Back")){
			currentWindow = (int) Menu.MainMenu;
		}
	}

	void JoinGameWindow(int windowId){

		Rect rectServerName = new Rect(30, 30, largeMenuRect.width/3, 30);
		Rect rectStatus = new Rect(largeMenuRect.width/3, 30, largeMenuRect.width/6, 30);
		Rect rectPlayers = new Rect(largeMenuRect.width*3/6, 30, largeMenuRect.width/6, 30);
		Rect rectJoinButton = new Rect(largeMenuRect.width*4/6, 30, largeMenuRect.width/6, 18);


		GUI.Box(new Rect(20, 20, largeMenuRect.width-40, largeMenuRect.height-100), "");

		GUI.Label(rectServerName, "Server Name");
		GUI.Label(rectStatus, "Status");
		GUI.Label(rectPlayers, "Players");

		HostData[] servers = MasterServer.PollHostList();

		for(int i=0; i< servers.Length; i++){
			rectServerName.y = 	50+(20*i);
			rectStatus.y = 		50+(20*i);
			rectPlayers.y = 	50+(20*i);
			rectJoinButton.y = 	50+(20*i);

			GUI.Label(rectServerName, servers[i].gameName);
			GUI.Label(rectStatus, servers[i].comment);
			GUI.Label(rectPlayers, servers[i].connectedPlayers+"/"+servers[i].playerLimit);
			if(GUI.Button(rectJoinButton, "Join Game")){
				masterServerData = servers[i];
				useMasterServer = true;
				currentWindow = (int) Menu.connecting;
			}
		}

		if(GUI.Button(new Rect(20, largeMenuRect.height-70, largeMenuRect.width/5, 30), "Refresh")){
			MasterServer.RequestHostList(GAME_TYPE);
		}
		if(GUI.Button (new Rect(largeMenuRect.width/4+20, largeMenuRect.height-70, largeMenuRect.width/5, 30), "Join By IP")){
			displayJoinByIpWindow = true;
		}
		if(GUI.Button (new Rect(largeMenuRect.width/2+20, largeMenuRect.height-70, largeMenuRect.width/5, 30), "Back")){
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
		levelSelectInt = GUI.Toolbar(new Rect(20, 20, smallMenuRect.width-40, 30), levelSelectInt, levelList);
		if(GUI.Button(new Rect(20, smallMenuRect.height-50, smallMenuRect.width-40, 30), "Close")){
			levelName = levelList[levelSelectInt];
			displayGameSettingsWindow = false;
		}
	}

	void JoinByIpWindow(int windowId){
		Rect standard = new Rect(20, 20, smallMenuRect.width-40, 30);
		
		GUI.Label(standard, "Server IP Address");
		standard.y += 30;		
		ipAddress = GUI.TextField(standard, ipAddress);

		
		standard.y += 50;		
		GUI.Label(standard, "Port Number");
		standard.y += 30;		
		strPortNum = GUI.TextField(standard, strPortNum);

		standard.y += 50;
		if(GUI.Button(standard, "Join Game")){
			displayJoinByIpWindow = false;

			bool error = false;
			
			try{
				int.Parse(strPortNum);
			}catch{
				error = true;
			}
			
			if(!error){
				PlayerPrefs.SetString("ipAddress", ipAddress);
				PlayerPrefs.SetString ("portNumber", strPortNum);
				useMasterServer = false;
				currentWindow = (int) Menu.connecting;
			}
			
		}

		standard.y = smallMenuRect.height-50;
		if(GUI.Button(standard, "Close")){
			displayJoinByIpWindow = false;
		}
	}

	void LobbyWindow(int windowId){
		buttonNum = 0;
		if(Network.isServer){
			if(GUI.Button(new Rect(20, 20, largeMenuRect.width/3, 30), "Start Game")){
				// Start game
				networkView.RPC("LoadLevel", RPCMode.AllBuffered, levelName);
			}
			if(GUI.Button(new Rect(20, 60, largeMenuRect.width/3, 30), "Settings")){
				displayGameSettingsWindow = true;
			}
		}

		string strPlayers = "";
		foreach(string player in connectedPlayers.Values){
			strPlayers += player + "\n";
		}

		GUIStyle leftTextAlign = new GUIStyle(GUI.skin.box);
		leftTextAlign.alignment = TextAnchor.UpperLeft;
		GUI.Box(new Rect(20, 100, largeMenuRect.width/3, largeMenuRect.height-150), strPlayers, leftTextAlign);

		if(Network.isServer){
			if(GUI.Button(new Rect(20, largeMenuRect.height-40, largeMenuRect.width/3, 30), "Shutdown Server")){
				Network.Disconnect();
			}
		}else{
			if(GUI.Button(new Rect(20, largeMenuRect.height-40, largeMenuRect.width/3, 30), "Disconnect")){
				Network.Disconnect();
			}
		}

		GUI.Box(new Rect( (largeMenuRect.width/3) + 40, 20, (largeMenuRect.width*2/3)-60, largeMenuRect.height - 80), submittedChat, leftTextAlign);

		currentChat = GUI.TextField(new Rect( (largeMenuRect.width/3) + 40, largeMenuRect.height - 40 , (largeMenuRect.width*2/3)-160, 20), currentChat);

		// Choo choo, all aboard the dodgy train
		if(GUI.Button(new Rect(largeMenuRect.width-100, largeMenuRect.height - 40, 80, 20), "Enter")){
			SubmitTextToChat();
		}

		if(Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return){
			SubmitTextToChat();
		}
	}

	void ConnectingWindow(int windowId){
		Rect standard = new Rect(smallMenuRect.width/4, smallMenuRect.height/4, smallMenuRect.width/2, smallMenuRect.height/2);
		GUIStyle style = new GUIStyle("box");
		style.alignment = TextAnchor.MiddleCenter;

		if(!connectingNow){
			connectingNow = true;
			if(useMasterServer){
				Network.Connect(masterServerData);
			}else{
				Network.Connect(ipAddress, int.Parse(strPortNum));
			}
		}
		if(!connectionError){
			GUI.Box (standard, "Connecting....", style);
		}else{
			GUI.Box (standard, "An error occured.", style);

			standard.y = smallMenuRect.height-50;
			standard.height = 30;

			if(GUI.Button(standard, "Back")){
				connectionError = false;
				currentWindow = (int) Menu.JoinGame;
			}
		}
	}



	void SubmitTextToChat(){
		if(currentChat != ""){
			string newChat = playerName+ ": "+currentChat +"\n";
			currentChat = "";

			networkView.RPC("UpdateChat", RPCMode.All, newChat);
		}
	}

	[RPC]
	void UpdateChat(string newChat){
		submittedChat += newChat;
	}

	[RPC]
	void AddPlayerToList(NetworkPlayer newPlayer, string newPlayerName){
		connectedPlayers.Add(newPlayer, newPlayerName);
		//numOfPlayers = connectedPlayers.Count;
	}

	[RPC]
	void RemovePlayerFromList(NetworkPlayer disconnectedPlayer){
		connectedPlayers.Remove(disconnectedPlayer);
		//numOfPlayers = connectedPlayers.Count;
	}

	[RPC]
	void LoadLevel(string level){
		Application.LoadLevel(level);
	}

	[RPC]
	void ChangeServerName(string name){
		serverName = name;
	}

	
	void OnServerInitialized(){
		networkView.RPC ("AddPlayerToList", RPCMode.AllBuffered, Network.player, playerName);
		networkView.RPC ("ChangeServerName", RPCMode.OthersBuffered, serverName);
	}
	void OnConnectedToServer(){
		currentWindow = (int) Menu.Lobby;
		networkView.RPC ("AddPlayerToList", RPCMode.AllBuffered, Network.player, playerName);
	}

	void OnFailedToConnect(){
		connectionError = true;
	}

	void OnPlayerDisconnected(NetworkPlayer disconnectedPlayer){
		networkView.RPC ("RemovePlayerFromList", RPCMode.AllBuffered, disconnectedPlayer);
	}
	void OnDisconnectedFromServer(){

		manager.CursorVisible(true);

		currentWindow = (int) Menu.MainMenu;

		//Reset Varibles
		//numOfPlayers = 0;
		connectedPlayers.Clear();
		submittedChat = "";
		currentChat = "";
	}
}
