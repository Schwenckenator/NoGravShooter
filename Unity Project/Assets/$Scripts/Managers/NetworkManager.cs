using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

///<summary>
/// Manages connections with the Network
/// </summary>
public class NetworkManager : MonoBehaviour {

    public const string GameType = "NoGravShooter";

    // Use this for initialization
    #region variables
    /// <summary>
    /// Holds names of players
    /// </summary>
    
    //public static Dictionary<NetworkPlayer, string> connectedPlayers = new Dictionary<NetworkPlayer, string>();
    public static List<Player> connectedPlayers = new List<Player>();

    private static string ipAddress;
    private static int portNum;
    private static bool useMasterServer;
    private static HostData masterServerData;
    private static int maxPlayers;
    private static bool useNat;

    private static bool rpcDisabled;
    public static int lastLevelPrefix = 0;
    #endregion

    private GuiManager guiManager;
    private static SettingsManager settingsManager;
    private ChatManager chatManager;

    void Start() {
        guiManager = GetComponent<GuiManager>();
        settingsManager = GetComponent<SettingsManager>();
        chatManager = GetComponent<ChatManager>();
    }

    public static Player GetPlayer(NetworkPlayer value) {
        return NetworkManager.connectedPlayers.Find(x => x.ID.Equals(value));
    }

    #region Connect To Server
    public static void ConnectToServer() {
        if (useMasterServer) {
            ConnectToServer(masterServerData);
        } else {
            ConnectToServer(ipAddress, portNum);
        }
    }

    public static void ConnectToServer(HostData hostData) {
        Network.Connect(hostData);
    }
    public static void ConnectToServer(string ipAddress, int portNum) {
        Network.Connect(ipAddress, portNum);
    } 
    #endregion

    public static void InitialiseServer() {
        if (useNat) useNat = !Network.HavePublicAddress();
        Network.InitializeServer(maxPlayers, portNum, useNat);
        if (useMasterServer) MasterServer.RegisterHost(GameType, settingsManager.MyServerName);
    }

    public static void Disconnect() {
        Network.Disconnect();
    }

    public static void DisableRPC() {
        Network.SetSendingEnabled(0, false);
        Network.isMessageQueueRunning = false;
        rpcDisabled = true;
    }

    
    #region SetDetails
    public static void SetClientDetails(HostData _masterServerData, bool _useMasterServer, string _ipAddress, int _portNum) {
        masterServerData = _masterServerData;
        useMasterServer = _useMasterServer;
        ipAddress = _ipAddress;
        portNum = _portNum;
    }
    public static void SetServerDetails(int _maxPlayers, int _portNum, bool _useMasterServer, bool _useNat = true) {
        useMasterServer = _useMasterServer;
        maxPlayers = _maxPlayers;
        portNum = _portNum;
        useNat = _useNat;

    }
    #endregion

    #region OnEvent
    void OnFailedToConnect() {
        GetComponent<GuiManager>().FailedToConnect();
    }
    void OnPlayerDisconnected(NetworkPlayer disconnectedPlayer) {

        string message = NetworkManager.GetPlayer(disconnectedPlayer).Name;

        message += " has disconnected.";
        chatManager.AddToChat(message, false);
        Network.RemoveRPCs(disconnectedPlayer);
        Network.DestroyPlayerObjects(disconnectedPlayer);
        networkView.RPC("RemovePlayerFromList", RPCMode.AllBuffered, disconnectedPlayer);
    }
    void OnApplicationQuit() {
        if (Network.isClient || Network.isServer) {
            Network.Disconnect();
        }
    }

    void OnDisconnectedFromServer() {

        GetComponent<GameManager>().SetCursorVisibility(true);
        if (!GameManager.IsMenuScene()) {
            Application.LoadLevel("MenuScene");
        }

        GetComponent<GuiManager>().SetCurrentMenuWindow(GuiManager.Menu.MainMenu);

        NetworkManager.connectedPlayers.Clear();
        ChatManager.ClearAllChat();
    }
    void OnLevelWasLoaded() {
        if (rpcDisabled) {
            Network.isMessageQueueRunning = true;
            Network.SetSendingEnabled(0, true);
            rpcDisabled = false;
        }
    }
    void OnServerInitialized() {
        networkView.RPC("AddPlayerToList", RPCMode.AllBuffered, Network.player, settingsManager.PlayerName);
        networkView.RPC("ChangeServerName", RPCMode.OthersBuffered, settingsManager.MyServerName);
    }
    void OnConnectedToServer() {
        // Set window to lobby
        guiManager.SetCurrentMenuWindow(GuiManager.Menu.Lobby);
        networkView.RPC("AddPlayerToList", RPCMode.AllBuffered, Network.player, settingsManager.PlayerName);
        
        string message = settingsManager.PlayerName + " has connected.";
        chatManager.AddToChat(message, false);
    }
    #endregion

    #region RPC
    [RPC]
    void ChangeServerName(string name) {
        settingsManager.DisplayServerName = name;
    }
    [RPC]
    void AddPlayerToList(NetworkPlayer newPlayer, string newPlayerName) {
        NetworkManager.connectedPlayers.Add(new Player(newPlayer, newPlayerName));

        guiManager.SetScoreBoardText(ScoreVictoryManager.UpdateScoreBoard());
    }

    [RPC]
    void RemovePlayerFromList(NetworkPlayer disconnectedPlayer) {
        NetworkManager.connectedPlayers.Remove(GetPlayer(disconnectedPlayer));

        guiManager.SetScoreBoardText(ScoreVictoryManager.UpdateScoreBoard());
    }
#endregion
    #region Destroy
    
    #endregion
}
