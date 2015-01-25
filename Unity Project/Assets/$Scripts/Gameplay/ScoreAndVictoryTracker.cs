using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreAndVictoryTracker : MonoBehaviour {

    public static Dictionary<NetworkPlayer, int> playerScores = new Dictionary<NetworkPlayer, int>();
    private GameManager manager;
	
	private NetworkPlayer winningPlayer;
	

    void Start() {
        manager = GetComponent<GameManager>();
    }
    public void GameStart() {
        StartCoroutine(CheckForGameEnd());
    }

    public void KillScored(NetworkPlayer player) {
        Debug.Log(GameManager.connectedPlayers[player] + " kills");
        networkView.RPC("RPCKillScored", RPCMode.AllBuffered, player);
    }

    [RPC]
    private void RPCKillScored(NetworkPlayer player) {
        playerScores[player] += 1;
        CheckForScoreVictory();
        manager.GetComponent<GUIScript>().UpdateScoreBoard();
    }

    void CheckForScoreVictory() {
        foreach (NetworkPlayer player in playerScores.Keys) {
            if (playerScores[player] >= manager.KillsToWin) {
                if (Network.isServer) {
                    manager.AddToChat(GameManager.connectedPlayers[player] + " wins!", false);
                }
                manager.EndGame(player);
                break;
            }

        }
    }

    void TimeVictory() {
        int maxValue = -1;
        foreach (NetworkPlayer player in playerScores.Keys) {
            if (playerScores[player] > maxValue) {
                maxValue = playerScores[player];
                winningPlayer = player;
            }
        }
        if (Network.isServer) {
            manager.AddToChat("Time is up.", false);
            manager.AddToChat(GameManager.connectedPlayers[winningPlayer] + " wins!", false);
        }
        manager.EndGame(winningPlayer);
    }

    IEnumerator CheckForGameEnd() {
        float waitTime = 1.0f;
        
        while(manager.GameInProgress){
            yield return new WaitForSeconds(waitTime);
            if (Time.time >= manager.EndTime && manager.GameInProgress) {
                TimeVictory();
                break;
            }
        }

    }
}
