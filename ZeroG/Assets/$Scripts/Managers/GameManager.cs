using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// Handles player, level loading and game interacitons
/// </summary>
public class GameManager : MonoBehaviour {

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

    [SerializeField]
	private static int maxStartingWeapons = 2;
	private int[] startingWeapons = new int[maxStartingWeapons];

	private bool myPlayerSpawned = false;

    [SerializeField]
	private GameObject playerPrefab;
	private GameObject[] spawnPoints;


    public static List<Weapon> weapon = new List<Weapon>();

    public float endTime;
    public bool IsUseTimer { get; private set; }
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
    public static int WeaponClassToWeaponId(Weapon input) {
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
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
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
        GameClock.SetEndTime(endTime);
    }
    public void SetPlayerMenu(bool input) {
        playerMenu = input;
    }
    #endregion
	

    public const int MaxPlayers = 15;

    NetworkView networkView;

    void Awake(){

		DontDestroyOnLoad(gameObject);

        networkView = GetComponent<NetworkView>();

		// Add weapons to list
        //weapon.Add(new LaserRifleValues());
        //weapon.Add(new SlugRifleValues());
        //weapon.Add(new LaserSniperValues());
        //weapon.Add(new ShotgunValues());
        //weapon.Add(new ForceShotgunValues());
        //weapon.Add(new RocketLauncherValues());
        //weapon.Add(new PlasmaBlasterValues());
        //weapon.Add(new MechPistolValues());
	}

