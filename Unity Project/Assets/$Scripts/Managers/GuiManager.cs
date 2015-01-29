using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// Manages the GUI and all GUI operations
/// </summary>
public class GuiManager : MonoBehaviour {
    #region ConstantDeclarations
   
    const int SecondsInMinute = 5;
    
    #endregion

    public enum Menu { MainMenu, CreateGame, JoinGame, Options, Quit, Lobby, GameSettings, JoinByIP, Connecting, Keybind, ChangeKeybind, GraphicsOptions }
    public void SetCurrentMenuWindow(Menu newWindow) {
        currentWindow = newWindow;
    }
    public bool IsCurrentWindow(Menu window) {
        return window == currentWindow;
    }

    #region  variable Dec
    private GameManager gameManager;
    private ChatManager chatManager;
    private ScoreVictoryManager scoreVictoryManager;
    private SettingsManager settingsManager;
	
    [SerializeField]
	private Texture empty;
    [SerializeField]
    private Texture fullFuel;
    [SerializeField]
    private Texture fullHealth;
    [SerializeField]
    private Texture fullHeat;
    [SerializeField]
    private Texture crosshair;
    
    public Texture bloodyScreen; // Needs to be public

    [SerializeField]
    private GUIStyle customGui;
    [SerializeField]
    private Texture2D radar;
    [SerializeField]
    private Texture2D playericon;
    [SerializeField]
    private Texture2D enemyicon;
    [SerializeField]
    private Texture2D itemicon;
    [SerializeField]
    private Texture2D allyicon;

    [SerializeField]
    private int detectionRadius;
    [SerializeField]
    private bool detectItems;
	
	private PlayerResources playerResource;
	
	private Menu currentWindow = 0;
	private bool displayGameSettingsWindow = false;
	private bool displayJoinByIpWindow = false;
	private bool displayChangeKeybindWindow = false;
	
	private Rect largeRect = new Rect(Screen.width/8, Screen.height/8, Screen.width*6/8, Screen.height*6/8);
	private Rect smallRect = new Rect(Screen.width/5, Screen.height/4, Screen.width*3/5, Screen.height/2);
	
	private const string GameType = "NoGravShooter";
	private const int MaxPlayers = 31;
	
	private bool useMasterServer = false;
	private HostData masterServerData;
	
    private string scoreBoardText = "";
    public void SetScoreBoardText(string value) {
        scoreBoardText = value;
    }
	
	private bool mouseInverted = false;
	
	private bool connectionError = false;
	private bool connectingNow = false;
	
	private bool autoPickupEnabled = false;
	
	
	private bool MedkitSpawning = true;
	private bool GrenadeSpawning = true;
	private bool WeaponSpawning = true;

    private int index = 0; // index for graphics resolutions
    private bool fullscreen = false;
	
	private int chatboxwidth = 500;

	string[] weaponlist = {"Laser Rifle","Assault Rifle","Beam Sniper","Shotgun","Force Cannon","Rocket Launcher","Plasma Blaster"};
	string[] weaponlist2 = {"Laser Rifle","Assault Rifle","Beam Sniper","Shotgun","Force Cannon","Rocket Launcher","Plasma Blaster","None"};

    SettingsManager.KeyBind editedBinding;

	GUIStyle upperLeftTextAlign;
	GUIStyle lowerLeftTextAlign;

    #endregion

    
    void Start(){
		gameManager = GetComponent<GameManager>();
        chatManager = GetComponent<ChatManager>();
        scoreVictoryManager = GetComponent<ScoreVictoryManager>();
        settingsManager = GetComponent<SettingsManager>();
		
        SetCurrentMenuWindow(Menu.MainMenu);

		mouseInverted = (settingsManager.MouseYDirection == 1);
        autoPickupEnabled = (settingsManager.AutoPickup == 1);
		
		MedkitSpawning = (settingsManager.MedkitCanSpawn == 1);
		GrenadeSpawning = (settingsManager.GrenadeCanSpawn == 1);
		WeaponSpawning = (settingsManager.WeaponCanSpawn == 1);


	}
	
	#region OnGUI
	void OnGUI(){
		// Sets GUI styles
		upperLeftTextAlign = new GUIStyle(GUI.skin.box);
		upperLeftTextAlign.alignment = TextAnchor.UpperLeft;

		lowerLeftTextAlign = new GUIStyle(GUI.skin.box);
		lowerLeftTextAlign.alignment = TextAnchor.LowerLeft;
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

		if(GameManager.testMode){
			GUI.Label(new Rect(10, 10, 100, 20), "TEST MODE");
		}

		if(connectingNow){
			if(!IsCurrentWindow(Menu.Connecting)){
				connectingNow = false;
			}
		}
		if(GameManager.IsMenuScene()){
			ChooseMenuWindow();

		}else if(GameManager.IsPaused()){
			GUI.Window(0, largeRect, PauseWindow, "MENU");
			
		}else if(gameManager.IsPlayerSpawned()){
			PlayerGUI();
			
		}else if(!GameManager.IsMenuScene()){
			GUI.Window(1, largeRect, PauseWindow, "");
			
		}
	}
	#endregion

