using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkInfoWrapper : NetworkBehaviour {
    // Holds info that needs to be networked

    public static NetworkInfoWrapper singleton { get; private set; }

    public bool debug = true;
    Logger log;

    [SyncVar(hook = "OnServerName")]
    public string ServerName = "";
    [SyncVar]
    public int ScoreToWin = 0;
    [SyncVar]
    public bool GameInProgress = false;
    [SyncVar]
    public int GameMode = 0;
    [SyncVar]
    public string GameModeName = "";
    [SyncVar(hook = "OnSecondsLeft")]
    public int SecondsLeft = 0;
    [SyncVar(hook = "OnPlayerListString")]
    public string playerListString = "";

    public SyncListInt startingWeapons = new SyncListInt();
    

    void Awake() {
        singleton = this;
        log = new Logger(debug);
        //connectedPlayers.Callback = OnPlayerList;
        StartCoroutine(SetLobbyName());
    }


    public IEnumerator SetLobbyName() {
        yield return null;
        UILobby.singleton.SetServerName();
    }

    public override void OnStartServer() {
        log.Log("NetworkInfoWrapper OnStartServer");

        ServerName = SettingsManager.singleton.ServerName;
        GameMode = SettingsManager.singleton.GameModeIndex;
        GameModeName = SettingsManager.singleton.GameModeName;
        ScoreToWin = SettingsManager.singleton.ScoreToWin;

        SetStartingWeapons();

    }

    public override void OnStartClient() {
        base.OnStartClient();
        UILobby.singleton.SetServerName();
    }

    private void SetStartingWeapons() {
        startingWeapons.Clear();
        startingWeapons.Add(SettingsManager.singleton.SpawnWeapon1);
        startingWeapons.Add(SettingsManager.singleton.SpawnWeapon2);
    }

    public void RefreshValues() {
        GameMode = SettingsManager.singleton.GameModeIndex;
        GameModeName = SettingsManager.singleton.GameModeName;
        ScoreToWin = SettingsManager.singleton.ScoreToWin;
        SecondsLeft = SettingsManager.singleton.TimeLimitSec;

        SetStartingWeapons();
    }

    // HOOKS
    private void OnServerName(string value) {
        log.Log("On Server Name.");
        ServerName = value;
        UILobby.singleton.SetServerName();
    }

    private void OnSecondsLeft(int timeLeft) {
        SecondsLeft = timeLeft;
        GameClock.ClientUpdateText();
    }

    public void SetPlayerListString(string newPlayerList) {

        log.Log("Set player list string.");
        log.Log(newPlayerList);
        playerListString = newPlayerList;
        PlayerList.Dirty();
    }

    private void OnPlayerListString(string newPlayerList) {
        log.Log("OnPlayerListString");
        playerListString = newPlayerList;
        PlayerList.Dirty();
    }

    // RPC and Commands
    [ClientRpc]
    public void RpcSetCheats(bool allWeapon, bool allAmmo, bool allGrenade, bool allFuel) {
        DebugManager.allWeapon = allWeapon;
        DebugManager.allAmmo = allAmmo;
        DebugManager.allGrenade = allGrenade;
        DebugManager.allFuel = allFuel;
    }

    [ClientRpc]
    public void RpcChatMessage(string message) {
        log.Log("RpcChatMessage");
        ChatManager.UpdateChat(message);
    }

    [ClientRpc]
    public void RpcClearChat() {
        ChatManager.ClearAllChat();
    }
}
