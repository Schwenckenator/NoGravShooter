using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreAndVictoryTracker : MonoBehaviour {

    public static Dictionary<NetworkPlayer, int> playerScores = new Dictionary<NetworkPlayer, int>();
    private GameManager manager;

    void Start() {
        manager = GetComponent<GameManager>();
    }

    public void KillScored(NetworkPlayer player) {
        Debug.Log(GameManager.connectedPlayers[player] + "kills");
        playerScores[player] += 1;
        CheckForVictory();
    }

    void CheckForVictory() {
        foreach (NetworkPlayer player in playerScores.Keys) {
            if (playerScores[player] >= manager.KillsToWin) {
                manager.AddToChat(GameManager.connectedPlayers[player] + " wins!", false);
                manager.IsVictor = true;
                manager.VictorName = GameManager.connectedPlayers[player];
            }

        }
    }
}
