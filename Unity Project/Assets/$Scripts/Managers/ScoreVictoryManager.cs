using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreVictoryManager : MonoBehaviour {

    public static Dictionary<NetworkPlayer, int> playerScores = new Dictionary<NetworkPlayer, int>();
    private GameManager manager;
    private GuiManager guiManager;
	
	private NetworkPlayer winningPlayer;
	

    void Start() {
        manager = GetComponent<GameManager>();
        guiManager = GetComponent<GuiManager>();
    }
    public void GameStart() {
        StartCoroutine(CheckForGameEnd());
    }

    public void KillScored(NetworkPlayer player) {
        Debug.Log(NetworkManager.connectedPlayers[player] + " kills");
        networkView.RPC("RPCKillScored", RPCMode.AllBuffered, player);
    }

    [RPC]
    private void RPCKillScored(NetworkPlayer player) {
        playerScores[player] += 1;
        CheckForScoreVictory();
        guiManager.SetScoreBoardText(ScoreVictoryManager.UpdateScoreBoard()); 
    }

    void CheckForScoreVictory() {
        foreach (NetworkPlayer player in playerScores.Keys) {
            if (playerScores[player] >= manager.KillsToWin) {
                if (Network.isServer) {
                    manager.AddToChat(NetworkManager.connectedPlayers[player] + " wins!", false);
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
            manager.AddToChat(NetworkManager.connectedPlayers[winningPlayer] + " wins!", false);
        }
        manager.EndGame(winningPlayer);
    }

    IEnumerator CheckForGameEnd() {
        float waitTime = 1.0f;
        
        while(manager.GameInProgress){
            yield return new WaitForSeconds(waitTime);
            if (Time.time >= manager.endTime && manager.GameInProgress) {
                TimeVictory();
                break;
            }
        }

    }

    public static string UpdateScoreBoard() {
        List<NetworkPlayer> playerBuffer = new List<NetworkPlayer>();
        string scoreBoardBuffer = "";

        // Sort the players descending score
        foreach (NetworkPlayer player in NetworkManager.connectedPlayers.Keys) {
            int score = ScoreVictoryManager.playerScores[player];
            int i;
            for (i = 0; i < playerBuffer.Count; i++) {
                if (score > ScoreVictoryManager.playerScores[playerBuffer[i]]) {
                    break;
                }

            }
            playerBuffer.Insert(i, player);
        }

        foreach (NetworkPlayer player in playerBuffer) {
            scoreBoardBuffer += NetworkManager.connectedPlayers[player] + ": " + ScoreVictoryManager.playerScores[player] + "\n";
        }

        return scoreBoardBuffer;

    }
}
