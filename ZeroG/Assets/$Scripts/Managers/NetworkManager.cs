using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

///<summary>
/// Manages connections with the Network
/// </summary>
public class NetworkManager : MonoBehaviour {

    public static NetworkManager singleton { get; private set; }

    public const string GameType = "NoGravShooter";

    // Use this for initialization
    #region variables
    /// <summary>
    /// Holds names of players
    /// </summary>
    
    public static List<Player> connectedPlayers = new List<Player>();
    private static Player myPlayer;

    private static string ipAddress;
    private static int portNum;
    private static bool useMasterServer;
    private static HostData masterServerData;
    private static int maxPlayers;
    private static bool useNat;

    private static bool rpcDisabled;
    public static int lastLevelPrefix = 0;
    #endregion
    //NetworkView //NetworkView;
    static List<NetworkPlayer> actorOwners;
    void Start() {
        //NetworkView = GetComponent<//NetworkView>();
        actorOwners = new List<NetworkPlayer>();
    }
    void Update() {
        if (DebugManager.IsAdminMode() && Input.GetKeyDown(KeyCode.F4)) {
            foreach (Player player in connectedPlayers) {
                ChatManager.DebugMessage("Player \"" + player.Name + "\" with ID \"" + player.ID+"\"");
            }
        }
    }

    public static Player GetPlayer(NetworkPlayer value) {
        return NetworkManager.connectedPlayers.Find(x => x.ID.Equals(value));
    }
    public static Player MyPlayer() {
        if (myPlayer == null) {
            myPlayer = GetPlayer(Network.player);
        }
        return myPlayer;
    }
    public static bool DoesPlayerExist(NetworkPlayer value) {
        return NetworkManager.connectedPlayers.Exists(x => x.ID.Equals(value));
    }
    
    public void PlayerChangedTeam(Player player, TeamColour newTeam) {
        UIChat.UpdatePlayerLists();
        //NetworkView.RPC("RPCPlayerChangedTeam", RPCMode.Others, player.ID, (int)newTeam);
    }
    //[RPC]
    private void RPCPlayerChangedTeam(NetworkPlayer player, int newTeam) {
        GetPlayer(player).ChangeTeam((TeamColour)newTeam, false);
        UIChat.UpdatePlayerLists();
    }
    #region Connect To Server
    public static void ConnectToServer() {
        if (useMasterServer) {
            ConnectToServer(masterServerData, SettingsManager.singleton.PasswordClient);
        } else {
            ConnectToServer(ipAddress, portNum, SettingsManager.singleton.PasswordClient);
        }
    }

    public static void ConnectToServer(HostData hostData, string password) {
        Network.Connect(hostData, password);
    }
    public static void ConnectToServer(string ipAddress, int portNum, string password) {
        Network.Connect(ipAddress, portNum, password);
    } 
    #endregion

    public static void InitialiseServer() {
        if (useNat) useNat = !Network.HavePublicAddress();
        Network.incomingPassword = SettingsManager.singleton.PasswordServer;
        Network.InitializeServer(maxPlayers, portNum, useNat);
        if (useMasterServer) MasterServer.RegisterHost(GameType, SettingsManager.singleton.ServerNameServer);
    }
    public static void Disconnect() {
        Network.Disconnect();
    }
    public static void DisableRPC() {
        Network.SetSendingEnabled(0, false);
        Network.isMessageQueueRunning = false;
        rpcDisabled = true;
    }
    private static void EnableRPC() {
        Network.isMessageQueueRunning = true;
        Network.SetSendingEnabled(0, true);
        rpcDisabled = false;
    }

    #region SetDetails
    public static void SetClientDetails(string _ipAddress, int _portNum) {
        ipAddress = _ipAddress;
        portNum = _portNum;

        useMasterServer = false;
    }
    public static void SetClientDetailsMasterServer(HostData _masterServerData) {
        masterServerData = _masterServerData;
        useMasterServer = true;
    }
    public static void SetServerDetails(int _maxPlayers, int _portNum, bool _useMasterServer) {
        maxPlayers = _maxPlayers;
        portNum = _portNum;
        useMasterServer = _useMasterServer;
        useNat = _useMasterServer;
    }
    #endregion

