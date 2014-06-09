using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIScript : MonoBehaviour {
	//
	private GameManagerScript manager;
	
	public Texture empty;
	public Texture fullFuel;
	public Texture fullHealth;
	public Texture fullHeat;
	public Texture crosshair;
	
	private PlayerResources res;
	
	private int currentWindow = 0;
	private bool displayGameSettingsWindow = false;
	private bool displayJoinByIpWindow = false;
	
	private Rect largeRect = new Rect(Screen.width/8, Screen.height/8, Screen.width*6/8, Screen.height*6/8);
	private Rect smallRect = new Rect(Screen.width/3, Screen.height/3, Screen.width/3, Screen.height/3);
	
	
	private enum Menu {MainMenu, CreateGame, JoinGame, Options, Quit, Lobby, GameSettings, JoinByIP, connecting}
	
	private const string GAME_TYPE = "NoGravShooter";
	
	// For lobby chat
	private string submittedChat = "";
	private string currentChat = "";
	//private int numOfPlayers = 0;
	
	private const int MAX_PLAYERS = 31;
	private Dictionary<NetworkPlayer, string> connectedPlayers = new Dictionary<NetworkPlayer, string>();
	
	private string serverName = "";
	private string playerName = "";
	
	private string ipAddress = "";
	private bool useMasterServer = false;
	private HostData masterServerData;
	
	private string password = "";
	private string strPortNum = "";
	
	private float xMouseSensitivity = 15f;
	private float yMouseSensitivity = 10f;
	
	private bool connectionError = false;
	private bool connectingNow = false;
	
	// For Game Settings
	public string levelName = "TestShip";
	public string gameMode = "DeathMatch";
	public int killsToWin = 20;
	
	private int levelSelectInt = 0;
	
	
	void Start(){
		manager = GetComponent<GameManagerScript>();
		
		currentWindow = (int) Menu.MainMenu;
		
		serverName = PlayerPrefs.GetString("serverName");
		
		playerName = PlayerPrefs.GetString("playerName");
		if(playerName == "")
			playerName = "Player";
		
		strPortNum = PlayerPrefs.GetString("portNumber");
		if(strPortNum == "")
			strPortNum = "25000";
		
		ipAddress = PlayerPrefs.GetString("ipAddress");
		if(ipAddress == "")
			ipAddress = "127.0.0.1";
		
		xMouseSensitivity = PlayerPrefs.GetFloat("sensitivityX");
		if(xMouseSensitivity == 0)
			xMouseSensitivity = 15;
		
		yMouseSensitivity = PlayerPrefs.GetFloat("sensitivityY");
		if(yMouseSensitivity == 0)
			yMouseSensitivity = 10;
		
	}
	
	void OnGUI(){
		if(connectingNow){
			if(currentWindow != (int) Menu.connecting){
				connectingNow = false;
			}
		}
		if(GameManagerScript.SceneIsMenu()){
			switch(currentWindow){
				
			case (int) Menu.MainMenu:
				GUI.Window ((int) Menu.MainMenu, largeRect, MainMenuWindow, "Main Menu");
				break;
			case (int) Menu.CreateGame:
				GUI.Window ((int) Menu.CreateGame, largeRect, CreateGameWindow, "Create Game");
				break;
			case (int) Menu.JoinGame:
				GUI.Window ((int) Menu.JoinGame, largeRect, JoinGameWindow, "Join Game");
				break;
			case (int) Menu.Options:
				GUI.Window ((int) Menu.Options, largeRect, OptionsWindow, "Options");
				break;
			case (int) Menu.Lobby:
				GUI.Window ((int) Menu.Lobby, largeRect, LobbyWindow, serverName);
				break;
			case (int) Menu.connecting:
				GUI.Window ((int) Menu.connecting, smallRect, ConnectingWindow, "");
				break;
			}
			if(displayGameSettingsWindow){
				GUI.ModalWindow((int) Menu.GameSettings, smallRect, GameSettingsWindow, "Settings");
			}
			if(displayJoinByIpWindow){
				GUI.ModalWindow((int) Menu.JoinByIP, smallRect, JoinByIpWindow, "Join By IP");
			}
		}else if(manager.IsPaused()){
			GUI.Window(0, largeRect, PauseWindow, "MENU");
			
		}else if(manager.IsPlayerSpawned()){
			Rect heat = new Rect(Screen.width-310, Screen.height-150, 300, 40);
			GUI.DrawTexture(heat, empty);
			
			float temp = res.GetWeaponHeat();
			temp = Mathf.Clamp(temp, 0, 100);
			heat.xMin = heat.xMax - temp*3;
			
			GUI.DrawTexture(heat, fullHeat);
			
			Rect fuel = new Rect(Screen.width-310, Screen.height-100, 300, 40);
			GUI.DrawTexture(fuel, empty);
			fuel.xMin = fuel.xMax - res.GetFuel()*3;
			GUI.DrawTexture(fuel, fullFuel);
			
			Rect health = new Rect(Screen.width-310, Screen.height-50, 300, 40);
			GUI.DrawTexture(health, empty);
			health.xMin = health.xMax - res.GetHealth()*3;
			GUI.DrawTexture(health, fullHealth);
			
			Rect rectCrosshair = new Rect(0, 0, 32, 32);
			rectCrosshair.center = new Vector2(Screen.width/2, Screen.height/2);
			GUI.DrawTexture(rectCrosshair, crosshair);
			
		}else if(!GameManagerScript.SceneIsMenu()){
			GUI.Window(1, largeRect, PauseWindow, "");
			
		}
	}
	
	void MainMenuWindow(int windowId){
		Rect standard = new Rect(largeRect.width/4, 20, largeRect.width/2, 30);
		
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
		Rect standard = new Rect(largeRect.width/4, 20, largeRect.width/2, 30);
		
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
				Network.InitializeServer(MAX_PLAYERS, portNum, !Network.HavePublicAddress());
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
		
		Rect rectServerName = new Rect(30, 30, largeRect.width/3, 30);
		Rect rectStatus = new Rect(largeRect.width/3, 30, largeRect.width/6, 30);
		Rect rectPlayers = new Rect(largeRect.width*3/6, 30, largeRect.width/6, 30);
		Rect rectJoinButton = new Rect(largeRect.width*4/6, 30, largeRect.width/6, 18);
		
		
		GUI.Box(new Rect(20, 20, largeRect.width-40, largeRect.height-100), "");
		
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
		
		if(GUI.Button(new Rect(20, largeRect.height-70, largeRect.width/5, 30), "Refresh")){
			MasterServer.RequestHostList(GAME_TYPE);
		}
		if(GUI.Button (new Rect(largeRect.width/4+20, largeRect.height-70, largeRect.width/5, 30), "Join By IP")){
			displayJoinByIpWindow = true;
		}
		if(GUI.Button (new Rect(largeRect.width/2+20, largeRect.height-70, largeRect.width/5, 30), "Back")){
			currentWindow = (int) Menu.MainMenu;
		}
		
	}
	
	void OptionsWindow(int windowId){
		Rect standard = new Rect(20, 20, -40+Screen.width/3, 30);
		standard.y += 50;
		GUI.Label(standard, "Mouse Sensitivity X: ");
		xMouseSensitivity = (float)System.Math.Round (xMouseSensitivity, 1);
		xMouseSensitivity = float.Parse(GUI.TextField(new Rect(300, standard.y, 50, 20), xMouseSensitivity.ToString()));
		standard.y += 20;
		xMouseSensitivity = GUI.HorizontalSlider(standard, xMouseSensitivity, 5, 25);
		
		standard.y += 50;
		GUI.Label(standard, "Mouse Sensitivity Y: ");
		yMouseSensitivity = (float)System.Math.Round (yMouseSensitivity, 1);
		yMouseSensitivity = float.Parse(GUI.TextField(new Rect(300, standard.y, 50, 20), yMouseSensitivity.ToString()));
		standard.y += 20;
		yMouseSensitivity = GUI.HorizontalSlider(standard, yMouseSensitivity, 5, 15);
		
		standard.y += 50;
		if(GUI.Button(standard, "Back")){
			PlayerPrefs.SetFloat("sensitivityX", xMouseSensitivity);
			PlayerPrefs.SetFloat("sensitivityY", yMouseSensitivity);
			
			currentWindow = (int) Menu.MainMenu;
		}
	}
	
	void GameSettingsWindow(int windowId){
		string[] levelList = {"TestShip", "TestScene"};
		levelSelectInt = GUI.Toolbar(new Rect(20, 20, smallRect.width-40, 30), levelSelectInt, levelList);
		if(GUI.Button(new Rect(20, smallRect.height-50, smallRect.width-40, 30), "Close")){
			levelName = levelList[levelSelectInt];
			displayGameSettingsWindow = false;
		}
	}
	
	void JoinByIpWindow(int windowId){
		Rect standard = new Rect(20, 20, smallRect.width-40, 30);
		
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
		
		standard.y = smallRect.height-50;
		if(GUI.Button(standard, "Close")){
			displayJoinByIpWindow = false;
		}
	}
	
	void LobbyWindow(int windowId){
		if(Network.isServer){
			if(GUI.Button(new Rect(20, 20, largeRect.width/3, 30), "Start Game")){
				// Start game
				networkView.RPC("LoadLevel", RPCMode.AllBuffered, levelName);
			}
			if(GUI.Button(new Rect(20, 60, largeRect.width/3, 30), "Settings")){
				displayGameSettingsWindow = true;
			}
		}
		
		string strPlayers = "";
		foreach(string player in connectedPlayers.Values){
			strPlayers += player + "\n";
		}
		
		GUIStyle leftTextAlign = new GUIStyle(GUI.skin.box);
		leftTextAlign.alignment = TextAnchor.UpperLeft;
		GUI.Box(new Rect(20, 100, largeRect.width/3, largeRect.height-150), strPlayers, leftTextAlign);
		
		if(Network.isServer){
			if(GUI.Button(new Rect(20, largeRect.height-40, largeRect.width/3, 30), "Shutdown Server")){
				Network.Disconnect();
			}
		}else{
			if(GUI.Button(new Rect(20, largeRect.height-40, largeRect.width/3, 30), "Disconnect")){
				Network.Disconnect();
			}
		}
		
		GUI.Box(new Rect( (largeRect.width/3) + 40, 20, (largeRect.width*2/3)-60, largeRect.height - 80), submittedChat, leftTextAlign);
		
		currentChat = GUI.TextField(new Rect( (largeRect.width/3) + 40, largeRect.height - 40 , (largeRect.width*2/3)-160, 20), currentChat);
		
		// Choo choo, all aboard the dodgy train
		if(GUI.Button(new Rect(largeRect.width-100, largeRect.height - 40, 80, 20), "Enter")){
			SubmitTextToChat();
		}
		
		if(Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return){
			SubmitTextToChat();
		}
	}
	
	void ConnectingWindow(int windowId){
		Rect standard = new Rect(smallRect.width/4, smallRect.height/4, smallRect.width/2, smallRect.height/2);
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
			
			standard.y = smallRect.height-50;
			standard.height = 30;
			
			if(GUI.Button(standard, "Back")){
				connectionError = false;
				currentWindow = (int) Menu.JoinGame;
			}
		}
	}
	
	
	void PauseWindow(int windowId){
		if(manager.IsPlayerSpawned()){
			if(GUI.Button(new Rect(20, 50, largeRect.width-40, 30), "Return to Game")){
				manager.Pause(false);
				manager.CursorVisible(false);
			}
		}else {
			if(GUI.Button(new Rect(20, 50, largeRect.width-40, 30), "Spawn")){
				manager.Spawn();
				GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
				foreach(GameObject player in players){
					if(player.networkView.isMine){
						res = player.GetComponent<PlayerResources>();
					}
				}
			}
		}

		string strPlayers = "";
		foreach(string player in connectedPlayers.Values){
			strPlayers += player + "\n";
		}
		
		GUIStyle leftTextAlign = new GUIStyle(GUI.skin.box);
		leftTextAlign.alignment = TextAnchor.UpperLeft;
		GUI.Box(new Rect(20, 100, largeRect.width/3, largeRect.height-150), strPlayers, leftTextAlign);
		
		if(Network.isServer){
			if(GUI.Button(new Rect(20, largeRect.height-40, largeRect.width/3, 30), "Shutdown Server")){
				Network.Disconnect();
			}
		}else{
			if(GUI.Button(new Rect(20, largeRect.height-40, largeRect.width/3, 30), "Disconnect")){
				Network.Disconnect();
			}
		}
		
		GUI.Box(new Rect( (largeRect.width/3) + 40, 100, (largeRect.width*2/3)-60, largeRect.height - 150), submittedChat, leftTextAlign);
		
		currentChat = GUI.TextField(new Rect( (largeRect.width/3) + 40, largeRect.height - 40 , (largeRect.width*2/3)-160, 20), currentChat);
		
		// Choo choo, all aboard the dodgy train
		if(GUI.Button(new Rect(largeRect.width-100, largeRect.height - 40, 80, 20), "Enter")){
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
		if(!GameManagerScript.SceneIsMenu()){
			Application.LoadLevel("MenuScene");
		}
		
		currentWindow = (int) Menu.MainMenu;
		
		//Reset Varibles
		//numOfPlayers = 0;
		connectedPlayers.Clear();
		submittedChat = "";
		currentChat = "";
	}
	
	void BackToMainMenu(){
		Network.Disconnect();
		Application.LoadLevel("MenuScene");
		
		Destroy(gameObject);
	}

}
