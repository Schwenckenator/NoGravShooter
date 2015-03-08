using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// Handles player, level loading and game interacitons
/// </summary>
public class GameManager : MonoBehaviour {
    #region testVariables
    private static bool adminMode = false;
    private static bool allWeapon = false;
    private static bool allGrenade = false;

    public static bool IsAllWeapon() {
        return allWeapon;
    }
    public static bool IsAllGrenade() {
        return allGrenade;
    }
    public static bool IsAdminMode() {
        return adminMode;
    }
    private static void ToggleTestMode() {
        allWeapon   = !allWeapon;
        allGrenade  = !allGrenade;
        adminMode   = !adminMode;
    }
    #endregion

    #region Instance
    //Here is a private reference only this class can access
    private static GameManager _instance;
    //This is the public reference that other classes will use
    public static GameManager instance {
        get {
            //If _instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }
    #endregion

    public static IGameMode gameMode;
    public GameObject[] gameModes;
    #region Variable Declarations

    private static bool playerMenu;

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

    #region public static methods
    public static int WeaponClassToWeaponId(WeaponSuperClass input) {
        for (int i = 0; i < weapon.Count; i++) {
            if (input == weapon[i]) {
                return i;
            }
        }
        return -1;
    }
    public static bool IsSceneMenu() {
        return Application.loadedLevelName == "MenuScene";
    }
    public static bool IsSceneTutorial() {
        return Application.loadedLevelName == "Tutorial";
    }
    public static bool IsPlayerMenu() {
        return playerMenu;
    }
    public static int GetMaxStartingWeapons() {
        return maxStartingWeapons;
    }
    public static void SetCursorVisibility(bool visible) {
        Screen.showCursor = visible;
        Screen.lockCursor = !visible;
    }
    #endregion 

    #region Variable accessors
    public int[] GetStartingWeapons() {
        return startingWeapons;
    }
    public bool IsPlayerSpawned() {
        return myPlayerSpawned;
    }
    private MouseLook GetPlayerCameraMouseLook(GameObject player) {
        return player.transform.FindChild("CameraPos").GetComponent<MouseLook>();
    }
    #endregion

    #region Variable mutators
    [RPC]
    private void SetStartingWeapons(int[] selection) {
        startingWeapons = selection;
    }
    [RPC]
    public void SetEndTime(float remainingSeconds) {
        endTime = Time.time + remainingSeconds;
    }
    public void SetPlayerMenu(bool input) {
        playerMenu = input;
    }
    #endregion

    void Awake(){
		DontDestroyOnLoad(gameObject);

		// Add weapons to list
		weapon.Add(new LaserRifleValues());
		weapon.Add(new SlugRifleValues());
		weapon.Add(new LaserSniperValues());
		weapon.Add(new ShotgunValues());
		weapon.Add(new ForceShotgunValues());
		weapon.Add(new RocketLauncherValues());
		weapon.Add(new PlasmaBlasterValues());
	}

	void OnLevelWasLoaded(int level){
		SetCursorVisibility(true);
		if(!GameManager.IsSceneMenu()){
			spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
			cameraMove = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMove>();

			PlayerDied(); //Died before you begin? Don't worry, it's just cleanup

			//
			if(Network.isServer){
                int[] temp = new int[2];

                temp[0] = SettingsManager.instance.SpawnWeapon1;
                temp[1] = SettingsManager.instance.SpawnWeapon2;
                StartCoroutine(SetStartingWeaponsDelay(temp));
			}
		}
	}

    private IEnumerator SetStartingWeaponsDelay(int[] weapons) {
        yield return new WaitForSeconds(0.1f); // Small delay
        networkView.RPC("SetStartingWeapons", RPCMode.AllBuffered, weapons);
    }

	public void SpawnActor(){
		myPlayerSpawned = true;

		SetCursorVisibility(false);
		int point = Random.Range(0, spawnPoints.Length);
		Network.Instantiate(playerPrefab, spawnPoints[point].transform.position, spawnPoints[point].transform.rotation, 0);
		cameraMove.PlayerSpawned();

		GameObject[] list = GameObject.FindGameObjectsWithTag("Player");
		foreach(GameObject actor in list){
			if(actor.networkView.isMine){
                GetPlayerCameraMouseLook(actor).SetYDirection(SettingsManager.instance.MouseYDirection);
                actor.GetComponent<MouseLook>().SetYDirection(SettingsManager.instance.MouseYDirection);
                
                actor.GetComponent<ActorTeam>().SetTeam(NetworkManager.MyPlayer().Team); // Apply team to Actor

				fireWeapon = actor.GetComponent<FireWeapon>();
				playerResources = actor.GetComponent<PlayerResources>();

                PlayerColourManager.instance.AssignColour(actor);
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
            ToggleTestMode();
		}
        if (Input.GetKeyDown(KeyCode.F3)) {
            if (adminMode) {
                PlayerPrefs.DeleteAll();
                Debug.Log("PlayerPrefs Wiped!");
            }
        }
        if (Input.GetKeyDown(KeyCode.F6)) {
            GameObject[] actors = GameObject.FindGameObjectsWithTag("Player");
            
            foreach (GameObject actor in actors) {
                ChatManager.DebugMessage(actor.GetComponent<ActorTeam>().GetTeam().ToString());
            }
        }
		if(!GameManager.IsSceneMenu()){
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
        if (InputConverter.GetKeyDown(KeyBind.GrenadeSwitch)) {
            playerResources.ChangeGrenade();
        }
    }

	public void ManagerDetachCamera(){
		cameraMove.DetachCamera();
	}

    // If a player connects mid game,
    // we need to send an updated remaining time
    void OnPlayerConnected(NetworkPlayer player) {
        if (GameInProgress) {
            networkView.RPC("SetEndTime", player, endTime - Time.time);
        }
    }

    public void EndGame(Player winningPlayer) {
        SetWinner(winningPlayer.Name);
        
        gameInProgress = false;
        
        SetEndTime(secondsUntilKick);
        if (Network.isServer) {
            StartCoroutine(KickPlayersAfterGameEnd());
        }
    }
    public void EndGame() {
        // This method is here for when we enevitably write code for ties/draws
    }
    private void SetWinner(string winners) {
        ScoreVictoryManager.instance.VictorName = winners;
    }
    IEnumerator KickPlayersAfterGameEnd() {
        yield return new WaitForSeconds(secondsUntilKick);
        if (!gameInProgress && !IsSceneMenu()) {
            ReturnToLobby();
        }
    }

    public void LoadLevel() {
        if (Network.isClient) {
            throw new System.AccessViolationException("Is client when it needs to be server.");
        }
        GetComponent<DestroyManager>().ClearDestroyLists();
        
        networkView.RPC("RPCLoadLevel", RPCMode.AllBuffered, 
            SettingsManager.instance.LevelName, 
            NetworkManager.lastLevelPrefix, 
            SettingsManager.instance.TimeLimitSec, 
            SettingsManager.instance.GameModeIndex);
    }
    private void LoadLevelTutorial() {
        networkView.RPC("RPCLoadLevel", RPCMode.AllBuffered, "Tutorial", NetworkManager.lastLevelPrefix, SettingsManager.instance.TimeLimitSec);
    }

    [RPC]
    private void RPCLoadLevel(string levelName, int levelPrefix, int secondsOfGame, int gameModeIndex) {

        //stops tutorial scripts showing after you leave and start a game
        GuiManager.instance.TutorialPrompt("", 0);

        //stuff for timer. Don't set up if it's tutorial or the menu.
        if (levelName != "MenuScene" && levelName != "Tutorial") {
            GameInProgress = true;
            ScoreVictoryManager.instance.GameStart();
            endTime = Time.time + secondsOfGame;
            ChatManager.ClearAllChat();
            GameObject temp = Instantiate(gameModes[gameModeIndex], Vector3.zero, Quaternion.identity) as GameObject;
            gameMode = temp.GetComponent(typeof(IGameMode)) as IGameMode;
        } else {
            GameInProgress = false;
        }

        NetworkManager.lastLevelPrefix = levelPrefix;

        Network.SetLevelPrefix(levelPrefix);

        GuiManager.instance.SetScoreBoardText(ScoreVictoryManager.UpdateScoreBoard());


        NetworkManager.DisableRPC();
        Application.LoadLevel(levelName);
    }

    //load the tutorial level
    public IEnumerator LoadTutorial() {

        int portNum = 25000; // Dummy values for server creation
        int maxTutorialConnections = 1;

        NetworkManager.SetServerDetails(maxTutorialConnections, portNum, false);
        NetworkManager.InitialiseServer();

        LoadLevelTutorial();

        yield return new WaitForSeconds(1 / 5f);
        SpawnActor();
        GuiManager.instance.SetMyPlayerResources();
    }

    public void ReturnToLobby() {

        networkView.RPC("RPCReturnToLobby", RPCMode.AllBuffered);

        int dummy = 0;
        networkView.RPC("RPCLoadLevel", RPCMode.AllBuffered, "MenuScene", NetworkManager.lastLevelPrefix + 1, dummy);
    }

    [RPC]
    void RPCReturnToLobby() {
        //currentWindow = Menu.Lobby;
        GuiManager.instance.SetCurrentMenuWindow(GuiManager.Menu.Lobby);

        // Keep the players, but wipe the scores
        foreach (Player player in NetworkManager.connectedPlayers) {
            player.ClearScore();
        }

        //Clear data about a winner, the games over yo
        ScoreVictoryManager.instance.ClearWinnerData();
    }

    public void BackToMainMenu() {

        if (Network.isClient || Network.isServer) {
            Network.Disconnect();
        }

        ScoreVictoryManager.instance.ClearWinnerData();
        Application.LoadLevel("MenuScene");

    }
}
