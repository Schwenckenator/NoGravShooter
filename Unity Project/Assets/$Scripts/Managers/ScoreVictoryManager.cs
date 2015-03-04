using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreVictoryManager : MonoBehaviour {

    #region Instance
    //Here is a private reference only this class can access
    private static ScoreVictoryManager _instance;
    //This is the public reference that other classes will use
    public static ScoreVictoryManager instance {
        get {
            //If _instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<ScoreVictoryManager>();
            }
            return _instance;
        }
    }
    #endregion

    //public bool IsVictor { get; set; }
    public string VictorName { get; set; }
    public bool IsVictor() {
        return VictorName != "";
    }

    void Start() {
        ClearWinnerData(); // Clean Set up
    }
    public void GameStart() {
        StartCoroutine(CheckForGameEnd());
    }

    public void PointScored(NetworkPlayer player) {
        networkView.RPC("RPCPointScored", RPCMode.All, player, 1);
    }
    public void PointLost(NetworkPlayer player) {
        networkView.RPC("RPCPointScored", RPCMode.All, player, -1);
    }

    void OnPlayerConnected(NetworkPlayer connectingPlayer) {
        foreach (Player player in NetworkManager.connectedPlayers) {
            networkView.RPC("RPCPointScored", connectingPlayer, player.ID, player.Score);
        }
    }

    [RPC]
    private void RPCPointScored(NetworkPlayer player, int score) {
        ChatManager.DebugMessage("RPCPointScored called");

        if (!NetworkManager.DoesPlayerExist(player)) {
            ChatManager.DebugMessage("Can't find Player "+player.ToString());
        }

        ChatManager.DebugMessage("RPCPointScored Player exists, will add point.");

        NetworkManager.GetPlayer(player).AddScore(score);
        CheckForScoreVictory();
        GuiManager.instance.SetScoreBoardText(ScoreVictoryManager.UpdateScoreBoard()); 
    }

    void CheckForScoreVictory() {
        foreach (Player player in NetworkManager.connectedPlayers) {
            if (player.IsScoreEqualOrOver(SettingsManager.instance.ScoreToWin)) {
                if (Network.isServer) {
                    ChatManager.instance.AddToChat(player.Name + " wins!");
                }
                GameManager.instance.EndGame(player);
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
            ChatManager.instance.AddToChat("Time is up.");
            ChatManager.instance.AddToChat(winningPlayer.Name + " wins!");
        }
        GameManager.instance.EndGame(winningPlayer);
    }

    IEnumerator CheckForGameEnd() {
        float waitTime = 1.0f;
        
        while(GameManager.instance.GameInProgress){
            yield return new WaitForSeconds(waitTime);
            if (Time.time >= GameManager.instance.endTime && GameManager.instance.GameInProgress) {
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
        VictorName = "";
    }
}
