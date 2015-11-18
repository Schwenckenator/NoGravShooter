using UnityEngine;
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

    public float endTime;
    public bool IsUseTimer { get; private set; }
    //public NetworkPlayer currentPlayer;
    // For Game Settings

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

    public const int MaxPlayers = 16;

    void Awake(){
        singleton = this;
	}

	void OnLevelWasLoaded(int level){
		SetCursorVisibility(true);

        if (NetworkManager.isServer && NetworkInfoWrapper.singleton.GameInProgress) {
            NetworkInfoWrapper.singleton.RpcSetCheats(DebugManager.allWeapon, DebugManager.allAmmo, DebugManager.allGrenade, DebugManager.allFuel);
            NetworkInfoWrapper.singleton.RefreshValues();
            GameClock.SetEndTime(SettingsManager.singleton.TimeLimitSec);
            ScoreVictoryManager.singleton.StartTimer();
        }
	}

    public void EndGame() {
        NetworkInfoWrapper.singleton.GameInProgress = false;

        GameClock.SetEndTime(secondsUntilKick);
        if (NetworkManager.isServer) {
            StartCoroutine(KickPlayersAfterGameEnd());
        }
    }
    IEnumerator KickPlayersAfterGameEnd() {
        yield return new WaitForSeconds(secondsUntilKick);
        if (!NetworkInfoWrapper.singleton.GameInProgress && !IsSceneMenu()) {
            NetworkManager.single.ServerReturnToLobby();
        }
    }

    public void LoadLevel() {
        if (!NetworkManager.isServer) {
            throw new ClientRunningServerCodeException();
        }
        ChatManager.ClearAllChat();
        NetworkManager.single.ServerChangeScene(SettingsManager.singleton.LevelName);
        NetworkInfoWrapper.singleton.GameInProgress = true;
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
        //stuff for timer. Don't set up if it's tutorial or the menu.
        if (levelName != "MenuScene" && levelName != "Tutorial") {
            NetworkInfoWrapper.singleton.GameInProgress = true;
            UIPauseSpawn.TutorialModeActive(false);
            UIPauseSpawn.SetServerNameText();
            if (secondsOfGame > 0) {
                endTime = Time.time + secondsOfGame;
                //GameClock.SetEndTime(endTime);
                ScoreVictoryManager.singleton.StartTimer();
                this.IsUseTimer = true;
            } else {
                this.IsUseTimer = false;
            }

            ChatManager.ClearAllChat();
            GameObject temp = Instantiate(gameModes[gameModeIndex], Vector3.zero, Quaternion.identity) as GameObject;
            gameMode = temp.GetInterface<IGameMode>();
        } else {
            NetworkInfoWrapper.singleton.GameInProgress = false;
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

    //[RPC]
    void RPCReturnToLobby() {
        //Clear data about a winner, the games over yo
        ScoreVictoryManager.singleton.ClearScoreData();
    }

    public void ReturnToLobby() {
        NetworkInfoWrapper.singleton.GameInProgress = false;
        NetworkManager.single.SendReturnToLobby();
    }
}