	void OnLevelWasLoaded(int level){
		SetCursorVisibility(true);
		if(!GameManager.IsSceneMenu()){
			spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
			cameraMove = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMove>();

			PlayerDied(); //Died before you begin? Don't worry, it's just cleanup

            UIManager.instance.SetMenuWindow(Menu.PauseMenu);
			//
			if(Network.isServer){
                int[] temp = new int[2];

                temp[0] = SettingsManager.instance.SpawnWeapon1;
				if(SettingsManager.instance.SpawnWeapon1 == SettingsManager.instance.SpawnWeapon2){
					Debug.Log("same weapon twice");
					temp[1] = 99;
				} else {
					temp[1] = SettingsManager.instance.SpawnWeapon2;
				}
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
            if (actor.GetComponent<NetworkView>().isMine) {
                GetPlayerCameraMouseLook(actor).SetYDirection(SettingsManager.instance.MouseYDirection);
                actor.GetComponent<MouseLook>().SetYDirection(SettingsManager.instance.MouseYDirection);
                
                actor.GetComponent<ActorTeam>().SetTeam(NetworkManager.MyPlayer().Team); // Apply team to Actor

                UIPlayerHUD.SetupPlayer(actor);
                DynamicCrosshair.SetInventory(actor.GetComponent<WeaponInventory>());
                PlayerColourManager.instance.AssignColour(actor);
			}
		}
		//Reload all weapons
		for(int i=0; i< weapon.Count; i++){
			if(weapon[i].isEnergy){
				weapon[i].currentClip = weapon[i].defaultRemainingAmmo;
				weapon[i].remainingAmmo = 0;
			} else {
				weapon[i].currentClip = weapon[i].clipSize;
				weapon[i].remainingAmmo = weapon[i].defaultRemainingAmmo;
			}
		}

        Radar.instance.ActorsChanged();
        UIPauseSpawn.PlayerSpawned();

	}

	public void PlayerDied(){
		myPlayerSpawned = false;
        UIPauseSpawn.PlayerDied();
        SetPlayerMenu(false);
	}

	public void ManagerDetachCamera(){
		cameraMove.DetachCamera();
	}

    // If a player connects mid game,
    // we need to send an updated remaining time
    void OnPlayerConnected(NetworkPlayer connectingPlayer) {
        if (GameInProgress) {
            ChatManager.AddToLocalChat(NetworkManager.lastLevelPrefix.ToString());
            networkView.RPC("RPCLoadLevel", connectingPlayer,
                SettingsManager.instance.LevelName,
                NetworkManager.lastLevelPrefix,
                SettingsManager.instance.TimeLimitSec,
                SettingsManager.instance.GameModeIndexServer);

            networkView.RPC("SetEndTime", connectingPlayer, endTime - Time.time);

            //GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
            //foreach (GameObject obj in objs) {
            //    networkView.RPC("ValidPlayerID", connectingPlayer, obj.GetComponent<NetworkView>().viewID);
            //}
        }
    }
    void OnDisconnectedFromServer() {
        GameInProgress = false;
        IsUseTimer = false;

        foreach (GameObject actor in GameObject.FindGameObjectsWithTag("Player")) {
            Debug.Log("Destroyed actor");
            Destroy(actor);
        }
    }
    public void EndGame() {
        gameInProgress = false;
        
        SetEndTime(secondsUntilKick);
        if (Network.isServer) {
            StartCoroutine(KickPlayersAfterGameEnd());
        }
    }
    IEnumerator KickPlayersAfterGameEnd() {
        yield return new WaitForSeconds(secondsUntilKick);
        if (!gameInProgress && !IsSceneMenu()) {
            ReturnToLobby();
        }
    }

    public void LoadLevel() {
        if (Network.isClient) {
            throw new ClientRunningServerCodeException("Thrown in GameManager.");
        }
        DestroyManager.instance.ClearDestroyLists();
        SettingsManager.instance.RelayScoreToWin();

        networkView.RPC("RPCLoadLevel", RPCMode.All, 
            SettingsManager.instance.LevelName, 
            NetworkManager.lastLevelPrefix + 1, 
            SettingsManager.instance.TimeLimitSec, 
            SettingsManager.instance.GameModeIndexServer);
    }
    private void LoadLevelTutorial() {
        int dummyValue = 0; // Just to keep the method happy
        networkView.RPC("RPCLoadLevel", RPCMode.All, 
            "Tutorial", 
            NetworkManager.lastLevelPrefix + 1, 
            dummyValue,
            dummyValue);
    }

    [RPC]
    private void RPCLoadLevel(string levelName, int levelPrefix, int secondsOfGame, int gameModeIndex) {

        SettingsManager.instance.GameModeIndexClient = gameModeIndex;
        UIChat.UpdatePlayerLists();
        //stuff for timer. Don't set up if it's tutorial or the menu.
        if (levelName != "MenuScene" && levelName != "Tutorial") {
            GameInProgress = true;
            UIPauseSpawn.TutorialModeActive(false);
            UIPauseSpawn.SetServerNameText();
            if (secondsOfGame > 0) {
                endTime = Time.time + secondsOfGame;
                GameClock.SetEndTime(endTime);
                ScoreVictoryManager.instance.StartTimer();
                this.IsUseTimer = true;
            } else {
                this.IsUseTimer = false;
            }

            ChatManager.ClearAllChat();
            GameObject temp = Instantiate(gameModes[gameModeIndex], Vector3.zero, Quaternion.identity) as GameObject;
            gameMode = temp.GetInterface<IGameMode>();
        } else {
            GameInProgress = false;
            if (levelName == "Tutorial") {
                UIPauseSpawn.TutorialModeActive(true);
            }
        }

        NetworkManager.lastLevelPrefix = levelPrefix;
        NetworkManager.DisableRPC();
        Network.SetLevelPrefix(levelPrefix);
        Application.LoadLevel(levelName);
    }

    private IEnumerator LoadLevelCoRoutine(string levelName, int levelPrefix, int secondsOfGame, int gameModeIndex) {

        yield return new WaitForSeconds(1.0f);

        NetworkManager.lastLevelPrefix = levelPrefix;
        Network.SetSendingEnabled(0, false);
        Network.isMessageQueueRunning = false;

        SettingsManager.instance.GameModeIndexClient = gameModeIndex;
        //stuff for timer. Don't set up if it's tutorial or the menu.
        if (levelName != "MenuScene" && levelName != "Tutorial") {
            GameInProgress = true;
            UIPauseSpawn.TutorialModeActive(false);
            UIPauseSpawn.SetServerNameText();
            UIChat.UpdatePlayerLists();
            if (secondsOfGame > 0) {
                endTime = Time.time + secondsOfGame;
                GameClock.SetEndTime(endTime);
                ScoreVictoryManager.instance.StartTimer();
                this.IsUseTimer = true;
            } else {
                this.IsUseTimer = false;
            }

            ChatManager.ClearAllChat();
            GameObject temp = Instantiate(gameModes[gameModeIndex], Vector3.zero, Quaternion.identity) as GameObject;
            gameMode = temp.GetInterface<IGameMode>();
        } else {
            GameInProgress = false;
            if (levelName == "Tutorial") {
                UIPauseSpawn.TutorialModeActive(true);
            }
        }


        //Network.SetLevelPrefix(levelPrefix);
        Application.LoadLevel(levelName);
        Network.SetLevelPrefix(levelPrefix);

        yield return null;

        Network.isMessageQueueRunning = true;
        Network.SetSendingEnabled(0, true);
    }

    //load the tutorial level
    public IEnumerator LoadTutorialCoRoutine() {

        int portNum = 25000; // Dummy values for server creation
        int maxTutorialConnections = 1;

        NetworkManager.SetServerDetails(maxTutorialConnections, portNum, false);
        NetworkManager.InitialiseServer();

        LoadLevelTutorial();

        yield return new WaitForSeconds(1f);
        
    }
    public void LoadTutorial() {
        StartCoroutine(LoadTutorialCoRoutine());
    }

    public void ReturnToLobby() {

        networkView.RPC("RPCReturnToLobby", RPCMode.All);

        int dummy = 0; // Dummy is just to keep the code happy. Has no effect
        networkView.RPC("RPCLoadLevel", RPCMode.All, "MenuScene", NetworkManager.lastLevelPrefix + 1, dummy, dummy);
    }

    [RPC]
    void RPCReturnToLobby() {
        //Clear data about a winner, the games over yo
        ScoreVictoryManager.instance.ClearScoreData();
        UIManager.instance.SetMenuWindow(Menu.Lobby);
    }
}
