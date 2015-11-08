using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// Handles player, level loading and game interacitons
/// </summary>
public class GameManager : MonoBehaviour {

    public static GameManager singleton { get; private set; }

    public static IGameMode gameMode;
    public GameObject[] gameModes;
    #region Variable Declarations

	private bool myPlayerSpawned = false;

    [SerializeField]
	private GameObject playerPrefab;
	private GameObject[] spawnPoints;


    public float endTime;
    public bool IsUseTimer { get; private set; }
    //public NetworkPlayer currentPlayer;
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
    public static bool IsSceneMenu() {
        return Application.loadedLevelName == "MenuScene";
    }
    public static bool IsSceneTutorial() {
        return Application.loadedLevelName == "Tutorial";
    }
    public static void SetCursorVisibility(bool visible) {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }
    #endregion 

    #region Variable mutators
    //[RPC]
    //private void SetStartingWeapons(int[] selection) {
    //    startingWeapons = selection;
    //}
    //[RPC]
    public void SetEndTime(float remainingSeconds) {
        endTime = Time.time + remainingSeconds;
        GameClock.SetEndTime(endTime);
    }
    #endregion
	

    public const int MaxPlayers = 15;

    //new //NetworkView //NetworkView;

    void Awake(){
        singleton = this;
	}

	void OnLevelWasLoaded(int level){
		SetCursorVisibility(true);

        if (NetworkManager.isServer && GameInProgress) {
            NetworkInfoWrapper.singleton.RpcSetCheats(DebugManager.allWeapon, DebugManager.allAmmo, DebugManager.allGrenade, DebugManager.allFuel);
            NetworkInfoWrapper.singleton.RefreshValues();
        }
	}

    // If a player connects mid game,
    // we need to send an updated remaining time
    //void OnPlayerConnected(NetworkPlayer connectingPlayer) {
    //    if (GameInProgress) {
    //        //NetworkView.RPC("RPCLoadLevel", connectingPlayer,
    //            //SettingsManager.singleton.LevelName,
    //            //NetworkManager.lastLevelPrefix,
    //            //SettingsManager.sin.TimeLimitSec,
    //            //SettingsManager.oeu.GameModeIndexServer);

    //        //NetworkView.RPC("SetEndTime", connectingPlayer, endTime - Time.time);
    //    }
    //}
    //void OnDisconnectedFromServer() {
    //    GameInProgress = false;
    //    IsUseTimer = false;

    //    //foreach (GameObject actor in GameObject.FindGameObjectsWithTag("Player")) {
    //    //    Debug.Log("Destroyed actor");
    //    //    Destroy(actor);
    //    //}
    //}
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
        if (!NetworkManager.isServer) {
            throw new ClientRunningServerCodeException();
        }
        NetworkManager.single.ServerChangeScene(SettingsManager.singleton.LevelName);
        GameInProgress = true;
        ////NetworkView.RPC("RPCLoadLevel", RPCMode.All, 
        //    SettingsManager.eu.LevelName, 
        //    NetworkManager.lastLevelPrefix + 1, 
        //    SettingsManager.eu.TimeLimitSec, 
        //    SettingsManager.eu.GameModeIndexServer);
    }
    private void LoadLevelTutorial() {
        //int dummyValue = 0; // Just to keep the method happy
        //NetworkView.RPC("RPCLoadLevel", RPCMode.All, 
            //"Tutorial", 
            //NetworkManager.lastLevelPrefix + 1, 
            //dummyValue,
            //dummyValue);
    }

    //[RPC]
    private void RPCLoadLevel(string levelName, int secondsOfGame, int gameModeIndex) {

        //SettingsManager.singleton.GameModeIndexClient = gameModeIndex;
        UIChat.UpdatePlayerLists();
        //stuff for timer. Don't set up if it's tutorial or the menu.
        if (levelName != "MenuScene" && levelName != "Tutorial") {
            GameInProgress = true;
            UIPauseSpawn.TutorialModeActive(false);
            UIPauseSpawn.SetServerNameText();
            if (secondsOfGame > 0) {
                endTime = Time.time + secondsOfGame;
                GameClock.SetEndTime(endTime);
                ScoreVictoryManager.singleton.StartTimer();
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

        Application.LoadLevel(levelName);
    }

    //load the tutorial level
    public IEnumerator LoadTutorialCoRoutine() {

        int portNum = 25000; // Dummy values for server creation
        int maxTutorialConnections = 1;

        NetworkManager.SetServerDetails(maxTutorialConnections, portNum);
        //NetworkManager.InitialiseServer();

        yield return new WaitForSeconds(1f);

        LoadLevelTutorial();

        yield return new WaitForSeconds(1f);
        
    }
    public void LoadTutorial() {
        StartCoroutine(LoadTutorialCoRoutine());
    }

    public void ReturnToLobby() {

        //NetworkView.RPC("RPCReturnToLobby", RPCMode.All);

        //int dummy = 0; // Dummy is just to keep the code happy. Has no effect
        //NetworkView.RPC("RPCLoadLevel", RPCMode.All, "MenuScene", NetworkManager.lastLevelPrefix + 1, dummy, dummy);
    }

    //[RPC]
    void RPCReturnToLobby() {
        //Clear data about a winner, the games over yo
        ScoreVictoryManager.singleton.ClearScoreData();
    }
}
