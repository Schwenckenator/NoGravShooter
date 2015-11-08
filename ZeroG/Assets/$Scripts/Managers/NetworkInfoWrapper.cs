using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkInfoWrapper : NetworkBehaviour{
    // Holds info that needs to be networked

    public static NetworkInfoWrapper singleton { get; private set; }

    [SyncVar]
    public string ServerName;
    [SyncVar]
    public int ScoreToWin;
    [SyncVar]
    public int GameMode;
    [SyncVar]
    public string GameModeName;
    [SyncVar]
    public int SecondsLeft;

    public SyncListInt startingWeapons = new SyncListInt();

    // Initialisation methods
    public override void OnStartClient() {
        singleton = this;
    }
    public override void OnStartServer() {
        ServerName = SettingsManager.singleton.ServerName;
        GameMode = SettingsManager.singleton.GameModeIndex;
        GameModeName = SettingsManager.singleton.GameModeName;
        ScoreToWin = SettingsManager.singleton.ScoreToWin;
        SecondsLeft = SettingsManager.singleton.TimeLimitSec;

        SetStartingWeapons();
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

    // RPC and Commands
    [ClientRpc]
    public void RpcSetCheats(bool allWeapon, bool allAmmo, bool allGrenade, bool allFuel) {
        DebugManager.allWeapon = allWeapon;
        DebugManager.allAmmo = allAmmo;
        DebugManager.allGrenade = allGrenade;
        DebugManager.allFuel = allFuel;
    }

}