	#region ChooseMenuWindow
	void ChooseMenuWindow(){
		switch(currentWindow){
			
		case Menu.MainMenu:
			GUI.Window ((int) Menu.MainMenu, largeRect, MainMenuWindow, "Main Menu");
			break;
		case Menu.CreateGame:
			GUI.Window ((int) Menu.CreateGame, largeRect, CreateGameWindow, "Create Game");
			break;
		case Menu.JoinGame:
			GUI.Window ((int) Menu.JoinGame, largeRect, JoinGameWindow, "Join Game");
			break;
		case Menu.Options:
			GUI.Window ((int) Menu.Options, largeRect, OptionsWindow, "Options");
			break;
		case Menu.Lobby:
			GUI.Window ((int) Menu.Lobby, largeRect, LobbyWindow, settingsManager.ServerName);
			break;
		case Menu.Connecting:
			GUI.Window ((int) Menu.Connecting, smallRect, ConnectingWindow, "");
			break;
		case Menu.Keybind:
			GUI.Window ((int) Menu.Keybind, largeRect, KeyBindWindow, "Edit Keybindings");
			break;
        case Menu.GraphicsOptions:
            GUI.Window ((int)Menu.GraphicsOptions, largeRect, GraphicsOptionsWindow, "Graphics Options");
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

		string ammoText = playerResource.GetCurrentClip().ToString();
		if(playerResource.GetRemainingAmmo() > 0){
			ammoText += "/" + playerResource.GetRemainingAmmo().ToString();
		}

		Rect grenadeTypeLabel = new Rect(Screen.width-300, Screen.height-250, 300, 100);
		string grenadeTypeString = "";
		switch(playerResource.GetCurrentGrenadeType()){
			case 0: // Black hole
				grenadeTypeString = "BH";
				break;
			case 1:
				grenadeTypeString = "EMP";
				break;
			case 2:
				grenadeTypeString = "Frag";
				break;
		}
		GUI.Label(grenadeTypeLabel, grenadeTypeString, style);
		GUI.Label(new Rect(Screen.width-200, Screen.height-250, 300, 100), playerResource.GetCurrentGrenadeCount().ToString(), style);
		GUI.Label(new Rect(Screen.width-250, Screen.height-175, 300, 100), ammoText, style);

		if(Screen.width - 550 < 500){
			chatboxwidth = Screen.width - 550;
		} else {
			chatboxwidth = 500;
		}
		Rect combatLogRect = new Rect(Screen.width/2 - chatboxwidth/2, Screen.height - 120, chatboxwidth, 100);
		GUI.Box(combatLogRect, ChatManager.SubmittedChat, lowerLeftTextAlign);

		
		Rect fuel = new Rect(Screen.width-260, Screen.height-100, 250, 40);
		if(playerResource.IsJetpackDisabled()){
			GUI.DrawTexture(fuel, fullHeat);
		}else{
			GUI.DrawTexture(fuel, empty);
		}

		fuel.xMin = fuel.xMax - (playerResource.GetFuel() / playerResource.GetMaxFuel())*250;
		GUI.DrawTexture(fuel, fullFuel);
		
		Rect health = new Rect(Screen.width-260, Screen.height-50, 250, 40);
		GUI.DrawTexture(health, empty);
		health.xMin = health.xMax - playerResource.GetHealth()*5/2;
		GUI.DrawTexture(health, fullHealth);
		
		Rect rectCrosshair = new Rect(0, 0, 32, 32);
		rectCrosshair.center = new Vector2(Screen.width/2, Screen.height/2);
		GUI.DrawTexture(rectCrosshair, crosshair);

		//tutorial prompt
		if(tutePromptShown > 0){
			GUI.Box(new Rect(Screen.width/2 - chatboxwidth/2, Screen.height - 120, chatboxwidth, 100), tutePromptText);
			tutePromptShown--;
		}
		
		//button prompt
		if(promptShown > 0){
			GUI.Box(new Rect(Screen.width - 160, Screen.height/2, 150, 30), promptText);
			promptShown--;
		}

        Radar();
        if (!GameManager.IsTutorialScene()) {
            TimerGUI();
            ScoreBoard();
        }
	}
    void Radar() {
        //radar system
        int radarCenter = 110;
        int radarPadding = 20;
        int radarDotArea = 95;
        int defaultdotsize = 30;
        Transform myTransform = null;
        int index = 0;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] items = GameObject.FindGameObjectsWithTag("BonusPickup");
        foreach (GameObject player in players) {
            if (player.networkView.isMine) {
                myTransform = player.transform;
            } else {
                index++;
            }
        }
        if (detectItems) {
            for(int i=0; i<items.Length; i++){
                index++;
            }
                
        }
        float[] dotx = new float[index];
        float[] doty = new float[index];
        float[] dotsize = new float[index];
        string[] dottype = new string[index];
        index = 0;

        foreach (GameObject player in players) {
            if (!player.networkView.isMine) {

                Vector3 posDiff = myTransform.InverseTransformPoint(player.transform.position); // Inverse is world space -> local space
                float sqrRadius = posDiff.sqrMagnitude;

                if (sqrRadius <= detectionRadius * detectionRadius) {
                    dotx[index] = posDiff.x / detectionRadius * radarDotArea;
                    doty[index] = -posDiff.z / detectionRadius * radarDotArea;
                    dotsize[index] = (posDiff.y / detectionRadius * defaultdotsize) + defaultdotsize;
                    //If (on same team) dottype[i] = "Ally", else "Enemy"
                    dottype[index] = "Enemy";
                    index++;
                }
            }
        }
        if (detectItems) {
            foreach (GameObject bonus in items) {
                Vector3 posDiff = myTransform.InverseTransformPoint(bonus.transform.position); // Inverse is world space -> local space
                float sqrRadius = posDiff.sqrMagnitude;

                if (sqrRadius <= detectionRadius * detectionRadius) {
                    dotx[index] = posDiff.x / detectionRadius * radarDotArea;
                    doty[index] = -posDiff.z / detectionRadius * radarDotArea;
                    dotsize[index] = (posDiff.y / detectionRadius * defaultdotsize) + defaultdotsize;
                    dottype[index] = "Item";
                    index++;
                }
            }
        }
        index = 0;

        GUI.Box(new Rect(radarPadding, Screen.height - (radarCenter * 2 + radarPadding), radarCenter * 2, radarCenter * 2), radar, customGui);

        for (int i = 0; i < dotx.Length; i++) {
            if (dotsize[i] < defaultdotsize) {
                if (dottype[i] == "Enemy") {
                    GUI.Box(new Rect(radarCenter + radarPadding + dotx[i] - dotsize[i] / 2, Screen.height - (radarCenter + radarPadding) + doty[i] - dotsize[i] / 2, dotsize[i], dotsize[i]), enemyicon, customGui);
                } else if (dottype[i] == "Item") {
                    GUI.Box(new Rect(radarCenter + radarPadding + dotx[i] - dotsize[i] / 2, Screen.height - (radarCenter + radarPadding) + doty[i] - dotsize[i] / 2, dotsize[i], dotsize[i]), itemicon, customGui);
                } else {
                    GUI.Box(new Rect(radarCenter + radarPadding + dotx[i] - dotsize[i] / 2, Screen.height - (radarCenter + radarPadding) + doty[i] - dotsize[i] / 2, dotsize[i], dotsize[i]), allyicon, customGui);
                }
            }
        }

        GUI.Box(new Rect(radarCenter + radarPadding - (defaultdotsize / 2), Screen.height - (radarCenter + radarPadding + (defaultdotsize / 2)), defaultdotsize, defaultdotsize), playericon, customGui);

        for (int i = 0; i < dotx.Length; i++) {
            if (dotsize[i] >= defaultdotsize) {
                if (dottype[i] == "Enemy") {
                    GUI.Box(new Rect(radarCenter + radarPadding + dotx[i] - dotsize[i] / 2, Screen.height - (radarCenter + radarPadding) + doty[i] - dotsize[i] / 2, dotsize[i], dotsize[i]), enemyicon, customGui);
                } else if (dottype[i] == "Item") {
                    GUI.Box(new Rect(radarCenter + radarPadding + dotx[i] - dotsize[i] / 2, Screen.height - (radarCenter + radarPadding) + doty[i] - dotsize[i] / 2, dotsize[i], dotsize[i]), itemicon, customGui);
                } else {
                    GUI.Box(new Rect(radarCenter + radarPadding + dotx[i] - dotsize[i] / 2, Screen.height - (radarCenter + radarPadding) + doty[i] - dotsize[i] / 2, dotsize[i], dotsize[i]), allyicon, customGui);
                }
            }
        }
    }
    void ScoreBoard() {
        Rect scoreBoard = new Rect(10, 10, 300, NetworkManager.connectedPlayers.Count * 20 + 20);
        GUI.Box(scoreBoard, scoreBoardText);
    }
    void TimerGUI() {

        float timeLeftMins = Mathf.Floor((gameManager.endTime - Time.time) / 60);
        float timeLeftSecs = Mathf.Floor((gameManager.endTime - Time.time) - (timeLeftMins * 60));

        if (timeLeftMins >= 0 && timeLeftSecs >= 0) {
            GUI.Box(new Rect(Screen.width / 2 - 35, 10, 70, 23), timeLeftMins.ToString("00") + ":" + timeLeftSecs.ToString("00"));
        } else {
            Debug.Log("Displayed the zero-d time area");
            GUI.Box(new Rect(Screen.width / 2 - 35, 10, 70, 23), "00:00");
        }
    }
    
