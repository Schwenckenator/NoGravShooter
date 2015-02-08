using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// Handles player, level loading and game interacitons
/// </summary>
public class GameManager : MonoBehaviour {
	// *****************Test Mode Variable************************
	public static bool testMode = true;
    // ***********************************************************
    #region Variable Declarations
    //Managers
    private ChatManager chatManager;
    private GuiManager guiManager; 
    private ScoreVictoryManager scoreVictoryManager;
    private SettingsManager settingsManager;


    private static bool playerMenu;

	private MouseLook cameraLook;
	private CameraMove cameraMove;
	private FireWeapon fireWeapon;
	private PlayerResources playerResources;

    [SerializeField]
	private static int maxStartingWeapons = 2;
	private int[] startingWeapons = new int[maxStartingWeapons];

	private bool myPlayerSpawned = false;

    [SerializeField]
	private GameObject playerPrefab;
	private GameObject[] spawnPoints;

	
	public static List<WeaponSuperClass> weapon = new List<WeaponSuperClass>();

    public float endTime;
    //public string currentPlayerName;
    public NetworkPlayer currentPlayer;

    // For Game Settings

    private bool gameInProgress = false;

    public bool GameInProgress {
        get { return gameInProgress; }
        set { gameInProgress = value; }
    }

    [SerializeField]
    private int secondsUntilKick = 10;

    #endregion

    void Awake(){
		DontDestroyOnLoad(gameObject);

        chatManager = GetComponent<ChatManager>();
        settingsManager = GetComponent<SettingsManager>();
        guiManager = GetComponent<GuiManager>();
        scoreVictoryManager = GetComponent<ScoreVictoryManager>();

		// Add weapons to list
		weapon.Add(new LaserRifleValues());
		weapon.Add(new SlugRifleValues());
		weapon.Add(new LaserSniperValues());
		weapon.Add(new ShotgunValues());
		weapon.Add(new ForceShotgunValues());
		weapon.Add(new RocketLauncherValues());
		weapon.Add(new PlasmaBlasterValues());
	}

	public void AddToChat(string input, bool addPlayerPrefix = true){
		chatManager.SubmitTextToChat(input, addPlayerPrefix);
	}

	public static int WeaponClassToWeaponId(WeaponSuperClass input){
		for(int i=0; i<weapon.Count; i++){
			if(input == weapon[i]){
				return i;
			}
		}
		return -1;
	}

	void OnLevelWasLoaded(int level){
		SetCursorVisibility(true);
		if(!GameManager.IsMenuScene()){
			spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
			cameraMove = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMove>();

			PlayerDied(); //Died before you begin? Don't worry, it's just cleanup

			//
			if(Network.isServer){
                startingWeapons[0] = settingsManager.SpawnWeapon1;
                startingWeapons[1] = settingsManager.SpawnWeapon2;
				networkView.RPC ("SetStartingWeapons", RPCMode.OthersBuffered, startingWeapons);
			}
		}
	}

	[RPC]
	private void SetStartingWeapons(int[] selection){
		startingWeapons = selection;
	}
	public int[] GetStartingWeapons(){
		return startingWeapons;
	}

    [RPC]
    public void SetEndTime(float remainingSeconds) {
        endTime = Time.time + remainingSeconds;
    }


	public static bool IsMenuScene(){
		return Application.loadedLevelName == "MenuScene";
	}
    public static bool IsTutorialScene() {
        return Application.loadedLevelName == "Tutorial";
    }
	public static bool IsPlayerMenu(){
		return playerMenu;
	}
	public bool IsPlayerSpawned(){
		return myPlayerSpawned;
	}
	public void Spawn(){
		myPlayerSpawned = true;

		SetCursorVisibility(false);
		int point = Random.Range(0, spawnPoints.Length);
		Network.Instantiate(playerPrefab, spawnPoints[point].transform.position, spawnPoints[point].transform.rotation, 0);
		cameraMove.Spawn();

		GameObject[] list = GameObject.FindGameObjectsWithTag("Player");
		foreach(GameObject player in list){
			if(player.networkView.isMine){
				cameraLook = player.transform.FindChild("CameraPos").GetComponent<MouseLook>();
				cameraLook.SetYDirection(settingsManager.MouseYDirection);

				fireWeapon = player.GetComponent<FireWeapon>();
				playerResources = player.GetComponent<PlayerResources>();
			}
		}

		//Reload all weapons
		for(int i=0; i< weapon.Count; i++){
			weapon[i].currentClip = weapon[i].clipSize;
			weapon[i].remainingAmmo = weapon[i].defaultRemainingAmmo;
		}

	}

