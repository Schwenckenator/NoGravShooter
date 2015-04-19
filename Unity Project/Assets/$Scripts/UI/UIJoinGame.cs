using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIJoinGame : MonoBehaviour {

    private const string GameType = "NoGravShooter";
    private static List<ServerListEntry> serverList;
    private static ServerListManager listManager; 

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
            
            serverList.Add(newServer);
        }
    }

    public void ClearServerList() {
        foreach (ServerListEntry server in serverList) {
            Destroy(server.gameObject);
        }
        serverList.Clear();
    }
}