	#endregion

	#region MainMenuWindow
	void MainMenuWindow(int windowId){
		Rect standard = new Rect(largeRect.width/4, 20, largeRect.width/2, 30);
		
		GUI.Label(standard, "Player Name");
		
		standard.y += 20;
        settingsManager.PlayerName = GUI.TextField(standard, settingsManager.PlayerName); 
		
		standard.y += 50;
		if(GUI.Button(standard, "Create Game")){
			//Check for preferred server name
            if (settingsManager.ServerName == "") {
                settingsManager.ServerName = settingsManager.PlayerName + "'s Server";
			}
            settingsManager.SaveSettings();

            SetCurrentMenuWindow(Menu.CreateGame);
		}
		
		standard.y += 50;
		if(GUI.Button(standard, "Join Game")){
			
			MasterServer.RequestHostList(GameType);
            settingsManager.SaveSettings();

            SetCurrentMenuWindow(Menu.JoinGame);
		}
		
		standard.y += 50;
		if(GUI.Button(standard, "Tutorial")){
			StartCoroutine(LoadTutorial());
		}
		
		standard.y += 50;
		if(GUI.Button(standard, "Options")){
            SetCurrentMenuWindow(Menu.Options);
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
		int portNum = 25000;
		int maxTutorialConnections = 1;

        NetworkManager.SetServerDetails(maxTutorialConnections, portNum, false, false);
        NetworkManager.InitialiseServer();

        settingsManager.LevelName = "Tutorial";
		gameManager.LoadLevel();

		yield return new WaitForSeconds(1/5f);
		gameManager.Spawn();
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach(GameObject player in players){
			if(player.networkView.isMine){
				playerResource = player.GetComponent<PlayerResources>();
			}
		}
	}

	#region CreateGameWindow
	void CreateGameWindow(int windowId){
		Rect standard = new Rect(largeRect.width/4, 20, largeRect.width/2, 30);
		
		GUI.Label(standard, "Server Name");
		
		standard.y += 30;
        settingsManager.ServerName = GUI.TextField(standard, settingsManager.ServerName);
		
		standard.y += 50;		
		GUI.Label(standard, "Port Number");
		standard.y += 30;
        settingsManager.PortNumStr = GUI.TextField(standard, settingsManager.PortNumStr);
		
		standard.y += 50;	
		GUI.Label(standard, "Password Required");
		standard.y += 30;
        settingsManager.Password = GUI.TextField(standard, settingsManager.Password);
		
		standard.y += 50;		
		useMasterServer = GUI.Toggle(standard, useMasterServer, "Publish Server");
		
		standard.y += 50;
		if(GUI.Button(standard, "Create Game")){
            CreateGame(false);
		}

		standard.y += 50;
        if (GUI.Button(standard, "Create LAN Game")) {
            CreateGame(true);
        }
		
		standard.y += 50;
		if(GUI.Button(standard, "Back")){
			currentWindow =  Menu.MainMenu;
		}
	}
    private void CreateGame(bool isLanGame) {
        settingsManager.ParsePortNumber();

        if (settingsManager.PortNum >= 0) { // Check for error
            NetworkManager.SetServerDetails(MaxPlayers, settingsManager.PortNum, useMasterServer, !isLanGame);
            NetworkManager.InitialiseServer();
            settingsManager.SaveSettings();
            currentWindow = Menu.Lobby;
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
				currentWindow =  Menu.Connecting;
			}
		}
		
		if(GUI.Button(new Rect(20, largeRect.height-70, largeRect.width/5, 30), "Refresh")){
			MasterServer.RequestHostList(GameType);
		}
		if(GUI.Button (new Rect(largeRect.width/4+20, largeRect.height-70, largeRect.width/5, 30), "Join By IP")){
			displayJoinByIpWindow = true;
		}
		if(GUI.Button (new Rect(largeRect.width/2+20, largeRect.height-70, largeRect.width/5, 30), "Back")){
			currentWindow =  Menu.MainMenu;
		}
		
	}

