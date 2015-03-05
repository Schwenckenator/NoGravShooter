using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// Manages the GUI and all GUI operations
/// </summary>
public class GuiManager : MonoBehaviour {
    #region Instance
    //Here is a private reference only this class can access
    private static GuiManager _instance;
    //This is the public reference that other classes will use
    public static GuiManager instance {
        get {
            //If _instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<GuiManager>();
            }
            return _instance;
        }
    }
    #endregion

    public enum Menu { MainMenu, CreateGame, JoinGame, Options, Quit, Lobby, GameSettings, JoinByIP, Connecting, Keybind, ChangeKeybind, GraphicsOptions, PasswordInput }
    public void SetCurrentMenuWindow(Menu newWindow) {
        currentWindow = newWindow;
    }
    public bool IsCurrentWindow(Menu window) {
        return window == currentWindow;
    }

    #region  variable Dec
	
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
	
    public Texture[] EMPradar;
    public Texture[] EMPcursor;
    public Texture[] EMPstats;
	
    public Texture SniperScope;
    public Texture SniperScopeBorder;
    public bool showSniperScope = false;

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
	
	public float RedTeamRedLimit;
	public float RedTeamGreenLimit;
	public float RedTeamBlueLimit;
	public float BlueTeamRedLimit;
	public float BlueTeamGreenLimit;
	public float BlueTeamBlueLimit;
	
	private PlayerResources playerResource;
	
	private Menu currentWindow = 0;
	private bool displayGameSettingsWindow = false;
	private bool displayJoinByIpWindow = false;
	private bool displayChangeKeybindWindow = false;
    private bool displayMasterServerPassword = false;

    private Rect largeRect;
    private Rect smallRect;
	
	private const string GameType = "NoGravShooter";
	private const int MaxPlayers = 16;
	
	private bool useMasterServer = false;
	private HostData masterServerData;
	
    private string scoreBoardText = "";
    public void SetScoreBoardText(string value) {
        scoreBoardText = value;
    }
	
	private bool mouseInverted = false;
	
	private bool connectionError = false;
    private string connectionErrorMessage;
	private bool connectingNow = false;
	
	private bool autoPickupEnabled = false;
	
	
	private bool MedkitSpawning = true;
	private bool GrenadeSpawning = true;
	private bool WeaponSpawning = true;

    private bool countdown = false;

	
	private int chatboxwidth = 500;

	string[] weaponlist = {"Laser Rifle","Assault Rifle","Beam Sniper","Shotgun","Force Cannon","Rocket Launcher","Plasma Blaster"};
	string[] weaponlist2 = {"Laser Rifle","Assault Rifle","Beam Sniper","Shotgun","Force Cannon","Rocket Launcher","Plasma Blaster","None"};

    KeyBind editedBinding;

	GUIStyle upperLeftTextAlign;
	GUIStyle lowerLeftTextAlign;
	
	private Color playercol;
	private Texture2D coltexture;

    #endregion

    
    void Start(){

        ChangeGuiRectSize(); // Do this first
		
        SetCurrentMenuWindow(Menu.MainMenu);

		mouseInverted = (SettingsManager.instance.MouseYDirection == 1);
        autoPickupEnabled = (SettingsManager.instance.AutoPickup);
		
		MedkitSpawning = (SettingsManager.instance.MedkitCanSpawn == 1);
		GrenadeSpawning = (SettingsManager.instance.GrenadeCanSpawn == 1);
		WeaponSpawning = (SettingsManager.instance.WeaponCanSpawn == 1);
	}
	
	#region OnGUI
	void OnGUI(){
		// Sets GUI styles
		upperLeftTextAlign = new GUIStyle(GUI.skin.box);
		upperLeftTextAlign.alignment = TextAnchor.UpperLeft;

		lowerLeftTextAlign = new GUIStyle(GUI.skin.box);
		lowerLeftTextAlign.alignment = TextAnchor.LowerLeft;
		
		playercol = new Color (0.1f, 0.1f, 0.1f, 0.5f);
		coltexture = new Texture2D(1, 1);
		coltexture.SetPixel(0,0,playercol);
		coltexture.Apply();
		GUI.skin.box.normal.background = coltexture;
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

		if(GameManager.IsAdminMode()){
			GUI.Label(new Rect(Screen.width/2, 50, 100, 20), "TEST MODE");
		}

		if(connectingNow){
			if(!IsCurrentWindow(Menu.Connecting)){
				connectingNow = false;
			}
		}
		if(GameManager.IsMenuScene()){
			ChooseMenuWindow();

		}else if(GameManager.IsPlayerMenu()){
			GUI.Window(0, largeRect, PauseWindow, "MENU");
			
		}else if(GameManager.instance.IsPlayerSpawned()){
			PlayerGUI();
			
		}else if(!GameManager.IsMenuScene()){
			GUI.Window(1, largeRect, PauseWindow, SettingsManager.instance.DisplayServerName);
			
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
			GUI.Window ((int) Menu.Lobby, largeRect, LobbyWindow, SettingsManager.instance.DisplayServerName);
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
        if (displayMasterServerPassword) {
            GUI.ModalWindow((int)Menu.PasswordInput, smallRect, PasswordInputWindow, "Password Required!");
        }
	}
	#endregion

	#region PlayerGUI
	void PlayerGUI(){


		GUIStyle style = new GUIStyle("label");
		style.fontSize = 50;
		style.alignment = TextAnchor.UpperCenter;

        if (ScoreVictoryManager.instance.IsVictor()) {
            GUI.Label(new Rect(Screen.width/4, Screen.height/4, Screen.width/2, Screen.height/2), "Winner!\n" + ScoreVictoryManager.instance.VictorName, style);
        }

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

        if (!GameManager.IsTutorialScene()) {
            Rect combatLogRect = new Rect(Screen.width / 2 - chatboxwidth / 2, Screen.height - 120, chatboxwidth, 100);
            GUI.Box(combatLogRect, ChatManager.SubmittedChat, lowerLeftTextAlign);
        }
		
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

        DrawCrosshair();

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

    private void DrawCrosshair() {
        if (showSniperScope) {
            GUI.DrawTexture(new Rect(0, 0, Screen.width / 2 - Screen.height / 2, Screen.height), SniperScopeBorder);
            GUI.DrawTexture(new Rect(Screen.width / 2 - Screen.height / 2, 0, Screen.height, Screen.height), SniperScope);
            GUI.DrawTexture(new Rect(Screen.width / 2 + Screen.height / 2, 0, Screen.width / 2 - Screen.height / 2, Screen.height), SniperScopeBorder);
        }else{
            Rect rectCrosshair = new Rect(0, 0, 32, 32);
            rectCrosshair.center = new Vector2(Screen.width / 2, Screen.height / 2);
            GUI.DrawTexture(rectCrosshair, crosshair);
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
    bool IsDisplayTimer() {
        return !GameManager.IsTutorialScene();
    }
    void TimerGUI() {

        float timeLeftMins = Mathf.Floor((GameManager.instance.endTime - Time.time) / 60);
        float timeLeftSecs = Mathf.Floor((GameManager.instance.endTime - Time.time) - (timeLeftMins * 60));

        if (timeLeftMins >= 0 && timeLeftSecs >= 0) {
            GUI.Box(new Rect(Screen.width / 2 - 35, 10, 70, 23), timeLeftMins.ToString("00") + ":" + timeLeftSecs.ToString("00"));
        } else {
            GUI.Box(new Rect(Screen.width / 2 - 35, 10, 70, 23), "00:00");
        }
    }
    
	#endregion

	#region MainMenuWindow
	void MainMenuWindow(int windowId){
		Rect standard = new Rect(largeRect.width/4, 20, largeRect.width/2, 30);
		
		GUI.Label(standard, "Player Name");
		
		standard.y += 20;
        SettingsManager.instance.PlayerName = GUI.TextField(standard, SettingsManager.instance.PlayerName); 
		
		standard.y += 50;
		if(GUI.Button(standard, "Create Game")){
			//Check for preferred server name
            if (SettingsManager.instance.MyServerName == "") {
                SettingsManager.instance.MyServerName = SettingsManager.instance.PlayerName + "'s Server";
			}
            SettingsManager.instance.SaveSettings();

            SetCurrentMenuWindow(Menu.CreateGame);
		}
		
		standard.y += 50;
		if(GUI.Button(standard, "Join Game")){
			
			MasterServer.RequestHostList(GameType);
            SettingsManager.instance.SaveSettings();

            SetCurrentMenuWindow(Menu.JoinGame);
		}
		
		standard.y += 50;
		if(GUI.Button(standard, "Tutorial")){
			StartCoroutine(GameManager.instance.LoadTutorial());
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

	#region CreateGameWindow
	void CreateGameWindow(int windowId){
		Rect standard = new Rect(largeRect.width/4, 20, largeRect.width/2, 30);
		
		GUI.Label(standard, "Server Name");
		
		standard.y += 30;
        SettingsManager.instance.MyServerName = GUI.TextField(standard, SettingsManager.instance.MyServerName);
		
		standard.y += 50;		
		GUI.Label(standard, "Port Number");
		standard.y += 30;
        SettingsManager.instance.PortNumStr = GUI.TextField(standard, SettingsManager.instance.PortNumStr);
		
		standard.y += 50;	
		GUI.Label(standard, "Password Required");
		standard.y += 30;
        SettingsManager.instance.PasswordServer = GUI.TextField(standard, SettingsManager.instance.PasswordServer);
		
		standard.y += 50;
		if(GUI.Button(standard, "Create Game")){
            CreateGame(true);
		}

		standard.y += 50;
        if (GUI.Button(standard, "Create LAN Game")) {
            CreateGame(false);
        }
		
		standard.y += 50;
		if(GUI.Button(standard, "Back")){
			currentWindow =  Menu.MainMenu;
		}
	}
    private void CreateGame(bool online) {
        SettingsManager.instance.ParsePortNumber();

        if (SettingsManager.instance.PortNum >= 0) { // Check for error
            NetworkManager.SetServerDetails(MaxPlayers, SettingsManager.instance.PortNum, online);
            NetworkManager.InitialiseServer();
            SettingsManager.instance.SaveSettings();
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
                if (masterServerData.passwordProtected) {
                    displayMasterServerPassword = true;
                } else { 
                    currentWindow = Menu.Connecting; 
                }
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
        SettingsManager.instance.IpAddress = GUI.TextField(standard, SettingsManager.instance.IpAddress);


        standard.y += 50;
        GUI.Label(standard, "Port Number");
        standard.y += 30;
        SettingsManager.instance.PortNumStr = GUI.TextField(standard, SettingsManager.instance.PortNumStr);

        standard.y += 50;
        GUI.Label(standard, "Password");
        standard.y += 30;
        SettingsManager.instance.PasswordClient = GUI.TextField(standard, SettingsManager.instance.PasswordClient);

        standard.y += 50;
        if (GUI.Button(standard, "Join Game")) {
            displayJoinByIpWindow = false;
            SettingsManager.instance.ParsePortNumber();

            if (SettingsManager.instance.PortNum >= 0) { // Check for error
                SettingsManager.instance.SaveSettings();
                useMasterServer = false;
                currentWindow = Menu.Connecting;
            }

        }

        standard.y = smallRect.height - 50;
        if (GUI.Button(standard, "Close")) {
            displayJoinByIpWindow = false;
        }
    }

    void PasswordInputWindow(int windowId) {
        Rect standard = new Rect(20, 20, smallRect.width - 40, 30);

        GUI.Label(standard, "Password");
        standard.y += 30;
        SettingsManager.instance.PasswordClient = GUI.TextField(standard, SettingsManager.instance.PasswordClient);

        standard.y += 50;
        if (GUI.Button(standard, "Submit Password")) {
            displayMasterServerPassword = false;
            currentWindow = Menu.Connecting;
        }

        standard.y = smallRect.height - 50;
        if (GUI.Button(standard, "Cancel")) {
            displayMasterServerPassword = false;
        }
    }
	#endregion

	#region OptionsWindow
	void OptionsWindow(int windowId){
		Rect standard = new Rect(20, 20, -40+Screen.width/3, 30);
		standard.y += 50;
		GUI.Label(standard, "Mouse Sensitivity X: ");
        SettingsManager.instance.xMouseSensitivity = (float)System.Math.Round(SettingsManager.instance.xMouseSensitivity, 2);
        SettingsManager.instance.xMouseSensitivity = float.Parse(GUI.TextField(new Rect(standard.width-60, standard.y, 50, 20), SettingsManager.instance.xMouseSensitivity.ToString()));
		standard.y += 20;
        SettingsManager.instance.xMouseSensitivity = GUI.HorizontalSlider(standard, SettingsManager.instance.xMouseSensitivity, 0, 1);
		
		standard.y += 50;
		GUI.Label(standard, "Mouse Sensitivity Y: ");
        SettingsManager.instance.yMouseSensitivity = (float)System.Math.Round(SettingsManager.instance.yMouseSensitivity, 2);
        SettingsManager.instance.yMouseSensitivity = float.Parse(GUI.TextField(new Rect(standard.width-60, standard.y, 50, 20), SettingsManager.instance.yMouseSensitivity.ToString()));
		standard.y += 20;
        SettingsManager.instance.yMouseSensitivity = GUI.HorizontalSlider(standard, SettingsManager.instance.yMouseSensitivity, 0, 1);
		
		standard.y += 40;
		mouseInverted = GUI.Toggle(new Rect(standard.x, standard.y,  100, 30), mouseInverted, "Invert Y Axis");
		autoPickupEnabled = GUI.Toggle(new Rect(standard.x+100, standard.y,  250, 30), autoPickupEnabled, "Automatically switch to new weapons");
		
		standard.y += 50;
		GUI.Label(standard, "Field Of View: ");
        SettingsManager.instance.FieldOfView = (float)System.Math.Round(SettingsManager.instance.FieldOfView, 1);
        SettingsManager.instance.FieldOfView = float.Parse(GUI.TextField(new Rect(standard.width-60, standard.y, 50, 20), SettingsManager.instance.FieldOfView.ToString()));
		standard.y += 20;
        SettingsManager.instance.FieldOfView = GUI.HorizontalSlider(standard, SettingsManager.instance.FieldOfView, 50, 100);
		
		
		standard.x += standard.width+50;
		standard.y -= 200;
		
		GUI.Label(standard, "Colour (Red): ");
		standard.x += 20;
        SettingsManager.instance.ColourR = (float)System.Math.Round(SettingsManager.instance.ColourR, 2);
		standard.y += 20;
        SettingsManager.instance.ColourR = float.Parse(GUI.TextField(new Rect(standard.x, standard.y, 50, 20), SettingsManager.instance.ColourR.ToString()));
		float RED = SettingsManager.instance.ColourR;
		float GREEN = SettingsManager.instance.ColourG;
		float BLUE = SettingsManager.instance.ColourB;
		if(RED < RedTeamRedLimit){RED = RedTeamRedLimit;}
		if(GREEN > RedTeamGreenLimit){GREEN = RedTeamGreenLimit;}
		if(BLUE > RedTeamBlueLimit){BLUE = RedTeamBlueLimit;}
		playercol = new Color (RED, GREEN, BLUE, 1);
		coltexture = new Texture2D(1, 1);
		coltexture.SetPixel(0,0,playercol);
		coltexture.Apply();
		GUI.Label(new Rect(standard.x, standard.y+30, 70, 20), "Red Team");
		GUI.skin.box.normal.background = coltexture;
		GUI.Box(new Rect(standard.x, standard.y+60, 50, 30), GUIContent.none);
        SettingsManager.instance.ColourR = float.Parse(GUI.TextField(new Rect(standard.x, standard.y, 50, 20), SettingsManager.instance.ColourR.ToString()));
		standard.x -= 20;
		SettingsManager.instance.ColourR = GUI.VerticalSlider(new Rect(standard.x, standard.y, 20, 200), SettingsManager.instance.ColourR, 1, 0);
		
		standard.y -= 20;
		standard.x += 100;
		
		GUI.Label(standard, "Colour (Green): ");
		standard.x += 20;
        SettingsManager.instance.ColourG = (float)System.Math.Round(SettingsManager.instance.ColourG, 2);
		standard.y += 20;
        SettingsManager.instance.ColourG = float.Parse(GUI.TextField(new Rect(standard.x, standard.y, 50, 20), SettingsManager.instance.ColourG.ToString()));
		RED = SettingsManager.instance.ColourR;
		GREEN = SettingsManager.instance.ColourG;
		BLUE = SettingsManager.instance.ColourB;
		playercol = new Color (RED, GREEN, BLUE, 1);
		coltexture = new Texture2D(1, 1);
		coltexture.SetPixel(0,0,playercol);
		coltexture.Apply();
		GUI.Label(new Rect(standard.x, standard.y+30, 70, 20), "Default");
		GUI.skin.box.normal.background = coltexture;
		GUI.Box(new Rect(standard.x, standard.y+60, 50, 30), GUIContent.none);
        SettingsManager.instance.ColourG = float.Parse(GUI.TextField(new Rect(standard.x, standard.y, 50, 20), SettingsManager.instance.ColourG.ToString()));
		standard.x -= 20;
		SettingsManager.instance.ColourG = GUI.VerticalSlider(new Rect(standard.x, standard.y, 20, 200), SettingsManager.instance.ColourG, 1, 0);
		
		standard.y -= 20;
		standard.x += 100;
		
		GUI.Label(standard, "Colour (Blue): ");
		standard.x += 20;
        SettingsManager.instance.ColourB = (float)System.Math.Round(SettingsManager.instance.ColourB, 2);
		standard.y += 20;
        SettingsManager.instance.ColourB = float.Parse(GUI.TextField(new Rect(standard.x, standard.y, 50, 20), SettingsManager.instance.ColourB.ToString()));
		RED = SettingsManager.instance.ColourR;
		GREEN = SettingsManager.instance.ColourG;
		BLUE = SettingsManager.instance.ColourB;
		if(RED > BlueTeamRedLimit){RED = BlueTeamRedLimit;}
		if(GREEN > BlueTeamGreenLimit){GREEN = BlueTeamGreenLimit;}
		if(BLUE < BlueTeamBlueLimit){BLUE = BlueTeamBlueLimit;}
		playercol = new Color (RED, GREEN, BLUE, 1);
		coltexture = new Texture2D(1, 1);
		coltexture.SetPixel(0,0,playercol);
		coltexture.Apply();
		GUI.Label(new Rect(standard.x, standard.y+30, 70, 20), "Blue Team");
		GUI.skin.box.normal.background = coltexture;
		GUI.Box(new Rect(standard.x, standard.y+60, 50, 30), GUIContent.none);
        SettingsManager.instance.ColourB = float.Parse(GUI.TextField(new Rect(standard.x, standard.y, 50, 20), SettingsManager.instance.ColourB.ToString()));
		standard.x -= 20;
		SettingsManager.instance.ColourB = GUI.VerticalSlider(new Rect(standard.x, standard.y, 20, 200), SettingsManager.instance.ColourB, 1, 0);
		playercol = new Color (0.1f, 0.1f, 0.1f, 0.5f);
		coltexture = new Texture2D(1, 1);
		coltexture.SetPixel(0,0,playercol);
		coltexture.Apply();
		GUI.skin.box.normal.background = coltexture;
		
		
		standard.x -= standard.width+250;
		standard.y += 160;
		

		standard.y += 50;
		if(GUI.Button(standard, "Edit Keybinds")){
			currentWindow = Menu.Keybind;
		}

        if (GUI.Button(new Rect(standard.width+50, standard.y, standard.width, standard.height), "Graphics Settings")) {
            GraphicsOptionsSetup();
            currentWindow = Menu.GraphicsOptions;
        }

		standard.y += 50;
		if(GUI.Button(standard, "Back")){

            SettingsManager.instance.MouseYDirection = mouseInverted ? 1 : -1;
            SettingsManager.instance.AutoPickup = autoPickupEnabled;

            SettingsManager.instance.SaveSettings();

			currentWindow =  Menu.MainMenu;
		}
	}
	#endregion

    #region GraphicsOptionsWindow
    private int resolutionIndex = 0; // index for graphics resolutions
    private bool fullscreen = false;

    void GraphicsOptionsWindow(int windowId) {
        Rect standard = new Rect(20, 20, -40 + Screen.width / 3, 30);
        Resolution[] resolutions = Screen.resolutions;
        int maxIndex = resolutions.Length;

        standard.y += 50;
        GUI.Label(standard, "Resolution: ");
        if (GUI.Button(new Rect(100, standard.y, 100, 30), resolutions[resolutionIndex].width + " x " + resolutions[resolutionIndex].height)) {
            resolutionIndex++;
            resolutionIndex %= maxIndex;
        }
        
        standard.y += 50;
        fullscreen = GUI.Toggle(new Rect(standard.x, standard.y, 100, 30), fullscreen, "Fullscreen");

        standard.y += 50;
        if (GUI.Button(standard, "Save Settings")) {

            Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, fullscreen);
            StartCoroutine(WaitForGUIRectResize());

            currentWindow = Menu.Options;
        }
        standard.y += 50;
        if (GUI.Button(standard, "Cancel")) {
            currentWindow = Menu.Options;
        }
    }

    void GraphicsOptionsSetup() {
        Resolution[] resolutions = Screen.resolutions;
        int maxIndex = resolutions.Length;
        for (int i = 0; i < maxIndex; i++) {
            if (Screen.height == resolutions[i].height && Screen.width == resolutions[i].width) {
                resolutionIndex = i;
            }

        }
    }

    void ChangeGuiRectSize() {
        largeRect = new Rect(Screen.width/8, Screen.height/8, Screen.width*6/8, Screen.height*6/8);
	    smallRect = new Rect(Screen.width/5, Screen.height/4, Screen.width*3/5, Screen.height/2);
    }
    IEnumerator WaitForGUIRectResize() {
        yield return null;
        ChangeGuiRectSize();
    }
    #endregion

    #region KeyBindWindow
    void KeyBindWindow(int windowId){
		Rect standard = new Rect(20, 20, -40+Screen.width/3, 30);

		//Move Forward
		standard.y += 50;
		GUI.Label(standard, "Move Forward: ");
		if(GUI.Button(new Rect(140, standard.y, 150, 20), SettingsManager.keyBindings[(int)KeyBind.MoveForward].ToString())){
			editedBinding = KeyBind.MoveForward;
			displayChangeKeybindWindow = true;
		}

		standard.y += 30;
		GUI.Label(standard, "Move Backward: ");
		if(GUI.Button(new Rect(140, standard.y, 150, 20), SettingsManager.keyBindings[(int)KeyBind.MoveBack].ToString())){
			editedBinding = KeyBind.MoveBack;
			displayChangeKeybindWindow = true;
		}

		standard.y += 30;
		GUI.Label(standard, "Move Left: ");
		if(GUI.Button(new Rect(140, standard.y, 150, 20), SettingsManager.keyBindings[(int)KeyBind.MoveLeft].ToString())){
			editedBinding = KeyBind.MoveLeft;
			displayChangeKeybindWindow = true;
		}

		standard.y += 30;
		GUI.Label(standard, "Move Right: ");
		if(GUI.Button(new Rect(140, standard.y, 150, 20), SettingsManager.keyBindings[(int)KeyBind.MoveRight].ToString())){
			editedBinding = KeyBind.MoveRight;
			displayChangeKeybindWindow = true;
		}

		standard.y += 30;
		GUI.Label(standard, "Roll Left: ");
		if(GUI.Button(new Rect(140, standard.y, 150, 20), SettingsManager.keyBindings[(int)KeyBind.RollLeft].ToString())){
			editedBinding = KeyBind.RollLeft;
			displayChangeKeybindWindow = true;
		}

		standard.y += 30;
		GUI.Label(standard, "Roll Right: ");
		if(GUI.Button(new Rect(140, standard.y, 150, 20), SettingsManager.keyBindings[(int)KeyBind.RollRight].ToString())){
			editedBinding = KeyBind.RollRight;
			displayChangeKeybindWindow = true;
		}
		standard.y += 30;
		GUI.Label(standard, "Jump / Jetpack Up: ");
		if(GUI.Button(new Rect(140, standard.y, 150, 20), SettingsManager.keyBindings[(int)KeyBind.JetUp].ToString())){
			editedBinding = KeyBind.JetUp;
			displayChangeKeybindWindow = true;
		}
		standard.y += 30;
		GUI.Label(standard, "Jetpack Down: ");
		if(GUI.Button(new Rect(140, standard.y, 150, 20), SettingsManager.keyBindings[(int)KeyBind.JetDown].ToString())){
			editedBinding = KeyBind.JetDown;
			displayChangeKeybindWindow = true;
		}

		standard.y -= 210;
		standard.x+= 300;
		GUI.Label(standard, "Reload: ");
		if(GUI.Button(new Rect(420, standard.y, 150, 20), SettingsManager.keyBindings[(int)KeyBind.Reload].ToString())){
			editedBinding = KeyBind.Reload;
			displayChangeKeybindWindow = true;
		}

		standard.y += 30;
		GUI.Label(standard, "Grenade: ");
		if(GUI.Button(new Rect(420, standard.y, 150, 20), SettingsManager.keyBindings[(int)KeyBind.Grenade].ToString())){
			editedBinding = KeyBind.Grenade;
			displayChangeKeybindWindow = true;
		}

		standard.y += 30;
		GUI.Label(standard, "Switch Grenade: ");
		if(GUI.Button(new Rect(420, standard.y, 150, 20), SettingsManager.keyBindings[(int)KeyBind.GrenadeSwitch].ToString())){
			editedBinding = KeyBind.GrenadeSwitch;
			displayChangeKeybindWindow = true;
		}

		standard.y += 30;
		GUI.Label(standard, "Interact: ");
		if(GUI.Button(new Rect(420, standard.y, 150, 20), SettingsManager.keyBindings[(int)KeyBind.Interact].ToString())){
			editedBinding = KeyBind.Interact;
			displayChangeKeybindWindow = true;
		}
        standard.y += 30;
        GUI.Label(standard, "Interact: ");
        if (GUI.Button(new Rect(420, standard.y, 150, 20), SettingsManager.keyBindings[(int)KeyBind.StopMovement].ToString())) {
            editedBinding = KeyBind.StopMovement;
            displayChangeKeybindWindow = true;
        }

		standard.y += 50;
		if(GUI.Button(standard, "Back")){
			// Save Configuation-
            SettingsManager.instance.SaveKeyBinds();

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
        SettingsManager.instance.GameModeIndex = GUI.Toolbar(new Rect(20, 30, smallRect.width - 40, 30), SettingsManager.instance.GameModeIndex, SettingsManager.instance.GameModeList);

		GUI.Label(new Rect(20, 65, 100, 30), "Score Limit");
        SettingsManager.instance.ScoreToWin = int.Parse(GUI.TextField(new Rect(95, 65, 50, 20), SettingsManager.instance.ScoreToWin.ToString()));

		GUI.Label(new Rect(160, 65, 120, 30), "Time Limit (mins)");
        SettingsManager.instance.TimeLimitMin = int.Parse(GUI.TextField(new Rect(275, 65, 50, 20), SettingsManager.instance.TimeLimitMin.ToString()));

		GUI.Label(new Rect(20, 90, 90, 30), "Item Spawning");
		MedkitSpawning = GUI.Toggle(new Rect(115, 90,  80, 30), MedkitSpawning, "Medkits");
		GrenadeSpawning = GUI.Toggle(new Rect(185, 90,  80, 30), GrenadeSpawning, "Grenades");
		WeaponSpawning = GUI.Toggle(new Rect(265, 90,  80, 30), WeaponSpawning, "Weapons");
		
		GUI.Label(new Rect(20, 110, smallRect.width-40, 30), "Map");
        SettingsManager.instance.LevelIndex = GUI.Toolbar(new Rect(20, 130, smallRect.width - 40, 30), SettingsManager.instance.LevelIndex, SettingsManager.instance.LevelList);

		GUI.Label(new Rect(20, 165, smallRect.width-40, 30), "Starting Weapons");
        SettingsManager.instance.SpawnWeapon1 = GUI.Toolbar(new Rect(20, 185, smallRect.width - 40, 30), SettingsManager.instance.SpawnWeapon1, weaponlist);
        SettingsManager.instance.SpawnWeapon2 = GUI.Toolbar(new Rect(20, 220, smallRect.width - 40, 30), SettingsManager.instance.SpawnWeapon2, weaponlist2);

        if(GUI.Button(new Rect(20, smallRect.height-45, smallRect.width-40, 30), "Close")){
            SettingsManager.instance.SaveSettings();
			displayGameSettingsWindow = false;
		}
	}
	#endregion

	#region LobbyWindow
	void LobbyWindow(int windowId){
		if(Network.isServer){
            string startGameText = countdown ? "Cancel" : "Start Game";

            if (GUI.Button(new Rect(20, 20, largeRect.width / 3, 30), startGameText)) {
                if (countdown) {
                    StopCoroutine("CountdownStartGame");
                    ChatManager.instance.AddToChat("Countdown cancelled.");
                    countdown = false;
                } else {
                    StartCoroutine("CountdownStartGame");
                }
			}
			if(GUI.Button(new Rect(20, 60, largeRect.width/3, 30), "Settings")){
				displayGameSettingsWindow = true;
			}
		}

		GUI.Box(new Rect(20, 100, largeRect.width/3, largeRect.height-150), PlayerList(), upperLeftTextAlign);
        if (GUI.Button(new Rect(largeRect.width / 3 - 100, 105, 100, 20), "Change Team")) {
            NetworkManager.GetPlayer(Network.player).IncrementTeam();
        }
        string message = Network.isServer ? "Shutdown Server": "Disconnect";
        if (GUI.Button(new Rect(20, largeRect.height - 40, largeRect.width / 3, 30), message)) {
            NetworkManager.Disconnect();
            if (countdown) StopCoroutine("CountdownStartGame");
            countdown = false;
        }
		
		GUI.Box(new Rect( (largeRect.width/3) + 40, 20, (largeRect.width*2/3)-60, largeRect.height - 80), ChatManager.SubmittedChat, lowerLeftTextAlign);
		
		ChatManager.currentChat = GUI.TextField(new Rect( (largeRect.width/3) + 40, largeRect.height - 40 , (largeRect.width*2/3)-160, 20), ChatManager.currentChat);
		
		// Choo choo, all aboard the dodgy train
		if(GUI.Button(new Rect(largeRect.width-100, largeRect.height - 40, 80, 20), "Enter")){
			ChatManager.instance.AddToChat(ChatManager.currentChat, true);
			ChatManager.ClearCurrentChat();
		}
		
		if(Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return){
			ChatManager.instance.AddToChat(ChatManager.currentChat, true);
			ChatManager.ClearCurrentChat();
		}
	}

    IEnumerator CountdownStartGame() {
        if (GameManager.IsAdminMode()) {
            GameManager.instance.LoadLevel();
            yield break;
        }
        countdown = true;
        int waitSeconds = 5;
        ChatManager.instance.AddToChat("Game starting in...");
        do {
            ChatManager.instance.AddToChat(waitSeconds.ToString() + "...");
            yield return new WaitForSeconds(1.0f);
        } while (waitSeconds-- > 0);
        GameManager.instance.LoadLevel();
        countdown = false;
    }
	#endregion

	#region ConnectingWindow
	void ConnectingWindow(int windowId){
		Rect standard = new Rect(smallRect.width/4, smallRect.height/4, smallRect.width/2, smallRect.height/2);
		GUIStyle style = new GUIStyle("box");
		style.alignment = TextAnchor.MiddleCenter;
		
		if(!connectingNow){
			connectingNow = true;
            NetworkManager.SetClientDetails(masterServerData, useMasterServer, SettingsManager.instance.IpAddress, int.Parse(SettingsManager.instance.PortNumStr));
            NetworkManager.ConnectToServer();
		}
		if(!connectionError){
			GUI.Box (standard, "Connecting....", style);
		}else{
			GUI.Box (standard, "An error occured.\n"+connectionErrorMessage, style);
			
			standard.y = smallRect.height-50;
			standard.height = 30;
			
			if(GUI.Button(standard, "Back")){
				connectionError = false;
				currentWindow =  Menu.JoinGame;
			}
		}
	}
	#endregion

    public void SetMyPlayerResources() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players) {
            if (player.networkView.isMine) {
                playerResource = player.GetComponent<PlayerResources>();
            }
        }
    }

	#region PauseWindow
	void PauseWindow(int windowId){
        if (IsDisplayTimer()) {
            TimerGUI(); // Display the timer even when paused
        }

		if(GameManager.instance.IsPlayerSpawned()){
            string strReturnToGame = GameManager.IsTutorialScene() ? "Return to Simulation" : "Return to Game";
			if(GUI.Button(new Rect(20, 50, largeRect.width-40, 30), strReturnToGame)){
				GameManager.instance.SetPlayerMenu(false);
				GameManager.SetCursorVisibility(false);
			}
		}else {
            if (ScoreVictoryManager.instance.IsVictor()) {
                GUI.enabled = false;
                GUI.Button(new Rect(20, 50, largeRect.width - 40, 30), ScoreVictoryManager.instance.VictorName + " has won!");
                GUI.enabled = true;
            } else {
                if (GUI.Button(new Rect(20, 50, largeRect.width - 40, 30), "Spawn")) {
                    GameManager.instance.SpawnPlayer();
                    SetMyPlayerResources();
                }
            }

		}

        GUI.Box(new Rect(20, 100, largeRect.width / 3, largeRect.height - 190), PlayerList(), upperLeftTextAlign);

        if (!GameManager.IsTutorialScene() && Network.isServer) {
            if (GUI.Button(new Rect(20, largeRect.height - 80, largeRect.width / 3, 30), "Return to Lobby")) {
                GameManager.instance.ReturnToLobby();
            }
        }
        if (GUI.Button(new Rect(20, largeRect.height - 40, largeRect.width / 3, 30), DisconnectButtonText())) {
            GameManager.instance.BackToMainMenu();
        }
		
		GUI.Box(new Rect( (largeRect.width/3) + 40, 100, (largeRect.width*2/3)-60, largeRect.height - 150), ChatManager.SubmittedChat, lowerLeftTextAlign);

        if (!GameManager.IsTutorialScene()) {
			ChatManager.currentChat = GUI.TextField(new Rect( (largeRect.width/3) + 40, largeRect.height - 40 , (largeRect.width*2/3)-160, 20), ChatManager.currentChat);
		}
		
		// Choo choo, all aboard the dodgy train
		if(!GameManager.IsTutorialScene()){
			if(GUI.Button(new Rect(largeRect.width-100, largeRect.height - 40, 80, 20), "Enter")){
				ChatManager.instance.AddToChat(ChatManager.currentChat, true);
				ChatManager.ClearCurrentChat();
			}
		}
		
		if(Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return){
			ChatManager.instance.AddToChat(ChatManager.currentChat, true);
			ChatManager.ClearCurrentChat();
		}
		
	}

    private string DisconnectButtonText() {
        string message;
        if (Network.isServer) {
            message = GameManager.IsTutorialScene() ? "Shutdown Simulation" : "Shutdown Server";
        } else {
            message = "Disconnect";
        }
        return message;
    }

    private string PlayerList() {
        string playerNames = "";
        foreach (Player player in NetworkManager.connectedPlayers) {
            playerNames += player.Name + " " + player.Team.ToString()+ "\n";
        }
        return playerNames;
    }
	#endregion


	
	//button prompts
	string promptText;
	private int promptShown;
	public void ButtonPrompt(string message, int buttonID){
		promptText = SettingsManager.keyBindings[buttonID].ToString() + "  -  " + message;
		promptShown = 20;
	}
	
	//tutorial prompts tutePromptShown
	string tutePromptText;
	public int tutePromptShown;
	public void TutorialPrompt(string message, int delay){
		tutePromptText = message;
		tutePromptShown = delay;
	}

	void OnFailedToConnect(NetworkConnectionError error){
		connectionError = true;
        connectionErrorMessage = error.ToString();
        
        SettingsManager.instance.ClearPasswordClient();
	}
}
