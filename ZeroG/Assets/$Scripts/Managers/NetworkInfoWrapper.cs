﻿using UnityEngine;
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
    [SyncVar]
    public int SecondsLeft = 0;

    public SyncListInt startingWeapons = new SyncListInt();

    void Awake() {
        singleton = this;
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
        SecondsLeft = SettingsManager.singleton.TimeLimitSec;

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
        Debug.Log("On Server Name.");
        ServerName = value;
        UILobby.singleton.SetServerName();
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