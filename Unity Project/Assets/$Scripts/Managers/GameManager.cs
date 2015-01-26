using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// Manages the player and the game 
/// 
/// </summary>
public class GameManager : MonoBehaviour {
	// *****************Test Mode Variable************************
	public static bool testMode = true;
    // ***********************************************************
    #region Variable Declarations
    //Managers
    private ChatManager chatManager;


    private static bool paused;

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
    public string currentPlayerName;
    public NetworkPlayer currentPlayer;

    // For Game Settings
    private string[] levelList = { "FirstLevel", "DerilictShipScene", "SpaceStationScene" };
    public string[] LevelList {
        get { return levelList; }
    }
    
    [SerializeField]
    private string levelName;
    public string LevelName {
        get { return levelName; }
        set { levelName = value; }
    }
	
	string[] gamemodeList = { "DeathMatch", "Team DeathMatch", "Capture the Flag", "Extraction", "Skirmish", "Team Skirmish", "Elimination", "Infection" };
    public string[] GameModeList {
        get { return gamemodeList; }
    }
	
	[SerializeField]
    private string gamemodeName;
    public string GameModeName {
        get { return gamemodeName; }
        set { gamemodeName = value; }
    }
	
	
    [SerializeField]
    private int timeLimit;
    public int TimeLimit {
        get { return timeLimit; }
        set { timeLimit = value; }
    }

    private bool gameInProgress = false;

    public bool GameInProgress {
        get { return gameInProgress; }
        set { gameInProgress = value; }
    }

    [SerializeField]
    private int killsToWin;
    public int KillsToWin {
        get { return killsToWin; }
        set { killsToWin = value; }
    }

    bool isVictor = false;

    public bool IsVictor {
        get { return isVictor; }
        set { isVictor = value; }
    }
    string victorName = "";

    public string VictorName {
        get { return victorName; }
        set { victorName = value; }
    }

    [SerializeField]
    private int secondsUntilKick = 10;

    #endregion

    void Awake(){
		DontDestroyOnLoad(gameObject);

        chatManager = GetComponent<ChatManager>();

		// Add weapons to list
		weapon.Add(new LaserRifleValues());
		weapon.Add(new SlugRifleValues());
		weapon.Add(new LaserSniperValues());
		weapon.Add(new ShotgunValues());
		weapon.Add(new ForceShotgunValues());
		weapon.Add(new RocketLauncherValues());
		weapon.Add(new PlasmaBlasterValues());

        KillsToWin = PlayerPrefs.GetInt("KillsToWin", 20);
		
        TimeLimit = PlayerPrefs.GetInt("TimeLimit", 15);

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

			Pause (false);

			//
			if(Network.isServer){
				startingWeapons[0] = PlayerPrefs.GetInt("1stWeapon", 0);
				startingWeapons[1] = PlayerPrefs.GetInt("2ndWeapon", 7);
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
	public static bool IsPaused(){
		return paused;
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
				cameraLook.SetYDirection(PlayerPrefs.GetInt("mouseYDirection"));

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
			if(Input.GetKeyDown(KeyCode.Escape)){
				SetCursorVisibility(!paused);
				Pause (!paused); // Toggle Pause
			}
			if(Input.GetKeyDown(KeyCode.Alpha1) && myPlayerSpawned){
				fireWeapon.ChangeWeapon(0);
			}
			if(Input.GetKeyDown(KeyCode.Alpha2) && myPlayerSpawned){
				fireWeapon.ChangeWeapon(1);
			}
			if(Input.GetKeyDown(KeyCode.Alpha3) && myPlayerSpawned){
				fireWeapon.ChangeWeapon(2);
			}
			if(Input.GetKeyDown(KeyCode.Alpha4) && myPlayerSpawned){
				fireWeapon.ChangeWeapon(3);
			}
			if(Input.GetKeyDown(KeyCode.Alpha5) && myPlayerSpawned){
				fireWeapon.ChangeWeapon(4);
			}
			if(Input.GetKeyDown(KeyCode.Alpha6) && myPlayerSpawned){
				fireWeapon.ChangeWeapon(5);
			}
			if(Input.GetKeyDown(KeyCode.Alpha7) && myPlayerSpawned){
				fireWeapon.ChangeWeapon(6);
			}
            if (Input.GetKeyDown(SettingsManager.keyBindings[(int)SettingsManager.KeyBind.GrenadeSwitch])) {
				Debug.Log ("Grenade Switch!");
				playerResources.ChangeGrenade();
			}
		}
	}
	public void Pause(bool input){
		paused = input;
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
        isVictor = true;
        gameInProgress = false;
        VictorName = NetworkManager.connectedPlayers[winningPlayer];
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
            GetComponent<GuiManager>().ReturnToLobby();
        }
    }
}