    void JoinByIpWindow(int windowId) {
        Rect standard = new Rect(20, 20, smallRect.width - 40, 30);

        GUI.Label(standard, "Server IP Address");
        standard.y += 30;
        settingsManager.IpAddress = GUI.TextField(standard, settingsManager.IpAddress);


        standard.y += 50;
        GUI.Label(standard, "Port Number");
        standard.y += 30;
        settingsManager.PortNumStr = GUI.TextField(standard, settingsManager.PortNumStr);

        standard.y += 50;
        if (GUI.Button(standard, "Join Game")) {
            displayJoinByIpWindow = false;
            settingsManager.ParsePortNumber();

            if (settingsManager.PortNum >= 0) { // Check for error
                settingsManager.SaveSettings();
                useMasterServer = false;
                currentWindow = Menu.Connecting;
            }

        }

        standard.y = smallRect.height - 50;
        if (GUI.Button(standard, "Close")) {
            displayJoinByIpWindow = false;
        }
    }
	#endregion

	#region OptionsWindow
	void OptionsWindow(int windowId){
		Rect standard = new Rect(20, 20, -40+Screen.width/3, 30);
		standard.y += 50;
		GUI.Label(standard, "Mouse Sensitivity X: ");
        settingsManager.xMouseSensitivity = (float)System.Math.Round(settingsManager.xMouseSensitivity, 2);
        settingsManager.xMouseSensitivity = float.Parse(GUI.TextField(new Rect(300, standard.y, 50, 20), settingsManager.xMouseSensitivity.ToString()));
		standard.y += 20;
        settingsManager.xMouseSensitivity = GUI.HorizontalSlider(standard, settingsManager.xMouseSensitivity, 0, 1);
		
		standard.y += 50;
		GUI.Label(standard, "Mouse Sensitivity Y: ");
        settingsManager.yMouseSensitivity = (float)System.Math.Round(settingsManager.yMouseSensitivity, 2);
        settingsManager.yMouseSensitivity = float.Parse(GUI.TextField(new Rect(300, standard.y, 50, 20), settingsManager.yMouseSensitivity.ToString()));
		standard.y += 20;
        settingsManager.yMouseSensitivity = GUI.HorizontalSlider(standard, settingsManager.yMouseSensitivity, 0, 1);
		
		standard.y += 40;
		mouseInverted = GUI.Toggle(new Rect(standard.x, standard.y,  100, 30), mouseInverted, "Invert Y Axis");
		autoPickupEnabled = GUI.Toggle(new Rect(standard.x+150, standard.y,  300, 30), autoPickupEnabled, "Automatically switch to new weapons");
		
		standard.y += 50;
		GUI.Label(standard, "Field Of View: ");
        settingsManager.FieldOfView = (float)System.Math.Round(settingsManager.FieldOfView, 1);
        settingsManager.FieldOfView = float.Parse(GUI.TextField(new Rect(300, standard.y, 50, 20), settingsManager.FieldOfView.ToString()));
		standard.y += 20;
        settingsManager.FieldOfView = GUI.HorizontalSlider(standard, settingsManager.FieldOfView, 50, 100);

		standard.y += 50;
		if(GUI.Button(standard, "Edit Keybinds")){
			currentWindow = Menu.Keybind;
		}

        if (GUI.Button(new Rect(standard.width+50, standard.y, standard.width, standard.height), "Graphics Settings")) {
            currentWindow = Menu.GraphicsOptions;
        }

		standard.y += 50;
		if(GUI.Button(standard, "Back")){

			if(mouseInverted){
				settingsManager.MouseYDirection = 1;
			}else{
                settingsManager.MouseYDirection = -1;
			}
			
			if(autoPickupEnabled){
                settingsManager.AutoPickup = 1;
			}else{
                settingsManager.AutoPickup = 0;
			}

            settingsManager.SaveSettings();
			currentWindow =  Menu.MainMenu;
		}
	}
	#endregion

