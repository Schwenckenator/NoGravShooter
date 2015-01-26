using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///<summary>
/// Manages connections with the Network
/// </summary>
public class NetworkManager : MonoBehaviour {

    // Use this for initialization
    #region variables
    public static Dictionary<NetworkPlayer, string> connectedPlayers = new Dictionary<NetworkPlayer, string>();

    private static string ipAddress;
    private static int portNum;
    private static bool useMasterServer;
    private static HostData masterServerData;
    private static int maxPlayers;
    private static bool useNat;

    private static bool rpcDisabled;
    #endregion

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

    #region IsState

    #endregion

    #region OnEvent
    void OnFailedToConnect() {
        GetComponent<GuiManager>().FailedToConnect();
    }
    void OnPlayerDisconnected(NetworkPlayer player) {

        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);
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

        //Reset Varibles
        //numOfPlayers = 0;
        NetworkManager.connectedPlayers.Clear();
        ScoreVictoryManager.playerScores.Clear();
        ChatManager.ClearChat();
    }
    void OnLevelWasLoaded() {
        if (rpcDisabled) {
            Network.isMessageQueueRunning = true;
            Network.SetSendingEnabled(0, true);
            rpcDisabled = false;
        }
    }
    #endregion
}
