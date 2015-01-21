using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreAndVictoryTracker : MonoBehaviour {

    public static Dictionary<NetworkPlayer, int> playerScores = new Dictionary<NetworkPlayer, int>();
    private GameManager manager;
	
	private NetworkPlayer winningPlayer;
	
	private bool roundstarted = false;

    void Start() {
        manager = GetComponent<GameManager>();
    }
	
	void Update() {
		if (!manager.RoundInProgress()){
			manager.endTime = Time.time + (manager.TimeLimit*60);
		} else if (roundstarted == false){
			if(Application.loadedLevelName != "Tutorial"){
				InvokeRepeating("CheckForVictory", 1, 1F);
			}
			roundstarted = true;
		}
	}

    public void KillScored(NetworkPlayer player) {
        Debug.Log(GameManager.connectedPlayers[player] + "kills");
        networkView.RPC("RPCKillScored", RPCMode.AllBuffered, player);
    }

    [RPC]
    private void RPCKillScored(NetworkPlayer player) {
        playerScores[player] += 1;
        //CheckForVictory();
        manager.GetComponent<GUIScript>().UpdateScoreBoard();
    }

    void CheckForVictory() {
		if(Application.loadedLevelName != "Tutorial"){
			if(Time.time >= manager.endTime && manager.RoundInProgress()){
				int maxValue = -1;
				foreach (NetworkPlayer player in playerScores.Keys) {
					if (playerScores[player] > maxValue){
						maxValue = playerScores[player];
						winningPlayer = player;
					}
				}
				if (Network.isServer) {
					manager.AddToChat("Time is up.", false);
					manager.AddToChat(GameManager.connectedPlayers[winningPlayer] + " wins!", false);
				}
				manager.IsVictor = true;
				manager.VictorName = GameManager.connectedPlayers[winningPlayer];
				manager.RoundEnd();
				roundstarted = false;
			}
		}
        foreach (NetworkPlayer player in playerScores.Keys) {
            if (playerScores[player] >= manager.KillsToWin) {
                if (Network.isServer) {
                    manager.AddToChat(GameManager.connectedPlayers[player] + " wins!", false);
                }
                manager.IsVictor = true;
                manager.VictorName = GameManager.connectedPlayers[player];
				manager.RoundEnd();
				roundstarted = false;
            }

        }
    }
}
