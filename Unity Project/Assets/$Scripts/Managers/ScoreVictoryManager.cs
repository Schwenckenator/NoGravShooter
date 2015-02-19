﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreVictoryManager : MonoBehaviour {

    //public static Dictionary<NetworkPlayer, int> playerScores = new Dictionary<NetworkPlayer, int>();
    private GameManager gameManager;
    private GuiManager guiManager;
    private SettingsManager settingsManager;
    private ChatManager chatManager;
	
    public bool IsVictor { get; set; }
    public string VictorName { get; set; }
	

    void Start() {
        gameManager = GetComponent<GameManager>();
        guiManager = GetComponent<GuiManager>();
        settingsManager = GetComponent<SettingsManager>();
        chatManager = GetComponent<ChatManager>();
    }
    public void GameStart() {
        StartCoroutine(CheckForGameEnd());
    }

    public void PointScored(NetworkPlayer player) {
        Debug.Log(NetworkManager.GetPlayer(player).Name + " kills");
        networkView.RPC("RPCKillScored", RPCMode.AllBuffered, player, 1);
    }
    public void Suicide(NetworkPlayer player) {
        networkView.RPC("RPCKillScored", RPCMode.AllBuffered, player, -1);
    }

    [RPC]
    private void RPCKillScored(NetworkPlayer player, int score) {
        if (!NetworkManager.DoesPlayerExist(player)) return;

        NetworkManager.GetPlayer(player).AddScore(score);
        CheckForScoreVictory();
        guiManager.SetScoreBoardText(ScoreVictoryManager.UpdateScoreBoard()); 
    }

    void CheckForScoreVictory() {
        foreach (Player player in NetworkManager.connectedPlayers) {
            if (player.IsScoreEqualOrOver(settingsManager.ScoreToWin)) {
                if (Network.isServer) {
                    chatManager.AddToChat(player.Name + " wins!", false);
                }
                gameManager.EndGame(player);
                break;
            }

        }
    }
    Player winningPlayer;
    void TimeVictory() {
        int maxValue = -999999999; // Very negative number
        foreach (Player player in NetworkManager.connectedPlayers) {
            if (player.Score > maxValue) {
                maxValue = player.Score;
                winningPlayer = player;
            }
        }
        if (Network.isServer) {
            chatManager.AddToChat("Time is up.", false);
            chatManager.AddToChat(winningPlayer.Name + " wins!", false);
        }
        gameManager.EndGame(winningPlayer);
    }

    IEnumerator CheckForGameEnd() {
        float waitTime = 1.0f;
        
        while(gameManager.GameInProgress){
            yield return new WaitForSeconds(waitTime);
            if (Time.time >= gameManager.endTime && gameManager.GameInProgress) {
                TimeVictory();
                break;
            }
        }

    }

    public static string UpdateScoreBoard() {
        List<Player> playerBuffer = new List<Player>();
        string scoreBoardBuffer = "";

        // Sort the players descending score
        foreach (Player player in NetworkManager.connectedPlayers) {
            int score = player.Score;
            int i = 0;
            foreach(Player buffer in playerBuffer) {
                if (score > buffer.Score) {
                    break;
                }
                i++;
            }
            playerBuffer.Insert(i, player);
        }

        foreach (Player player in playerBuffer) {
            scoreBoardBuffer += player.Name + ": " + player.Score + "\n";
        }

        return scoreBoardBuffer;

    }

    public void ClearWinnerData() {
        IsVictor = false;
        VictorName = "";
    }
}
