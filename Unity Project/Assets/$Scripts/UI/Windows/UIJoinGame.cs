using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIJoinGame : MonoBehaviour {
    #region Instance
    //Here is a private reference only this class can access
    private static UIJoinGame _instance;
    //This is the public reference that other classes will use
    public static UIJoinGame instance {
        get {
            //If _instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<UIJoinGame>();
            }
            return _instance;
        }
    }
    #endregion


    private const string GameType = "NoGravShooter";
    private static List<ServerListEntry> serverList;
    private static ServerListManager listManager;

    private HostData masterServerData;

	// Use this for initialization
	void Start () {
        serverList = new List<ServerListEntry>();
        JoinGameInit();
	}
    void JoinGameInit() {
        Canvas canvas = UIManager.GetCanvas(Menu.JoinGame);
        listManager = canvas.GetComponentInChildren<ServerListManager>();
    }
	
    public void RefreshServerList() {
        ClearServerList();

        MasterServer.RequestHostList(GameType);

        HostData[] servers = MasterServer.PollHostList();
        foreach (HostData server in servers) {
            ServerListEntry newServer = listManager.AddServer(server.gameName, server.comment, server.connectedPlayers + "/" + server.playerLimit);
            newServer.hostData = server;
            serverList.Add(newServer);
        }
    }

    public void ClearServerList() {
        foreach (ServerListEntry server in serverList) {
            Destroy(server.gameObject);
        }
        serverList.Clear();
    }

    public void JoinButtonPressed() {
        for (int i = 0; i < serverList.Count; i++) {
            if (serverList[i].IsPressed()) {
                masterServerData = serverList[i].hostData;
                
                NetworkManager.SetClientDetailsMasterServer(masterServerData);
                NetworkManager.ConnectToServer();
            }
        }
    }
}
