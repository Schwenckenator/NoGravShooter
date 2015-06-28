﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

///<summary>
/// Manages connections with the Network
/// </summary>
public class NetworkManager : MonoBehaviour {

    #region Instance
    //Here is a private reference only this class can access
    private static NetworkManager _instance;
    //This is the public reference that other classes will use
    public static NetworkManager instance {
        get {
            //If _instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<NetworkManager>();
            }
            return _instance;
        }
    }
    #endregion

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
    NetworkView networkView;
    void Start() {
        networkView = GetComponent<NetworkView>();
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
        networkView.RPC("RPCPlayerChangedTeam", RPCMode.Others, player.ID, (int)newTeam);
    }
    [RPC]
    private void RPCPlayerChangedTeam(NetworkPlayer player, int newTeam) {
        GetPlayer(player).ChangeTeam((TeamColour)newTeam, false);
        UIChat.UpdatePlayerLists();
    }
    #region Connect To Server
    public static void ConnectToServer() {
        if (useMasterServer) {
            ConnectToServer(masterServerData, SettingsManager.instance.PasswordClient);
        } else {
            ConnectToServer(ipAddress, portNum, SettingsManager.instance.PasswordClient);
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
        Network.incomingPassword = SettingsManager.instance.PasswordServer;
        Network.InitializeServer(maxPlayers, portNum, useNat);
        if (useMasterServer) MasterServer.RegisterHost(GameType, SettingsManager.instance.ServerNameServer);
    }
    public static void Disconnect() {
        Network.Disconnect();
    }
    public static void DisableRPC() {
        Network.isMessageQueueRunning = false;
        rpcDisabled = true;
    }
    private static IEnumerator EnableRPC(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        
        Network.isMessageQueueRunning = true;
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
            networkView.RPC("RPCPlayerChangedTeam", connectedPlayer, player.ID, (int)player.Team);
        }
    }
    void OnPlayerDisconnected(NetworkPlayer disconnectedPlayer) {

        string message = NetworkManager.GetPlayer(disconnectedPlayer).Name;
        message += " has disconnected.";
        ChatManager.instance.AddToChat(message);

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

        GameManager.SetCursorVisibility(true);
        if (!GameManager.IsSceneMenu()) {
            Application.LoadLevel("MenuScene");
        }

        UIManager.instance.SetMenuWindow(Menu.MainMenu);

        NetworkManager.connectedPlayers.Clear();
        NetworkManager.myPlayer = null;
        ChatManager.ClearAllChat();
    }
    void OnLevelWasLoaded() {
        if (rpcDisabled) {
            StartCoroutine(EnableRPC(0.0f));
        }
    }
    void OnServerInitialized() {
        networkView.RPC("AddPlayerToList", RPCMode.AllBuffered, Network.player, SettingsManager.instance.PlayerName);
        SettingsManager.instance.RelayServerName();
        AssignMyPlayerToTeam();

        UIManager.instance.UpdateArraysFromNetworkConnection();
    }
    void OnConnectedToServer() {
        
        SettingsManager.instance.ClearPasswordClient();

        // Set window to lobby
        UIManager.instance.SetMenuWindow(Menu.Lobby);
        networkView.RPC("AddPlayerToList", RPCMode.AllBuffered, Network.player, SettingsManager.instance.PlayerName);
        
        string message = SettingsManager.instance.PlayerName + " has connected.";
        ChatManager.instance.AddToChat(message);
        AssignMyPlayerToTeam();

        UIManager.instance.UpdateArraysFromNetworkConnection();
    }
    #endregion
    
    public static void AssignMyPlayerToTeam() {
        if (SettingsManager.instance.IsTeamGameMode() == NetworkManager.MyPlayer().HasNoTeam()) {
            NetworkManager.MyPlayer().ChangeTeam();
        }
    }

    #region RPC
    [RPC]
    void AddPlayerToList(NetworkPlayer newPlayer, string newPlayerName) {
        NetworkManager.connectedPlayers.Add(new Player(newPlayer, newPlayerName));

        UIChat.UpdatePlayerLists();
    }
    [RPC]
    void RemovePlayerFromList(NetworkPlayer disconnectedPlayer) {
        NetworkManager.connectedPlayers.Remove(GetPlayer(disconnectedPlayer));
        UIChat.UpdatePlayerLists();
    }
    #endregion
}