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
    public int GameMode = 0;
    [SyncVar]
    public string GameModeName = "";
    [SyncVar(hook = "OnSecondsLeft")]
    public int SecondsLeft = 0;

    public SyncListInt startingWeapons = new SyncListInt();
    public SyncListString playerNames = new SyncListString();

    void Awake() {
        singleton = this;
        playerNames.Callback = OnPlayerName;
        StartCoroutine(SpawnMe());
    }

    public IEnumerator SpawnMe() {
        yield return null;
        if (NetworkManager.isServer) {
            NetworkServer.Spawn(gameObject);
            UILobby.singleton.SetServerName();
        }
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
        PlayerList.listDirty = true;
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

    public void AddPlayerName(string name) {
        StartCoroutine(CoAddPlayerName(name));
    }
    private IEnumerator CoAddPlayerName(string name) {
        bool done = false;
        while (!done) {
            if (isServer) {
                playerNames.Add(name);
                break;
            }
            yield return null;
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

    private void OnPlayerName(SyncListString.Operation op, int index) {
        PlayerList.listDirty = true;
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
        ChatManager.singleton.AddToChat(message);
    }
}
