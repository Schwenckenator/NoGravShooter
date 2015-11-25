using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

///<summary>
/// Manages connections with the network
/// </summary>
public class NetworkManager : NetworkLobbyManager {

    public float newPlayerWait = 1.0f;

    public static NetworkManager single { get; private set; }
    public static bool isServer {
        get {
            return NetworkServer.active;
        }
    }

    public GameObject InfoPrefab;
    
    public static Player myPlayer;

    /// <summary>
    /// SERVER USE ONLY. Is not useable on clients
    /// </summary>
    [SerializeField]
    public static List<Player> connectedPlayers;
    

    private static MatchDesc matchData;
    private static bool useMatchmaking = false;

    private static int nextID = 0;
    public static int GetNextID() {
        return nextID++;
    }

    void Awake() {
        single = this;
        singleton = this;
        if (dontDestroyOnLoad) {
            DontDestroyOnLoad(gameObject);
        }
        single.StartMatchMaker();
        connectedPlayers = new List<Player>();
    }

    public static Player GetPlayer(int value) {
        return connectedPlayers.Find(x => x.ID.Equals(value));
    }
    public static Player MyPlayer() {
        return myPlayer;
    }
    public static bool DoesPlayerExist(int value) {
        return connectedPlayers.Exists(x => x.ID.Equals(value));
    }

    public void PlayerChangedTeam(Player player, TeamColour newTeam) {
        //NetworkView.RPC("RPCPlayerChangedTeam", RPCMode.Others, player.ID, (int)newTeam);
    }

    public static void Disconnect() {
        single.StopHost();
    }

    #region SetDetails
    public static void SetClientDetails(string _ipAddress, int _portNum) {
        single.networkAddress = _ipAddress;
        single.networkPort = _portNum;

    }
    public static void SetClientDetailsMatch(MatchDesc match) {
        matchData = match;
        useMatchmaking = true;
    }
    public static void SetServerDetails(int _maxPlayers, int _portNum) {
        single.maxPlayers = _maxPlayers;
        single.networkPort = _portNum;
    }
    #endregion

    //public override void OnLobbyServerConnect(NetworkConnection conn) {
    //    base.OnLobbyServerConnect(conn);
    //    Debug.Log(conn.connectionId.ToString() + ", OnConnect");
    //}
    //public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
    //    base.OnServerAddPlayer(conn, playerControllerId);
    //    GameObject player = (GameObject)Instantiate(lobbyPlayerPrefab.gameObject, Vector3.zero, Quaternion.identity);

    //    .ReplacePlayerForConnection(conn, player, playerControllerId);
    //}
    //public override void OnLobbyClientConnect(NetworkConnection conn) {
    //    ClientScene.AddPlayer(0);
    //}

    //void OnPlayerConnected(NetworkPlayer connectedPlayer) {
    //    foreach (Player player in NetworkManager.connectedPlayers) {
    //        //NetworkView.RPC("RPCPlayerChangedTeam", connectedPlayer, player.ID, (int)player.Team);
    //    }
    //}
    //void OnPlayerDisconnected(NetworkPlayer disconnectedPlayer) {

    //    string message = NetworkManager.GetPlayer(disconnectedPlayer).Name;
    //    message += " has disconnected.";
    //    ChatManager.singleton.AddToChat(message);

    //    Network.RemoveRPCs(disconnectedPlayer);
    //    Network.DestroyPlayerObjects(disconnectedPlayer);

    //    //NetworkView.RPC("RemovePlayerFromList", RPCMode.AllBuffered, disconnectedPlayer);
    //}
    void OnApplicationQuit() {
        if (isNetworkActive) {
            StopHost();
        }
    }
    public override void OnClientDisconnect(NetworkConnection conn) {
        base.OnClientDisconnect(conn);
        GameManager.SetCursorVisibility(true);
        if (!GameManager.IsSceneMenu()) {
            Debug.Log("Setting time back to normal.");
            Time.timeScale = 1.0f; // Make sure time is normal
            UIPlayerHUD.RemoveTutorialPrompt(); // Clear prompt
            Application.LoadLevel("MenuScene");
        }

        myPlayer = null;

        ChatManager.ClearAllChat();
    }

