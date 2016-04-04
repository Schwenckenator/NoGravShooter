//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Networking;

//public class PlayerInfo : NetworkLobbyPlayer {
//    [SyncVar]
//    public int ID;
//    [SyncVar(hook = "OnName")]
//    public string Name;
//    [SyncVar(hook = "OnITeam")]
//    private int i_Team;
//    [SyncVar]
//    public int Score;

//    public bool isMine = false;
//    public TeamColour Team; // Shut up errors

//    public PlayerListEntry myUI;


//    public bool debug = true;
//    private Logger log;

//    void Awake() {
//        log = new Logger(debug);
//    }

//    void Start() {
//        GameObject list = GameObject.FindGameObjectWithTag("PlayerList");
//        myUI.transform.SetParent(list.transform, false);
//    }

//    #region Score
//    public bool IsScoreEqualOrOverAmount(int amount) {
//        return (Score >= amount);
//    }
//    [Server]
//    public void AddScore(int addAmount) {
//        Score += addAmount;
//    }
//    [Server]
//    public void MinusScore(int minusAmount) {
//        Score -= minusAmount;
//    }
//    [Server]
//    public void ClearScore() {
//        Score = 0;
//    }
//    #endregion

//    #region Team
//    public void ChangeTeam(TeamColour newTeam) {
//        Team = newTeam;
//    }
//    public void ChangeTeam(bool sendRPC = true) {
//        if (SettingsManager.singleton.IsTeamGameMode()) {
//            SwapTeam();
//        } else {
//            Team = TeamColour.None;
//        }

//        if (sendRPC) { // Sends RPC by default
//            NetworkManager.single.PlayerChangedTeam(this, Team);
//        }
//    }
//    private void SwapTeam() {
//        if (Team == TeamColour.Red) Team = TeamColour.Blue;
//        else Team = TeamColour.Red;
//    }
//    /// <summary>
//    /// Returns false if this.Team = Team.None
//    /// </summary>
//    /// <param name="team"></param>
//    /// <returns>If team == this.team true</returns>
//    public bool IsOnTeam(TeamColour team) {
//        return Team != TeamColour.None && Team == team;
//    }
//    public bool HasNoTeam() {
//        return Team == TeamColour.None;
//    }
//    #endregion

//    public override void OnStartServer() {
//        base.OnStartServer();
//        ID = slot;
//        NetworkManager.connectedPlayers.Add(this);
//    }
//    public override void OnStartLocalPlayer() {
//        log.Log("Player started");
//        CmdSetName(SettingsManager.singleton.PlayerName);
//        NetworkManager.myPlayer = this;
//        isMine = true;
//    }

//    [Command]
//    private void CmdSetName(string name) {
//        Name = name;
//        gameObject.name = "Lobby" + name;
//    }
//    [Command]
//    public void CmdChangeTeam(int newTeam) {
//        if (SettingsManager.singleton.IsTeamGameMode()) {
//            i_Team = newTeam;
//        } else {
//            i_Team = 0;
//        }
//    }

//    [Command]
//    public void CmdSendChatMessage(string message, bool addPrefix) {

//        log.Log("CmdSendChatMessage called");
//        string newMessage = "";
//        if (addPrefix) {
//            newMessage += Name + ": ";
//        }
//        newMessage += message;

//        NetworkInfoWrapper.singleton.RpcChatMessage(newMessage);
//    }

//    private void OnITeam(int newTeam) {
//        i_Team = newTeam;
//        Team = (TeamColour)newTeam;
//    }
//    private void OnName(string newName) {
//        Name = newName;
//        myUI.UpdateName();
//    }

//    public override string ToString() {
//        string message = "";
//        message += "Player\n";
//        message += "ID: " + ID.ToString() + "\n";
//        message += "Name: " + Name.ToString() + "\n";
//        message += "Team: " + Team.ToString() + "\n";
//        message += "Score: " + Score.ToString() + "\n";
//        message += "Is Mine? " + isMine.ToString() + "\n";
//        message += "Is local? " + isLocalPlayer.ToString() + "\n";
//        message += "Has Authority? " + hasAuthority.ToString();

//        return message;
//    }
//}