	public void PlayerDied(){
		myPlayerSpawned = false;
        SetPlayerMenu(false);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.F1)){
			testMode = !testMode;
		}
        if (Input.GetKeyDown(KeyCode.F3)) {
            if (testMode) {
                PlayerPrefs.DeleteAll();
                Debug.Log("PlayerPrefs Wiped!");
            }
        }
		if(!GameManager.IsMenuScene()){
            GetKeyStrokes();
		}
	}
    void GetKeyStrokes() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            SetCursorVisibility(!playerMenu);
            SetPlayerMenu(!playerMenu); // Toggle Pause
        }
        if (Input.GetKeyDown(KeyCode.Alpha1) && myPlayerSpawned) {
            fireWeapon.ChangeWeapon(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && myPlayerSpawned) {
            fireWeapon.ChangeWeapon(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && myPlayerSpawned) {
            fireWeapon.ChangeWeapon(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && myPlayerSpawned) {
            fireWeapon.ChangeWeapon(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5) && myPlayerSpawned) {
            fireWeapon.ChangeWeapon(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6) && myPlayerSpawned) {
            fireWeapon.ChangeWeapon(5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7) && myPlayerSpawned) {
            fireWeapon.ChangeWeapon(6);
        }
        if (Input.GetKeyDown(SettingsManager.keyBindings[(int)SettingsManager.KeyBind.GrenadeSwitch])) {
            Debug.Log("Grenade Switch!");
            playerResources.ChangeGrenade();
        }
    }

	public void SetPlayerMenu(bool input){
		playerMenu = input;
	}
	public void SetCursorVisibility(bool visible){
		Screen.showCursor = visible;
		Screen.lockCursor = !visible;
	}
	public void ManagerDetachCamera(){
		cameraMove.DetachCamera();
	}

    public static int GetMaxStartingWeapons() {
        return maxStartingWeapons;
    }

    // If a player connects mid game,
    // we need to send an updated remaining time
    void OnPlayerConnected(NetworkPlayer player) {
        if (GameInProgress) {
            networkView.RPC("SetEndTime", player, endTime - Time.time);
        }
    }

    public void EndGame(NetworkPlayer winningPlayer) {
        scoreVictoryManager.IsVictor = true;
        gameInProgress = false;
        scoreVictoryManager.VictorName = NetworkManager.connectedPlayers[winningPlayer];
        SetEndTime(secondsUntilKick);
        if (Network.isServer) {
            StartCoroutine(KickPlayersAfterGameEnd());
        }
    }
    public void EndGame() {
        // This method is here for when we enevitably write code for ties/draws
    }
    IEnumerator KickPlayersAfterGameEnd() {
        yield return new WaitForSeconds(secondsUntilKick);
        if (!gameInProgress && !IsMenuScene()) {
            ReturnToLobby();
        }
    }

    public void LoadLevel() {
        networkView.RPC("RPCLoadLevel", RPCMode.AllBuffered, settingsManager.LevelName, NetworkManager.lastLevelPrefix, settingsManager.TimeLimitSec);
    }
    void LoadLevelTutorial() {
        networkView.RPC("RPCLoadLevel", RPCMode.AllBuffered, "Tutorial", NetworkManager.lastLevelPrefix, settingsManager.TimeLimitSec);
    }

    [RPC]
    private void RPCLoadLevel(string levelName, int levelPrefix, int secondsOfGame) {


        //stops tutorial scripts showing after you leave and start a game
        guiManager.TutorialPrompt("", 0);

        //stuff for timer. Don't set up if it's tutorial or the menu.
        if (levelName != "MenuScene" && levelName != "Tutorial") {
            GameInProgress = true;
            GetComponent<ScoreVictoryManager>().GameStart();
            endTime = Time.time + secondsOfGame;
        } else {
            GameInProgress = false;
        }

        NetworkManager.lastLevelPrefix = levelPrefix;

        Network.SetLevelPrefix(levelPrefix);

        guiManager.SetScoreBoardText(ScoreVictoryManager.UpdateScoreBoard());
        ChatManager.ClearAllChat();

        Application.LoadLevel(levelName);
    }

    //load the tutorial level
    public IEnumerator LoadTutorial() {

        int portNum = 25000; // Dummy values for server creation
        int maxTutorialConnections = 1;

        NetworkManager.SetServerDetails(maxTutorialConnections, portNum, false, false);
        NetworkManager.InitialiseServer();

        LoadLevelTutorial();

        yield return new WaitForSeconds(1 / 5f);
        Spawn();
        guiManager.SetMyPlayerResources();
    }

    public void ReturnToLobby() {

        networkView.RPC("RPCReturnToLobby", RPCMode.AllBuffered);

        int dummy = 0;
        networkView.RPC("RPCLoadLevel", RPCMode.AllBuffered, "MenuScene", NetworkManager.lastLevelPrefix + 1, dummy);
    }

    [RPC]
    void RPCReturnToLobby() {
        //currentWindow = Menu.Lobby;
        guiManager.SetCurrentMenuWindow(GuiManager.Menu.Lobby);

        // Keep the players, but wipe the scores
        Dictionary<NetworkPlayer, int> buffer = new Dictionary<NetworkPlayer, int>();
        foreach (NetworkPlayer player in ScoreVictoryManager.playerScores.Keys) {
            buffer.Add(player, 0);
        }
        ScoreVictoryManager.playerScores = buffer;

        //Clear data about a winner, the games over yo
        scoreVictoryManager.ClearWinnerData();
    }

    public void BackToMainMenu() {

        if (Network.isClient || Network.isServer) {
            Network.Disconnect();
        }

        scoreVictoryManager.ClearWinnerData();
        Application.LoadLevel("MenuScene");

    }
}