    #region OnEvent
    void OnPlayerConnected(NetworkPlayer connectedPlayer) {
        foreach (Player player in NetworkManager.connectedPlayers) {
            //NetworkView.RPC("RPCPlayerChangedTeam", connectedPlayer, player.ID, (int)player.Team);
        }
    }
    void OnPlayerDisconnected(NetworkPlayer disconnectedPlayer) {

        string message = NetworkManager.GetPlayer(disconnectedPlayer).Name;
        message += " has disconnected.";
        ChatManager.singleton.AddToChat(message);

        Network.RemoveRPCs(disconnectedPlayer);
        Network.DestroyPlayerObjects(disconnectedPlayer);

        //NetworkView.RPC("RemovePlayerFromList", RPCMode.AllBuffered, disconnectedPlayer);
    }
    void OnApplicationQuit() {
        if (Network.isClient || Network.isServer) {
            Network.Disconnect();
        }
    }
    void OnDisconnectedFromServer() {

        GameManager.SetCursorVisibility(true);
        if (!GameManager.IsSceneMenu()) {
            Debug.Log("Setting time back to normal.");
            Time.timeScale = 1.0f; // Make sure time is normal
            UIPlayerHUD.RemoveTutorialPrompt(); // Clear prompt
            Application.LoadLevel("MenuScene");
        }

        UIManager.singleton.SetMenuWindow(Menu.MainMenu);

        NetworkManager.connectedPlayers.Clear();
        NetworkManager.actorOwners.Clear();
        NetworkManager.myPlayer = null;
        NetworkManager.isReadyToSpawn = false;
        
        ChatManager.ClearAllChat();
    }
    void OnLevelWasLoaded() {
        if (rpcDisabled) {
            EnableRPC();

            if (!GameManager.IsSceneMenu()) {
                //ReleaseObjects();
                isReadyToSpawn = true;
            } else {
                isReadyToSpawn = false;
            }
        }
    }
    void OnServerInitialized() {
        //NetworkView.RPC("AddPlayerToList", RPCMode.AllBuffered, Network.player, SettingsManager.instance.PlayerName);
        SettingsManager.singleton.RelayServerName();
        AssignMyPlayerToTeam();

        PlayerManager.singleton.Init(); // Initialise player

        UIManager.singleton.UpdateArraysFromNetworkConnection();
    }
    void OnConnectedToServer() {
        UIMessage.CloseMessage();
        SettingsManager.singleton.ClearPasswordClient();

        // Set window to lobby
        UIManager.singleton.SetMenuWindow(Menu.Lobby);
        //NetworkView.RPC("AddPlayerToList", RPCMode.AllBuffered, Network.player, SettingsManager.instance.PlayerName);
        
        string message = SettingsManager.singleton.PlayerName + " has connected.";
        ChatManager.singleton.AddToChat(message);
        AssignMyPlayerToTeam();

        PlayerManager.singleton.Init(); // Initialise players

        UIManager.singleton.UpdateArraysFromNetworkConnection();
    }
    void OnFailedToConnect(NetworkConnectionError error) {
        SettingsManager.singleton.ClearPasswordClient();
        
        string message = "";
        switch (error) {
            case NetworkConnectionError.ConnectionFailed:
                message = "Could not connect to Server.";
                break;
            case NetworkConnectionError.InvalidPassword:
                message = "Invalid password. Please try again.";
                break;
            default:
                message = error.ToString();
                break;
        }
        UIMessage.ShowMessage(message, true);
    }
    #endregion
    
    public void AssignMyPlayerToTeam() {
        if (SettingsManager.singleton.IsTeamGameMode() && NetworkManager.MyPlayer().HasNoTeam()) {
            if (Network.isServer) {
                FindTeamWithLeastPlayers(Network.player);
            } else { // Is client
                //NetworkView.RPC("FindTeamWithLeastPlayers", RPCMode.Server, Network.player, true); 
            }
        } else if(!SettingsManager.singleton.IsTeamGameMode()){
            NetworkManager.MyPlayer().ChangeTeam();
        }
    }
    //[RPC]
    void FindTeamWithLeastPlayers(NetworkPlayer networkPlayer, bool callback = false) {
        // This should only be called on server
        if(Network.isClient) throw new ClientRunningServerCodeException();

        int[] teamCount = new int[2]; 
        foreach (Player player in connectedPlayers) {
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

    #region RPC
    //[RPC]
    void AddPlayerToList(NetworkPlayer newPlayer, string newPlayerName) {
        NetworkManager.connectedPlayers.Add(new Player(newPlayer, newPlayerName));

        UIChat.UpdatePlayerLists();
    }
    //[RPC]
    void RemovePlayerFromList(NetworkPlayer disconnectedPlayer) {
        NetworkManager.connectedPlayers.Remove(GetPlayer(disconnectedPlayer));
        UIChat.UpdatePlayerLists();
    }
    #endregion

    // Spawning Data Structure
    private static bool isReadyToSpawn = false;
    public static bool IsReadyToSpawn() {
        return isReadyToSpawn;
    }

    //public static void ReserveObject(NetworkMessageInfo info, //NetworkView nView, GameObject obj) {
    //    DontDestroyOnLoad(obj);

    //    if (actorOwners.Contains(info.sender)) {
    //    } else {
    //        actorOwners.Add(info.sender);
    //    }
    //}

}