    #region GraphicsOptionsWindow
    void GraphicsOptionsWindow(int windowId) {
        Rect standard = new Rect(20, 20, -40 + Screen.width / 3, 30);
        Resolution[] resolutions = Screen.resolutions;
        int maxIndex = resolutions.Length;

        standard.y += 50;
        GUI.Label(standard, "Resolution: ");
        if (GUI.Button(new Rect(100, standard.y, 100, 30), resolutions[index].width + " x " + resolutions[index].height)) {
            index++;
            index %= maxIndex;
        }
        
        standard.y += 50;
        fullscreen = GUI.Toggle(new Rect(standard.x, standard.y, 100, 30), fullscreen, "Fullscreen");

        standard.y += 50;
        if (GUI.Button(standard, "Back")) {

            Screen.SetResolution(resolutions[index].width, resolutions[index].height, fullscreen);

            currentWindow = Menu.Options;
        }
    }
    #endregion

    #region KeyBindWindow
    void KeyBindWindow(int windowId){
		Rect standard = new Rect(20, 20, -40+Screen.width/3, 30);

		//Move Forward
		standard.y += 50;
		GUI.Label(standard, "Move Forward: ");
		if(GUI.Button(new Rect(140, standard.y, 150, 20), SettingsManager.keyBindings[(int)SettingsManager.KeyBind.MoveForward].ToString())){
			editedBinding = SettingsManager.KeyBind.MoveForward;
			displayChangeKeybindWindow = true;
		}

		standard.y += 30;
		GUI.Label(standard, "Move Backward: ");
		if(GUI.Button(new Rect(140, standard.y, 150, 20), SettingsManager.keyBindings[(int)SettingsManager.KeyBind.MoveBack].ToString())){
			editedBinding = SettingsManager.KeyBind.MoveBack;
			displayChangeKeybindWindow = true;
		}

		standard.y += 30;
		GUI.Label(standard, "Move Left: ");
		if(GUI.Button(new Rect(140, standard.y, 150, 20), SettingsManager.keyBindings[(int)SettingsManager.KeyBind.MoveLeft].ToString())){
			editedBinding = SettingsManager.KeyBind.MoveLeft;
			displayChangeKeybindWindow = true;
		}

		standard.y += 30;
		GUI.Label(standard, "Move Right: ");
		if(GUI.Button(new Rect(140, standard.y, 150, 20), SettingsManager.keyBindings[(int)SettingsManager.KeyBind.MoveRight].ToString())){
			editedBinding = SettingsManager.KeyBind.MoveRight;
			displayChangeKeybindWindow = true;
		}

		standard.y += 30;
		GUI.Label(standard, "Roll Left: ");
		if(GUI.Button(new Rect(140, standard.y, 150, 20), SettingsManager.keyBindings[(int)SettingsManager.KeyBind.RollLeft].ToString())){
			editedBinding = SettingsManager.KeyBind.RollLeft;
			displayChangeKeybindWindow = true;
		}

		standard.y += 30;
		GUI.Label(standard, "Roll Right: ");
		if(GUI.Button(new Rect(140, standard.y, 150, 20), SettingsManager.keyBindings[(int)SettingsManager.KeyBind.RollRight].ToString())){
			editedBinding = SettingsManager.KeyBind.RollRight;
			displayChangeKeybindWindow = true;
		}
		standard.y += 30;
		GUI.Label(standard, "Jump / Jetpack Up: ");
		if(GUI.Button(new Rect(140, standard.y, 150, 20), SettingsManager.keyBindings[(int)SettingsManager.KeyBind.JetUp].ToString())){
			editedBinding = SettingsManager.KeyBind.JetUp;
			displayChangeKeybindWindow = true;
		}
		standard.y += 30;
		GUI.Label(standard, "Jetpack Down: ");
		if(GUI.Button(new Rect(140, standard.y, 150, 20), SettingsManager.keyBindings[(int)SettingsManager.KeyBind.JetDown].ToString())){
			editedBinding = SettingsManager.KeyBind.JetDown;
			displayChangeKeybindWindow = true;
		}

		standard.y -= 210;
		standard.x+= 300;
		GUI.Label(standard, "Reload: ");
		if(GUI.Button(new Rect(420, standard.y, 150, 20), SettingsManager.keyBindings[(int)SettingsManager.KeyBind.Reload].ToString())){
			editedBinding = SettingsManager.KeyBind.Reload;
			displayChangeKeybindWindow = true;
		}

		standard.y += 30;
		GUI.Label(standard, "Grenade: ");
		if(GUI.Button(new Rect(420, standard.y, 150, 20), SettingsManager.keyBindings[(int)SettingsManager.KeyBind.Grenade].ToString())){
			editedBinding = SettingsManager.KeyBind.Grenade;
			displayChangeKeybindWindow = true;
		}

		standard.y += 30;
		GUI.Label(standard, "Switch Grenade: ");
		if(GUI.Button(new Rect(420, standard.y, 150, 20), SettingsManager.keyBindings[(int)SettingsManager.KeyBind.GrenadeSwitch].ToString())){
			editedBinding = SettingsManager.KeyBind.GrenadeSwitch;
			displayChangeKeybindWindow = true;
		}

		standard.y += 30;
		GUI.Label(standard, "Interact: ");
		if(GUI.Button(new Rect(420, standard.y, 150, 20), SettingsManager.keyBindings[(int)SettingsManager.KeyBind.Interact].ToString())){
			editedBinding = SettingsManager.KeyBind.Interact;
			displayChangeKeybindWindow = true;
		}

		standard.y += 50;
		if(GUI.Button(standard, "Back")){
			// Save Configuation-
            settingsManager.SaveKeyBinds();

			currentWindow =  Menu.Options;
		}
	}
	#endregion

