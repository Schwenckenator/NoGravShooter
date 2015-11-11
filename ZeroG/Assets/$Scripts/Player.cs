using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour{
    [SyncVar]
    public int ID;
    [SyncVar]
    public string Name;
    [SyncVar]
    public int Score;
    [SyncVar(hook ="OnTeamChange")]
    private int i_Team;
    public TeamColour Team {get; private set;}

    #region Score
    public bool IsScoreEqualOrOverAmount(int amount) {
        return (Score >= amount);
    }
    [Server]
    public void AddScore(int addAmount) {
        Score += addAmount;
    }
    [Server]
    public void MinusScore(int minusAmount) {
        Score -= minusAmount;
    }
    [Server]
    public void ClearScore() {
        Score = 0;
    }
    #endregion

    #region Team
    public void ChangeTeam(TeamColour newTeam) {
        Team = newTeam;
    }
    public void ChangeTeam(bool sendRPC = true) {
        if (SettingsManager.singleton.IsTeamGameMode()) {
            SwapTeam();
        } else {
            Team = TeamColour.None;
        }

        if (sendRPC) { // Sends RPC by default
            NetworkManager.single.PlayerChangedTeam(this, Team);
        }
    }
    private void SwapTeam() {
        if (Team == TeamColour.Red) Team = TeamColour.Blue;
        else Team = TeamColour.Red;
    }
    /// <summary>
    /// Returns false if this.Team = Team.None
    /// </summary>
    /// <param name="team"></param>
    /// <returns>If team == this.team true</returns>
    public bool IsOnTeam(TeamColour team) {
        return Team != TeamColour.None && Team == team;
    }
    public bool HasNoTeam() {
        return Team == TeamColour.None;
    }
    #endregion

    public override void OnStartLocalPlayer() {
        CmdSetName(SettingsManager.singleton.PlayerName);
        NetworkManager.myPlayer = this;
    }

    [Command]
    private void CmdSetName(string name) {
        Name = name;
        NetworkInfoWrapper.singleton.AddPlayerName(name);
    }
    [Command]
    public void CmdChangeTeam(int newTeam) {
        if (SettingsManager.singleton.IsTeamGameMode()) {
            i_Team = newTeam;
        } else {
            i_Team = 0;
        }
        OnTeamChange(i_Team);
    }

    private void OnTeamChange(int newTeam) {
        i_Team = newTeam;
        Team = (TeamColour)i_Team;
    }

    [Command]
    public void CmdSendChatMessage(string message, bool addPrefix) {
        string newMessage = "";
        if (addPrefix) {
            newMessage += Name + ": ";
        }
        newMessage += message;

        NetworkInfoWrapper.singleton.RpcChatMessage(newMessage);
    }
}
