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
    public void GameStart() {
        StartCoroutine(CheckForGameEnd());
    }

    public void KillScored(NetworkPlayer player) {
        Debug.Log(GameManager.connectedPlayers[player] + "kills");
        networkView.RPC("RPCKillScored", RPCMode.AllBuffered, player);
    }

    [RPC]
    private void RPCKillScored(NetworkPlayer player) {
        playerScores[player] += 1;
        CheckForVictory(false);
        manager.GetComponent<GUIScript>().UpdateScoreBoard();
    }

    void CheckForVictory(bool forceVictor) { // Force a victory?
        if(forceVictor){
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
            manager.GameInProgress = false;
			roundstarted = false;
        } else {
            foreach (NetworkPlayer player in playerScores.Keys) {
                if (playerScores[player] >= manager.KillsToWin) {
                    if (Network.isServer) {
                        manager.AddToChat(GameManager.connectedPlayers[player] + " wins!", false);
                    }
                    manager.IsVictor = true;
                    manager.VictorName = GameManager.connectedPlayers[player];
                    manager.GameInProgress = false;
                    roundstarted = false;
                }

            }
        }
    }

    IEnumerator CheckForGameEnd() {
        float waitTime = 1.0f;
        
        while(true){
            yield return new WaitForSeconds(waitTime);
            if (Time.time >= manager.EndTime && manager.GameInProgress) {
                CheckForVictory(true);
                break;
            }
        }

    }
}