	#region ChangeKeybindWindow
	void ChangeKeybindWindow(int windowId){
		GUI.Label(new Rect(20, 20, smallRect.width-40, 30), "Press Escape to cancel.");

		if(Event.current.isKey){
			if(Event.current.keyCode != KeyCode.Escape){
				SettingsManager.keyBindings[(int)editedBinding] = Event.current.keyCode;
			}
			displayChangeKeybindWindow = false;
		}else if(Event.current.shift){
			SettingsManager.keyBindings[(int)editedBinding] = KeyCode.LeftShift;
			displayChangeKeybindWindow = false;
		}
	}
	#endregion

	#region GameSettingsWindow
	void GameSettingsWindow(int windowId){
		GUI.Label(new Rect(20, 10, 100, 30), "Game Mode");
        settingsManager.GameModeIndex = GUI.Toolbar(new Rect(20, 30, smallRect.width - 40, 30), settingsManager.GameModeIndex, settingsManager.GameModeList);

		GUI.Label(new Rect(20, 65, 100, 30), "Score Limit");
        settingsManager.ScoreToWin = int.Parse(GUI.TextField(new Rect(95, 65, 50, 20), settingsManager.ScoreToWin.ToString()));

		GUI.Label(new Rect(160, 65, 120, 30), "Time Limit (mins)");
        settingsManager.TimeLimitMin = int.Parse(GUI.TextField(new Rect(275, 65, 50, 20), settingsManager.TimeLimitMin.ToString()));

		GUI.Label(new Rect(20, 90, 90, 30), "Item Spawning");
		MedkitSpawning = GUI.Toggle(new Rect(115, 90,  80, 30), MedkitSpawning, "Medkits");
		GrenadeSpawning = GUI.Toggle(new Rect(185, 90,  80, 30), GrenadeSpawning, "Grenades");
		WeaponSpawning = GUI.Toggle(new Rect(265, 90,  80, 30), WeaponSpawning, "Weapons");
		
		GUI.Label(new Rect(20, 110, smallRect.width-40, 30), "Map");
        settingsManager.LevelIndex = GUI.Toolbar(new Rect(20, 130, smallRect.width - 40, 30), settingsManager.LevelIndex, settingsManager.LevelList);

		GUI.Label(new Rect(20, 165, smallRect.width-40, 30), "Starting Weapons");
        settingsManager.SpawnWeapon1 = GUI.Toolbar(new Rect(20, 185, smallRect.width - 40, 30), settingsManager.SpawnWeapon1, weaponlist);
        settingsManager.SpawnWeapon2 = GUI.Toolbar(new Rect(20, 220, smallRect.width - 40, 30), settingsManager.SpawnWeapon2, weaponlist2);

        if(GUI.Button(new Rect(20, smallRect.height-45, smallRect.width-40, 30), "Close")){
            settingsManager.SaveSettings();
			displayGameSettingsWindow = false;
		}
	}
	#endregion

