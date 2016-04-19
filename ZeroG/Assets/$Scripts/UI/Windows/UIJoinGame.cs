using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIJoinGame : MonoBehaviour {

    public static UIJoinGame singleton { get; private set; }


    private const string GameType = "NoGravShooter";
    private static List<ServerListEntry> serverList;
    private static ServerListManager listManager;

	// Use this for initialization
	void Start () {
        singleton = this;
        serverList = new List<ServerListEntry>();
        JoinGameInit();

	}
    void JoinGameInit() {
        
        listManager = GetComponentInChildren<ServerListManager>();
        Invoke("StartMatchmakerDelay", 0.1f);
    }
    private void StartMatchmakerDelay() {
        //NetworkManager.single.StartMatchMaker();
        //RefreshPress();
    }
	
    public void RefreshPress() {
        UIJoinGame.singleton.PollServerList();
    }
    public void PollServerList() {
        ClearServerList();
        
        //NetworkManager.single.matchMaker.ListMatches(0, 10, "", RefreshServerList);
        
    }

    private static void RefreshServerList(ListMatchResponse response) {
        if (!response.success) {
            UIMessage.ShowMessage("Cannot refresh list!");
            return;
        }
        
        foreach (MatchDesc match in response.matches) {
            ServerListEntry newServer = listManager.AddServer(match);
            serverList.Add(newServer);
        }
    }

    public static void ClearServerList() {
        foreach (ServerListEntry server in serverList) {
            Destroy(server.gameObject);
        }
        serverList.Clear();
    }

    public void JoinButtonPressed() {
        for (int i = 0; i < serverList.Count; i++) {
            if (serverList[i].IsPressed()) {
                ConnectToServer(serverList[i].match);
            }
        }
    }
    public void ConnectToServer(MatchDesc match) {
        //NetworkManager.SetClientDetailsMatch(match);
        //NetworkManager.single.matchMaker.JoinMatch(match.networkId, "", NetworkManager.single.OnMatchJoined);
    }
}
