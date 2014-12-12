using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIScript : MonoBehaviour {
	//
	private GameManager manager;
	
	public Texture empty;
	public Texture fullFuel;
	public Texture fullHealth;
	public Texture fullHeat;
	public Texture crosshair;
	public Texture bloodyScreen;
	
	public GUIStyle customGui;
    public Texture2D radar;
    public Texture2D playericon;
    public Texture2D enemyicon;
    public Texture2D itemicon;
    public Texture2D allyicon;
	
	private PlayerResources res;
	
	private int currentWindow = 0;
	private bool displayGameSettingsWindow = false;
	private bool displayJoinByIpWindow = false;
	private bool displayChangeKeybindWindow = false;
	
	private Rect largeRect = new Rect(Screen.width/8, Screen.height/8, Screen.width*6/8, Screen.height*6/8);
	private Rect smallRect = new Rect(Screen.width/5, Screen.height/4, Screen.width*3/5, Screen.height/2);
	
	
	private enum Menu {MainMenu, CreateGame, JoinGame, Options, Quit, Lobby, GameSettings, JoinByIP, Connecting, Keybind, ChangeKeybind}

	
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
	
	private float FOVsetting = 60f;

	private int mouseYDirection = -1;
	private bool mouseInverted = false;
	
	private bool connectionError = false;
	private bool connectingNow = false;
	
	private bool autoPickupEnabled = false;
	private int autoPickup = 0;
	
	// For Game Settings
	public string levelName;
	string[] levelList = {"FirstLevel","DerilictShipScene","SpaceStationScene"};
	public string gameMode = "DeathMatch";
	public int killsToWin = 20;
	
	private int levelSelectInt = 0;
	
	public string weapon1Name;
	public string weapon2Name;
	string[] weaponlist = {"Laser Rifle","Assault Rifle","Beam Sniper","Shotgun","Force Cannon","Rocket Launcher","Plasma Blaster"};
	string[] weaponlist2 = {"Laser Rifle","Assault Rifle","Beam Sniper","Shotgun","Force Cannon","Rocket Launcher","Plasma Blaster","None"};
	private int spawnWeapon1 = 0;
	private int spawnWeapon2 = 7;
	
	GameManager.KeyBind editedBinding;
	
	
	void Start(){
		manager = GetComponent<GameManager>();
		
		GameManager.testMode = false;
		
		currentWindow = (int) Menu.MainMenu;
		
		
		serverName = PlayerPrefs.GetString("serverName");
		playerName = PlayerPrefs.GetString("playerName", "Player");
		strPortNum = PlayerPrefs.GetString("portNumber", "25000");
		ipAddress = PlayerPrefs.GetString("ipAddress", "127.0.0.1");

		xMouseSensitivity = PlayerPrefs.GetFloat("sensitivityX", 15);
		yMouseSensitivity = PlayerPrefs.GetFloat("sensitivityY", 10);
		mouseYDirection = PlayerPrefs.GetInt("mouseYDirection", -1);
		autoPickup = PlayerPrefs.GetInt("autoPickup", 0);
		
		FOVsetting = PlayerPrefs.GetFloat("FOVsetting", 60);

		levelSelectInt = PlayerPrefs.GetInt("levelSelectInt", 0);
		levelName = levelList[levelSelectInt];
		
		weapon1Name = weaponlist[spawnWeapon1];
		weapon2Name = weaponlist2[spawnWeapon2];
		spawnWeapon1 = PlayerPrefs.GetInt("1stWeapon", 0);
		spawnWeapon2 = PlayerPrefs.GetInt("2ndWeapon", 7);

		mouseInverted = (mouseYDirection == 1);
		autoPickupEnabled = (autoPickup == 1);
	}
	
	#region OnGUI
	void OnGUI(){
		if(GameManager.testMode){
			GUI.Label(new Rect(10, 10, 100, 20), "TEST MODE");
		}

		if(connectingNow){
			if(currentWindow != (int) Menu.Connecting){
				connectingNow = false;
			}
		}
		if(GameManager.SceneIsMenu()){
			ChooseMenuWindow();

		}else if(GameManager.IsPaused()){
			GUI.Window(0, largeRect, PauseWindow, "MENU");
			
		}else if(manager.IsPlayerSpawned()){
			PlayerGUI();
			
		}else if(!GameManager.SceneIsMenu()){
			GUI.Window(1, largeRect, PauseWindow, "");
			
		}
	}
	#endregion

	#region ChooseMenuWindow
	void ChooseMenuWindow(){
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
		case (int) Menu.Connecting:
			GUI.Window ((int) Menu.Connecting, smallRect, ConnectingWindow, "");
			break;
		case (int) Menu.Keybind:
			GUI.Window ((int) Menu.Keybind, largeRect, KeyBindWindow, "Edit Keybindings");
			break;
		}
		if(displayGameSettingsWindow){
			GUI.ModalWindow((int) Menu.GameSettings, smallRect, GameSettingsWindow, "Settings");
		}
		if(displayJoinByIpWindow){
			GUI.ModalWindow((int) Menu.JoinByIP, smallRect, JoinByIpWindow, "Join By IP");
		}
		if(displayChangeKeybindWindow){
			GUI.ModalWindow((int) Menu.ChangeKeybind, smallRect, ChangeKeybindWindow, "Press new key");
		}
	}
	#endregion

	#region PlayerGUI
	void PlayerGUI(){


		GUIStyle style = new GUIStyle("label");
		style.fontSize = 50;
		style.alignment = TextAnchor.UpperCenter;

		string ammoText = res.GetCurrentClip().ToString();
		if(res.GetRemainingAmmo() > 0){
			ammoText += "/" + res.GetRemainingAmmo().ToString();
		}


		GUI.Label(new Rect(Screen.width-200, Screen.height-250, 300, 100), res.GetGrenades().ToString(), style);
		GUI.Label(new Rect(Screen.width-250, Screen.height-175, 300, 100), ammoText, style);

		//Rect combatLogRect = new Rect(20, Screen.height*3/4, Screen.width * 1/4, (Screen.height*1/4)-20);
		Rect combatLogRect = new Rect(Screen.width/2 - 250, Screen.height - 120, 500, 100);
		//temporarily moving chat box while testing radar
		GUI.Box(combatLogRect, "");
		Rect combatLogTextRect = new Rect(combatLogRect.x + 5, combatLogRect.y + 5, combatLogRect.width - 10, combatLogRect.height - 10);
		GUI.Label(combatLogTextRect, submittedChat);
		
		Rect fuel = new Rect(Screen.width-310, Screen.height-100, 300, 40);
		if(res.IsJetpackDisabled()){
			GUI.DrawTexture(fuel, fullHeat);
		}else{
			GUI.DrawTexture(fuel, empty);
		}

		fuel.xMin = fuel.xMax - (res.GetFuel() / res.GetMaxFuel())*300;
		GUI.DrawTexture(fuel, fullFuel);
		
		Rect health = new Rect(Screen.width-310, Screen.height-50, 300, 40);
		GUI.DrawTexture(health, empty);
		health.xMin = health.xMax - res.GetHealth()*3;
		GUI.DrawTexture(health, fullHealth);
		
		Rect rectCrosshair = new Rect(0, 0, 32, 32);
		rectCrosshair.center = new Vector2(Screen.width/2, Screen.height/2);
		GUI.DrawTexture(rectCrosshair, crosshair);

		//tutorial prompt
		if(tutePromptShown > 0){
			GUI.Box(new Rect(Screen.width/2 - 250, Screen.height - 120, 500, 100), tutePromptText);
			tutePromptShown--;
		}
		
		//button prompt
		if(promptShown > 0){
			GUI.Box(new Rect(Screen.width - 160, Screen.height/2, 150, 30), promptText);
			promptShown--;
		}
		
		
		//radar system
		//does everything except detect items and take player rotation into account

		//Totes making this local space now
		
		int detectionRadius = 75;
		int radarCenter = 110;
		int radarPadding = 20;
		int defaultdotsize = 30;
		int dotsizevariation = 20;
		//Vector3 mypos = new Vector3(0,0,0);
		Transform myTransform = null;
		int i = 0;
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach(GameObject player in players){
			if(player.networkView.isMine){
				myTransform = player.transform;
			} else {
				i++;
			}
		}
		float[] dotx = new float[i];
		float[] doty = new float[i];
		float[] dotsize = new float[i];
		i = 0;
		
		foreach(GameObject player in players){
			if(!player.networkView.isMine){

				Vector3 posDiff = myTransform.InverseTransformPoint(player.transform.position); // Inverse is world space -> local space
				float sqrRadius = posDiff.sqrMagnitude;

				if (sqrRadius <= detectionRadius * detectionRadius){
					dotx[i] = posDiff.x/detectionRadius * radarCenter;
					doty[i] = -posDiff.z/detectionRadius * radarCenter;
					dotsize[i] = (posDiff.y/detectionRadius * dotsizevariation) + defaultdotsize;
					i++;
				}
			}
		}
		i = 0;
		
		GUI.Box(new Rect(radarPadding, Screen.height-(radarCenter*2 + radarPadding), radarCenter*2, radarCenter*2), radar, customGui);
		
		foreach (float element in dotx){
			if(dotsize[i] < defaultdotsize){
				GUI.Box(new Rect(radarCenter+radarPadding+dotx[i] - dotsize[i]/2, Screen.height-(radarCenter+radarPadding) + doty[i] - dotsize[i]/2, dotsize[i], dotsize[i]), enemyicon, customGui);
			}
			i++;
		}
		i = 0;
		GUI.Box(new Rect(radarCenter+radarPadding-(defaultdotsize/2), Screen.height-(radarCenter+radarPadding+(defaultdotsize/2)), defaultdotsize, defaultdotsize), playericon, customGui);
		foreach (float element in dotx){
			if(dotsize[i] >= defaultdotsize){
				GUI.Box(new Rect(radarCenter+radarPadding+dotx[i] - dotsize[i]/2, Screen.height-(radarCenter+radarPadding) + doty[i] - dotsize[i]/2, dotsize[i], dotsize[i]), enemyicon, customGui);
			}
			i++;
		}		
	}
	#endregion

	#region MainMenuWindow
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
		if(GUI.Button(standard, "Tutorial")){
			StartCoroutine(LoadTutorial());
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
	#endregion
	
	//load the tutorial level
	IEnumerator LoadTutorial(){
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
			Network.InitializeServer(MAX_PLAYERS, portNum, false);
		}
		LoadLevel("Tutorial", lastLevelPrefix + 1);
		yield return new WaitForSeconds(1/5f);
		manager.Spawn();
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach(GameObject player in players){
			if(player.networkView.isMine){
				res = player.GetComponent<PlayerResources>();
			}
		}
	}

	#region CreateGameWindow
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
		if(GUI.Button(standard, "Create LAN Game")){
			
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
				Network.InitializeServer(MAX_PLAYERS, portNum, false);
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
	#endregion

	#region JoinGameWindow
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
				currentWindow = (int) Menu.Connecting;
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
	#endregion

	#region OptionsWindow
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
		
		standard.y += 40;
		mouseInverted = GUI.Toggle(new Rect(standard.x, standard.y,  100, 30), mouseInverted, "Invert Y Axis");
		autoPickupEnabled = GUI.Toggle(new Rect(standard.x+150, standard.y,  300, 30), autoPickupEnabled, "Automatically switch to new weapons");
		
		standard.y += 50;
		GUI.Label(standard, "Field Of View: ");
		FOVsetting = (float)System.Math.Round (FOVsetting, 1);
		FOVsetting = float.Parse(GUI.TextField(new Rect(300, standard.y, 50, 20), FOVsetting.ToString()));
		standard.y += 20;
		FOVsetting = GUI.HorizontalSlider(standard, FOVsetting, 50, 100);

		standard.y += 50;
		if(GUI.Button(standard, "Edit Keybinds")){
			currentWindow = (int)Menu.Keybind;
		}

		standard.y += 50;
		if(GUI.Button(standard, "Back")){
			PlayerPrefs.SetFloat("sensitivityX", xMouseSensitivity);
			PlayerPrefs.SetFloat("sensitivityY", yMouseSensitivity);

			if(mouseInverted){
				mouseYDirection = 1;
			}else{
				mouseYDirection = -1;
			}
			
			if(autoPickupEnabled){
				autoPickup = 1;
			}else{
				autoPickup = 0;
			}

			PlayerPrefs.SetInt("mouseYDirection", mouseYDirection);
			PlayerPrefs.SetInt("autoPickup", autoPickup);
			
			PlayerPrefs.SetFloat("FOVsetting", FOVsetting);
			currentWindow = (int) Menu.MainMenu;
		}
	}
	#endregion

	#region KeyBindWindow
	void KeyBindWindow(int windowId){
		Rect standard = new Rect(20, 20, -40+Screen.width/3, 30);

		//Move Forward
		standard.y += 50;
		GUI.Label(standard, "Move Forward: ");
		if(GUI.Button(new Rect(200, standard.y, 150, 20), GameManager.keyBindings[(int)GameManager.KeyBind.MoveForward].ToString())){
			editedBinding = GameManager.KeyBind.MoveForward;
			displayChangeKeybindWindow = true;
		}

		standard.y += 30;
		GUI.Label(standard, "Move Backward: ");
		if(GUI.Button(new Rect(200, standard.y, 150, 20), GameManager.keyBindings[(int)GameManager.KeyBind.MoveBack].ToString())){
			editedBinding = GameManager.KeyBind.MoveBack;
			displayChangeKeybindWindow = true;
		}

		standard.y += 30;
		GUI.Label(standard, "Move Left: ");
		if(GUI.Button(new Rect(200, standard.y, 150, 20), GameManager.keyBindings[(int)GameManager.KeyBind.MoveLeft].ToString())){
			editedBinding = GameManager.KeyBind.MoveLeft;
			displayChangeKeybindWindow = true;
		}

		standard.y += 30;
		GUI.Label(standard, "Move Right: ");
		if(GUI.Button(new Rect(200, standard.y, 150, 20), GameManager.keyBindings[(int)GameManager.KeyBind.MoveRight].ToString())){
			editedBinding = GameManager.KeyBind.MoveRight;
			displayChangeKeybindWindow = true;
		}

		standard.y += 30;
		GUI.Label(standard, "Roll Left: ");
		if(GUI.Button(new Rect(200, standard.y, 150, 20), GameManager.keyBindings[(int)GameManager.KeyBind.RollLeft].ToString())){
			editedBinding = GameManager.KeyBind.RollLeft;
			displayChangeKeybindWindow = true;
		}

		standard.y += 30;
		GUI.Label(standard, "Roll Right: ");
		if(GUI.Button(new Rect(200, standard.y, 150, 20), GameManager.keyBindings[(int)GameManager.KeyBind.RollRight].ToString())){
			editedBinding = GameManager.KeyBind.RollRight;
			displayChangeKeybindWindow = true;
		}
		standard.y += 30;
		GUI.Label(standard, "Jump / Jetpack Up: ");
		if(GUI.Button(new Rect(200, standard.y, 150, 20), GameManager.keyBindings[(int)GameManager.KeyBind.JetUp].ToString())){
			editedBinding = GameManager.KeyBind.JetUp;
			displayChangeKeybindWindow = true;
		}
		standard.y += 30;
		GUI.Label(standard, "Jetpack Down: ");
		if(GUI.Button(new Rect(200, standard.y, 150, 20), GameManager.keyBindings[(int)GameManager.KeyBind.JetDown].ToString())){
			editedBinding = GameManager.KeyBind.JetDown;
			displayChangeKeybindWindow = true;
		}

		standard.y += 50;
		GUI.Label(standard, "Reload: ");
		if(GUI.Button(new Rect(200, standard.y, 150, 20), GameManager.keyBindings[(int)GameManager.KeyBind.Reload].ToString())){
			editedBinding = GameManager.KeyBind.Reload;
			displayChangeKeybindWindow = true;
		}

		standard.y += 30;
		GUI.Label(standard, "Grenade: ");
		if(GUI.Button(new Rect(200, standard.y, 150, 20), GameManager.keyBindings[(int)GameManager.KeyBind.Grenade].ToString())){
			editedBinding = GameManager.KeyBind.Grenade;
			displayChangeKeybindWindow = true;
		}

		standard.y += 30;
		GUI.Label(standard, "Interact: ");
		if(GUI.Button(new Rect(200, standard.y, 150, 20), GameManager.keyBindings[(int)GameManager.KeyBind.Interact].ToString())){
			editedBinding = GameManager.KeyBind.Interact;
			displayChangeKeybindWindow = true;
		}

		standard.y += 50;
		if(GUI.Button(standard, "Back")){
			// Save Configuation
			PlayerPrefs.SetInt("bindMoveForward", 	(int) GameManager.keyBindings[ (int) GameManager.KeyBind.MoveForward] );
			PlayerPrefs.SetInt("bindMoveBack", 		(int) GameManager.keyBindings[ (int) GameManager.KeyBind.MoveBack] );
			PlayerPrefs.SetInt("bindMoveLeft", 		(int) GameManager.keyBindings[ (int) GameManager.KeyBind.MoveLeft] );
			PlayerPrefs.SetInt("bindMoveRight", 	(int) GameManager.keyBindings[ (int) GameManager.KeyBind.MoveRight] );
			
			
			PlayerPrefs.SetInt("bindRollLeft", 		(int) GameManager.keyBindings[ (int) GameManager.KeyBind.RollLeft] );
			PlayerPrefs.SetInt("bindRollRight", 	(int) GameManager.keyBindings[ (int) GameManager.KeyBind.RollRight] );
			PlayerPrefs.SetInt("bindJetUp", 		(int) GameManager.keyBindings[ (int) GameManager.KeyBind.JetUp] );
			PlayerPrefs.SetInt("bindJetDown", 		(int) GameManager.keyBindings[ (int) GameManager.KeyBind.JetDown] );

			PlayerPrefs.SetInt("bindReload", 		(int) GameManager.keyBindings[ (int) GameManager.KeyBind.Reload] );
			PlayerPrefs.SetInt("bindGrenade", 		(int) GameManager.keyBindings[ (int) GameManager.KeyBind.Grenade] );
			PlayerPrefs.SetInt("bindInteract", 		(int) GameManager.keyBindings[ (int) GameManager.KeyBind.Interact] );

			currentWindow = (int) Menu.Options;
		}
	}
	#endregion

	#region ChangeKeybindWindow
	void ChangeKeybindWindow(int windowId){
		GUI.Label(new Rect(20, 20, smallRect.width-40, 30), "Press Escape to cancel.");

		if(Event.current.isKey){
			if(Event.current.keyCode != KeyCode.Escape){
				GameManager.keyBindings[(int)editedBinding] = Event.current.keyCode;
			}
			displayChangeKeybindWindow = false;
		}else if(Event.current.shift){
			GameManager.keyBindings[(int)editedBinding] = KeyCode.LeftShift;
			displayChangeKeybindWindow = false;
		}
	}
	#endregion

	#region GameSettingsWindow
	void GameSettingsWindow(int windowId){
		GUI.Label(new Rect(20, 20, smallRect.width-40, 30), "Map");
		levelSelectInt = GUI.Toolbar(new Rect(20, 40, smallRect.width-40, 30), levelSelectInt, levelList);
		GUI.Label(new Rect(20, 80, smallRect.width-40, 30), "Starting Weapons");
		spawnWeapon1 = GUI.Toolbar(new Rect(20, 100, smallRect.width-40, 30), spawnWeapon1, weaponlist);
		spawnWeapon2 = GUI.Toolbar(new Rect(20, 140, smallRect.width-40, 30), spawnWeapon2, weaponlist2);
		if(GUI.Button(new Rect(20, smallRect.height-50, smallRect.width-40, 30), "Close")){
			PlayerPrefs.SetInt ("levelSelectInt", levelSelectInt);
			PlayerPrefs.SetInt ("1stWeapon", spawnWeapon1);
			PlayerPrefs.SetInt ("2ndWeapon", spawnWeapon2);
			levelName = levelList[levelSelectInt];
			displayGameSettingsWindow = false;
		}
	}
	#endregion
	
	#region JoinByIpWindow
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
				currentWindow = (int) Menu.Connecting;
			}
			
		}
		
		standard.y = smallRect.height-50;
		if(GUI.Button(standard, "Close")){
			displayJoinByIpWindow = false;
		}
	}
	#endregion

	#region LobbyWindow
	void LobbyWindow(int windowId){
		if(Network.isServer){
			if(GUI.Button(new Rect(20, 20, largeRect.width/3, 30), "Start Game")){
				// Start game
				networkView.RPC("LoadLevel", RPCMode.AllBuffered, levelName, lastLevelPrefix + 1);
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
			SubmitTextToChat(currentChat);
		}
		
		if(Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return){
			SubmitTextToChat(currentChat);
		}
	}
	#endregion

	#region ConnectingWindow
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
	#endregion

	#region PauseWindow
	void PauseWindow(int windowId){
		if(manager.IsPlayerSpawned()){
			string strReturnToGame = "Return to Game"; if(Application.loadedLevelName == "Tutorial") strReturnToGame = "Return to Simulation";
			if(GUI.Button(new Rect(20, 50, largeRect.width-40, 30), strReturnToGame)){
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
			string strShutdown = "Shutdown Server"; if(Application.loadedLevelName == "Tutorial") strShutdown = "Shutdown Simulation";
			if(GUI.Button(new Rect(20, largeRect.height-40, largeRect.width/3, 30), strShutdown)){
				BackToMainMenu();
			}
		}else{
			if(GUI.Button(new Rect(20, largeRect.height-40, largeRect.width/3, 30), "Disconnect")){
				BackToMainMenu();
			}
		}
		
		GUI.Box(new Rect( (largeRect.width/3) + 40, 100, (largeRect.width*2/3)-60, largeRect.height - 150), submittedChat, leftTextAlign);
		
		currentChat = GUI.TextField(new Rect( (largeRect.width/3) + 40, largeRect.height - 40 , (largeRect.width*2/3)-160, 20), currentChat);
		
		// Choo choo, all aboard the dodgy train
		if(GUI.Button(new Rect(largeRect.width-100, largeRect.height - 40, 80, 20), "Enter")){
			SubmitTextToChat(currentChat);
		}
		
		if(Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return){
			SubmitTextToChat(currentChat);
		}
	}
	#endregion

	public void SubmitTextToChat(string input){
		if(input != ""){
			string newChat = playerName+ ": "+input +"\n";
			currentChat = "";
			
			networkView.RPC("UpdateChat", RPCMode.All, newChat);
		}
	}
	
	//button prompts
	string promptText;
	public int promptShown;
	public void ButtonPrompt(string message, int buttonID){
		promptText = GameManager.keyBindings[buttonID].ToString() + "  -  " + message;
		promptShown = 10;
	}
	
	//tutorial prompts tutePromptShown
	string tutePromptText;
	public int tutePromptShown;
	public void TutorialPrompt(string message, int delay){
		tutePromptText = message;
		tutePromptShown = delay;
	}


	[RPC]
	void UpdateChat(string newChat){
		submittedChat += newChat;
	}
	
	[RPC]
	void AddPlayerToList(NetworkPlayer newPlayer, string newPlayerName){
		connectedPlayers.Add(newPlayer, newPlayerName);
	}
	
	[RPC]
	void RemovePlayerFromList(NetworkPlayer disconnectedPlayer){
		connectedPlayers.Remove(disconnectedPlayer);
	}


	int lastLevelPrefix = 0;
	bool rpcDisabled = false;

	[RPC]
	void LoadLevel(string level, int levelPrefix){

		lastLevelPrefix = levelPrefix;

		manager.playerCurrentName = playerName;

		Network.SetSendingEnabled(0, false);
		Network.isMessageQueueRunning = false;
		rpcDisabled = true;

		Network.SetLevelPrefix(levelPrefix);

		Application.LoadLevel(level);


	}

	void OnLevelWasLoaded(){
		if(rpcDisabled){
			Network.isMessageQueueRunning = true;
			Network.SetSendingEnabled(0, true);
			rpcDisabled = false;
		}
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
		if(!GameManager.SceneIsMenu()){
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

		if(Network.isClient || Network.isServer){
			Network.Disconnect();
		}
		Application.LoadLevel("MenuScene");

	}

}
