using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkInfoWrapper : NetworkBehaviour {
    // Holds info that needs to be networked

    public static NetworkInfoWrapper singleton { get; private set; }

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

    public SyncListInt startingWeapons = new SyncListInt();
    public SyncListPlayerInfo connectedPlayers = new SyncListPlayerInfo();

    public SyncListPlayerInfo GetPlayers() {
        return connectedPlayers;
    }

    void Awake() {
        singleton = this;
        //connectedPlayers.Callback = OnPlayerList;
        StartCoroutine(SetLobbyName());
    }

    void Update() {
        if (NetworkClient.active) {
            string message = connectedPlayers == null ? "connectedPlayers is null" : "connectedPlayers found!";
            Debug.Log(message);
            //message = startingWeapons == null ? "startingWeapons is null" : "startingWeapons found!";
            //Debug.Log(message);
        }
    }

    public IEnumerator SetLobbyName() {
        yield return null;
        UILobby.singleton.SetServerName();
    }

    public override void OnStartServer() {
        Debug.Log("NetworkInfoWrapper OnStartServer");

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

    public void AddPlayer(PlayerInfo player) {
        connectedPlayers.Add(player);
        //StartCoroutine(CoAddPlayer(player));
    }

    private IEnumerator CoAddPlayer(PlayerInfo player) {
        yield return new WaitForSeconds(1.0f);
        
    }

    [Server]
    public void UpdateInfo(PlayerInfo player) {
        Debug.Log("Changing ID is " + player.id.ToString());
        Debug.Log("connectedPlayers.Count is " + connectedPlayers.Count.ToString());
        for (int i=0; i<connectedPlayers.Count; i++) {

            Debug.Log("List position " + i.ToString() + " is " + connectedPlayers[i].id.ToString());

            if (connectedPlayers[i].id == player.id) {
                connectedPlayers[i] = player;
                break;
            }
        }
    }

    // HOOKS
    private void OnServerName(string value) {
        Debug.Log("On Server Name.");
        ServerName = value;
        UILobby.singleton.SetServerName();
    }

    private void OnSecondsLeft(int timeLeft) {
        SecondsLeft = timeLeft;
        GameClock.ClientUpdateText();
    }

    private void OnPlayerList(SyncListPlayerInfo.Operation op, int index) {
        Debug.Log("OnPlayerName List Changed");
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
        Debug.Log("RpcChatMessage");
        ChatManager.UpdateChat(message);
    }

    [ClientRpc]
    public void RpcClearChat() {
        ChatManager.ClearAllChat();
    }
}
