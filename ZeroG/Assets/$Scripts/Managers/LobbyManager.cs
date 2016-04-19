using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class LobbyManager : NetworkLobbyManager {
    static short MsgKicked = MsgType.Highest + 1;
    static public LobbyManager s_Singleton;

    static public bool isServer { get {
            return NetworkServer.active;
        }
    }
    static public bool IsSceneMenu { get {
            return SceneManager.GetActiveScene().name == "MenuScene";
        }
    }

    public int countdownTime = 3;

    UIManager menuManager;

    protected void Start() {
        s_Singleton = this;
        Invoke("FindMenuManager", 0.05f);
    }
    private void FindMenuManager() {
        menuManager = GameObject.FindGameObjectWithTag("MenuWindow").GetComponent<UIManager>();
    }

    public void LoadTutorial() {
        // TODO Implement
        Debug.LogError("Not Implemented");
    }
    public void LoadGameScene() {
        ServerChangeScene(SettingsManager.singleton.LevelName);
    }

    public void Disconnect() {
        if (isServer) {
            StopHost();
        } else {
            StopClient();
        }
    }

    public void AddLocalPlayer() {
        TryToAddPlayer();
    }
    public void RemovePlayer(LobbyPlayer player) {
        player.RemovePlayer();
    }

    public void CreateGame(bool useMatchmaking) {
        // TODO Don't ignore matchmaking
        this.maxPlayers = 16;
        this.networkPort = SettingsManager.singleton.PortNum;
        StartHost();
    }

    public override void OnStartHost() {
        base.OnStartHost();
        Debug.Log("Started Host");
        menuManager.OpenConnected();
    }
    public override void OnLobbyClientConnect(NetworkConnection conn) {
        if (!NetworkServer.active) {
            menuManager.OpenConnected();
        }
    }
    public override void OnLobbyClientDisconnect(NetworkConnection conn) {

    }
}