	#region LobbyWindow
	void LobbyWindow(int windowId){
		if(Network.isServer){
			if(GUI.Button(new Rect(20, 20, largeRect.width/3, 30), "Start Game")){
                gameManager.LoadLevel();
                
			}
			if(GUI.Button(new Rect(20, 60, largeRect.width/3, 30), "Settings")){
				displayGameSettingsWindow = true;
			}
		}
		

		GUI.Box(new Rect(20, 100, largeRect.width/3, largeRect.height-150), PlayerList(), upperLeftTextAlign);
		
		if(Network.isServer){
			if(GUI.Button(new Rect(20, largeRect.height-40, largeRect.width/3, 30), "Shutdown Server")){
				NetworkManager.Disconnect();
			}
		}else{
			if(GUI.Button(new Rect(20, largeRect.height-40, largeRect.width/3, 30), "Disconnect")){
				NetworkManager.Disconnect();
			}
		}
		
		GUI.Box(new Rect( (largeRect.width/3) + 40, 20, (largeRect.width*2/3)-60, largeRect.height - 80), ChatManager.SubmittedChat, lowerLeftTextAlign);
		
		ChatManager.currentChat = GUI.TextField(new Rect( (largeRect.width/3) + 40, largeRect.height - 40 , (largeRect.width*2/3)-160, 20), ChatManager.currentChat);
		
		// Choo choo, all aboard the dodgy train
		if(GUI.Button(new Rect(largeRect.width-100, largeRect.height - 40, 80, 20), "Enter")){
			chatManager.SubmitTextToChat(ChatManager.currentChat);
			ChatManager.ClearCurrentChat();
		}
		
		if(Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return){
			chatManager.SubmitTextToChat(ChatManager.currentChat);
			ChatManager.ClearCurrentChat();
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
            NetworkManager.SetClientDetails(masterServerData, useMasterServer, settingsManager.IpAddress, int.Parse(settingsManager.PortNumStr));
            NetworkManager.ConnectToServer();
		}
		if(!connectionError){
			GUI.Box (standard, "Connecting....", style);
		}else{
			GUI.Box (standard, "An error occured.", style);
			
			standard.y = smallRect.height-50;
			standard.height = 30;
			
			if(GUI.Button(standard, "Back")){
				connectionError = false;
				currentWindow =  Menu.JoinGame;
			}
		}
	}
	#endregion