    public override void OnStartServer() {
        base.OnStartServer();
        GameObject newInfo = Instantiate(InfoPrefab) as GameObject;
        StartCoroutine(CoSpawnInfo(newInfo));
        Invoke("OnNewPlayer", newPlayerWait);
        
        //NetworkView.RPC("AddPlayerToList", RPCMode.AllBuffered, .player, SettingsManager.instance.PlayerName);
        //SettingsManager.singleton.RelayServerName();
        //AssignMyPlayerToTeam();

        //PlayerManager.singleton.Init(); // Initialise player
    }
    IEnumerator CoSpawnInfo(GameObject newInfo) {
        yield return null;
        NetworkServer.Spawn(newInfo);
    }
    public override void OnServerConnect(NetworkConnection conn) {
        base.OnServerConnect(conn);
        Invoke("OnNewPlayer", newPlayerWait);
    }
    public override void OnServerDisconnect(NetworkConnection conn) {
        base.OnServerDisconnect(conn);
        Invoke("OnNewPlayer", 0); // Don't have to wait for someone leaving
    }
    public override void OnClientConnect(NetworkConnection conn) {
        base.OnClientConnect(conn);
        UIMessage.CloseMessage();
        SettingsManager.singleton.ClearPasswordClient();
        UIManager.singleton.OpenReplace(UIManager.singleton.connectedOpen);
        //    // Set window to lobby

        //    //NetworkView.RPC("AddPlayerToList", RPCMode.AllBuffered, .player, SettingsManager.instance.PlayerName);

        //    string message = SettingsManager.singleton.PlayerName + " has connected.";
        //    ChatManager.singleton.AddToChat(message);
        //    AssignMyPlayerToTeam();

        //    PlayerManager.singleton.Init(); // Initialise players

        //    UIManager.singleton.UpdateArraysFromNetworkConnection();
    }

    public override void OnClientError(NetworkConnection conn, int errorCode) {
        base.OnClientError(conn, errorCode);
        SettingsManager.singleton.ClearPasswordClient();

        //string message = "";
        //switch (errorCode) {
        //    case NetworkConnectionError.ConnectionFailed:
        //        message = "Could not connect to Server.";
        //        break;
        //    case NetworkConnectionError.InvalidPassword:
        //        message = "Invalid password. Please try again.";
        //        break;
        //    default:
        //        message = error.ToString();
        //        break;
        //}
        string message = "Shit went wrong yo.";
        UIMessage.ShowMessage(message);
    }

    public void AssignMyPlayerToTeam() {
        if (SettingsManager.singleton.IsTeamGameMode() && NetworkManager.MyPlayer().HasNoTeam()) {
            //if (Network.isServer) {
            //    FindTeamWithLeastPlayers(Network.player);
            //} else { // Is client
            //    //NetworkView.RPC("FindTeamWithLeastPlayers", RPCMode.Server, Network.player, true); 
            //}
        } else if(!SettingsManager.singleton.IsTeamGameMode()){
            //NetworkManager.MyPlayer().ChangeTeam();
        }
    }
    //[RPC]
    void FindTeamWithLeastPlayers(NetworkPlayer networkPlayer, bool callback = false) {
        // This should only be called on server

        int[] teamCount = new int[2]; 
        foreach (Player player in NetworkManager.connectedPlayers) {
            if (player.Team == TeamColour.Red) {
                teamCount[0]++;
            } else {
                teamCount[1]++;
            }
        }
        int smallestTeamIndex = 0;
        if (teamCount[0] >= teamCount[1]) {
            smallestTeamIndex = 1;
        }

        //Callback
        if (callback) {
            //NetworkView.RPC("ChangeTeamFromIndex", networkPlayer, smallestTeamIndex);
        } else {
            ChangeTeamFromIndex(smallestTeamIndex);
        }
    }
    //[RPC]
    void ChangeTeamFromIndex(int newTeamIndex) {
        NetworkManager.MyPlayer().ChangeTeam(ScoreVictoryManager.singleton.Teams[newTeamIndex].Type);
    }

    // Spawning Data Structure
    private static bool isReadyToSpawn = false;
    public static bool IsReadyToSpawn() {
        return isReadyToSpawn;
    }

    public void OnNewPlayer() {
        //int index = 0;
        //connectedPlayers.Clear();
        //foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
        //    connectedPlayers.Add(player.GetComponent<Player>());
        //    connectedPlayers[index].ID = index++;
        //}
        ////string playerList = "";
        ////foreach (Player player in connectedPlayers) {
        ////    playerList += player.Name + "\n";
        ////}
        //NetworkInfoWrapper.singleton.playerListString = ScoreVictoryManager.UpdateScoreBoard();
    }
}
