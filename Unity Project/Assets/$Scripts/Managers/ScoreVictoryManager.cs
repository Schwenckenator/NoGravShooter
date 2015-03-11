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

    public string VictorName { get; set; }
    public bool IsVictor() {
        return VictorName != "";
    }

    public List<Team> Teams = new List<Team>();
    Team GetTeam(TeamColour colour) {
        return Teams.Find(x => x.Type.Equals(colour));
    }

    void Start() {
        ClearScoreData(); // Clean Set up
        Teams.Add(new Team(TeamColour.Red));
        Teams.Add(new Team(TeamColour.Blue));
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
    private void RPCPointScored(NetworkPlayer playerID, int score) {
        ChatManager.DebugMessage("RPCPointScored called");

        if (!NetworkManager.DoesPlayerExist(playerID)) {
            ChatManager.DebugMessage("Can't find Player "+playerID.ToString());
        }

        ChatManager.DebugMessage("RPCPointScored Player exists, will add point.");
        
        if (score != 0) { // If 0, don't bother
            Player player = NetworkManager.GetPlayer(playerID);
            player.AddScore(score);
            if (SettingsManager.instance.IsTeamGameMode()) {
                GetTeam(player.Team).AddScore(score);
            }
            CheckForScoreVictory();
        }

        GuiManager.instance.SetScoreBoardText(ScoreVictoryManager.UpdateScoreBoard()); 
    }

    void CheckForScoreVictory() {
        if (SettingsManager.instance.IsTeamGameMode()) {
            TeamCheckForScoreVictory();
        } else {
            FFACheckForScoreVictory();
        }
    }
    // Bad bad code duplication. Bad Matt! *smack*
    void TeamCheckForScoreVictory() {
        foreach (Team team in Teams) {
            if (team.IsScoreEqualOrOverAmount(SettingsManager.instance.ScoreToWin)) {
                if (Network.isServer) {
                    ChatManager.instance.AddToChat(team.Name + " wins!");
                }
                GameManager.instance.EndGame();
                this.VictorName = team.Name;
                break;
            }
        }
    }
    void FFACheckForScoreVictory() {
        foreach (Player player in NetworkManager.connectedPlayers) {
            if (player.IsScoreEqualOrOverAmount(SettingsManager.instance.ScoreToWin)) {
                if (Network.isServer) {
                    ChatManager.instance.AddToChat(player.Name + " wins!");
                }
                GameManager.instance.EndGame();
                this.VictorName = player.Name;
                break;
            }

        }
    }
    
    void TimeVictory() {

        string winningName = WinningName();
        if (Network.isServer) {
            ChatManager.instance.AddToChat("Time is up.");
            ChatManager.instance.AddToChat(winningName + " wins!");
        }
        GameManager.instance.EndGame();
        this.VictorName = winningName;
    }

    private string WinningName() {
        int maxValue = -999999999; // Very negative number
        string winningName = "";
        if (SettingsManager.instance.IsTeamGameMode()) { // Bad code duplication
            foreach (Team team in Teams) {
                if (team.Score > maxValue) {
                    maxValue = team.Score;
                    winningName = team.Name;
                } else if (team.Score == maxValue) {
                    winningName += " and " + team.Name;
                }
            }
        } else {
            foreach (Player player in NetworkManager.connectedPlayers) {
                if (player.Score > maxValue) {
                    maxValue = player.Score;
                    winningName = player.Name;
                } else if (player.Score == maxValue) {
                    winningName += " and " + player.Name;
                }
            }
        }

        return winningName;
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
        List<Team> teamBuffer = new List<Team>();
        string scoreBoardBuffer = "";

        playerBuffer = SortPlayers();
        teamBuffer = SortTeams();

        if (SettingsManager.instance.IsTeamGameMode()) {
            foreach (Team team in teamBuffer) {
                scoreBoardBuffer += "Team "+ team.Type.ToString() + ": " + team.Score + "\n";
            }
        }
        foreach (Player player in playerBuffer) {
            scoreBoardBuffer += player.Name + ": " + player.Score + "\n";
        }

        return scoreBoardBuffer;

    }

    private static List<Player> SortPlayers() {
        List<Player> playerBuffer = new List<Player>();
        // Sort the players descending score
        foreach (Player player in NetworkManager.connectedPlayers) {
            int score = player.Score;
            int i = 0;
            foreach (Player buffer in playerBuffer) {
                if (score > buffer.Score) {
                    break;
                }
                i++;
            }
            playerBuffer.Insert(i, player);
        }
        return playerBuffer;
    }
    private static List<Team> SortTeams() {
        List<Team> teamBuffer = new List<Team>();

        foreach (Team team in ScoreVictoryManager.instance.Teams) {
            int score = team.Score;
            int i = 0;
            foreach (Team buffer in teamBuffer) {
                if (score > buffer.Score) {
                    break;
                }
                i++;
            }
            teamBuffer.Insert(i, team);
        }

        return teamBuffer;
    }

    public void ClearScoreData() {
        VictorName = "";
        // Keep the players, but wipe the scores
        foreach (Player player in NetworkManager.connectedPlayers) {
            player.ClearScore();
        }
        foreach (Team team in Teams) {
            team.ClearScore();
        }
    }
}