	#region PauseWindow
	void PauseWindow(int windowId){
        if (!GameManager.IsTutorialScene()) {
            TimerGUI(); // Display the timer even when paused
        }

		if(gameManager.IsPlayerSpawned()){
            string strReturnToGame = "Return to Game"; if (GameManager.IsTutorialScene()) strReturnToGame = "Return to Simulation";
			if(GUI.Button(new Rect(20, 50, largeRect.width-40, 30), strReturnToGame)){
				gameManager.Pause(false);
				gameManager.SetCursorVisibility(false);
			}
		}else {
            if (scoreVictoryManager.IsVictor) {
                GUI.enabled = false;
                GUI.Button(new Rect(20, 50, largeRect.width - 40, 30), scoreVictoryManager.VictorName + " has won!");
                GUI.enabled = true;
            } else {
                if (GUI.Button(new Rect(20, 50, largeRect.width - 40, 30), "Spawn")) {
                    gameManager.Spawn();
                    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                    foreach (GameObject player in players) {
                        if (player.networkView.isMine) {
                            playerResource = player.GetComponent<PlayerResources>();
                        }
                    }
                }
            }

		}

        GUI.Box(new Rect(20, 100, largeRect.width / 3, largeRect.height - 190), PlayerList(), upperLeftTextAlign);
		
		if(Network.isServer){
            if (GameManager.IsTutorialScene()) {
                if (GUI.Button(new Rect(20, largeRect.height - 40, largeRect.width / 3, 30), "Shutdown Simulation")) {
                    gameManager.BackToMainMenu();
                }
            } else {
                if (GUI.Button(new Rect(20, largeRect.height - 80, largeRect.width / 3, 30), "Return to Lobby")) {
                    gameManager.ReturnToLobby();
                }
                if (GUI.Button(new Rect(20, largeRect.height - 40, largeRect.width / 3, 30), "Shutdown Server")) {
                    gameManager.BackToMainMenu();
                }
            }

		}else{
			if(GUI.Button(new Rect(20, largeRect.height-40, largeRect.width/3, 30), "Disconnect")){
                gameManager.BackToMainMenu();
			}
		}
		
		GUI.Box(new Rect( (largeRect.width/3) + 40, 100, (largeRect.width*2/3)-60, largeRect.height - 150), ChatManager.SubmittedChat, lowerLeftTextAlign);

        if (!GameManager.IsTutorialScene()) {
			ChatManager.currentChat = GUI.TextField(new Rect( (largeRect.width/3) + 40, largeRect.height - 40 , (largeRect.width*2/3)-160, 20), ChatManager.currentChat);
		}
		
		// Choo choo, all aboard the dodgy train
		if(!GameManager.IsTutorialScene()){
			if(GUI.Button(new Rect(largeRect.width-100, largeRect.height - 40, 80, 20), "Enter")){
				chatManager.SubmitTextToChat(ChatManager.currentChat);
				ChatManager.ClearCurrentChat();
			}
		}
		
		if(Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return){
			chatManager.SubmitTextToChat(ChatManager.currentChat);
			ChatManager.ClearCurrentChat();
		}
		
	}

    private string PlayerList() {
        string playerNames = "";
        foreach (string playerName in NetworkManager.connectedPlayers.Values) {
            playerNames += playerName + "\n";
        }
        return playerNames;
    }
	#endregion


	
	//button prompts
	string promptText;
	public int promptShown;
	public void ButtonPrompt(string message, int buttonID){
		promptText = SettingsManager.keyBindings[buttonID].ToString() + "  -  " + message;
		promptShown = 10;
	}
	
	//tutorial prompts tutePromptShown
	string tutePromptText;
	public int tutePromptShown;
	public void TutorialPrompt(string message, int delay){
		tutePromptText = message;
		tutePromptShown = delay;
	}

	public void FailedToConnect(){
		connectionError = true;
	}
}
