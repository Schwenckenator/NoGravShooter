using System;
using System.Collections.Generic;
using UnityEngine;



public class Player {
    public NetworkPlayer ID { get; protected set; }
    public string Name { get; protected set; }
    public int Score { get; protected set; }
    public TeamColour Team { get; protected set; }

    public Player(NetworkPlayer id, string name) {
        this.ID = id;
        this.Name = name;
        this.Score = 0;
        this.Team = TeamColour.None;
    }
    public Player() {
        this.Name = "";
        this.Score = 0;
        this.Team = TeamColour.None;
    }

    #region Score
    public bool IsScoreEqualOrOverAmount(int amount) {
        return (this.Score >= amount);
    }
    public void AddScore(int addAmount) {
        this.Score += addAmount;
    }
    public void MinusScore(int minusAmount) {
        this.Score -= minusAmount;
    }
    public void ClearScore() {
        this.Score = 0;
    }
    #endregion

    #region Team
    public void ChangeTeam(TeamColour newTeam, bool sendRPC = true) {
        this.Team = newTeam;

        if (sendRPC) { // Sends RPC by default
            NetworkManager.singleton.PlayerChangedTeam(this, newTeam);
        }
    }
    public void ChangeTeam(bool sendRPC = true) {
        if (SettingsManager.singleton.IsTeamGameMode()) {
            SwapTeam();
        } else {
            this.Team = TeamColour.None;
        }

        if (sendRPC) { // Sends RPC by default
            NetworkManager.singleton.PlayerChangedTeam(this, this.Team);
        }
    }
    private void SwapTeam() {
        if (this.Team == TeamColour.Red) this.Team = TeamColour.Blue;
        else this.Team = TeamColour.Red;
    }
    /// <summary>
    /// Returns false if this.Team = Team.None
    /// </summary>
    /// <param name="team"></param>
    /// <returns>If team == this.team true</returns>
    public bool IsOnTeam(TeamColour team) {
        return this.Team != TeamColour.None && this.Team == team;
    }
    public bool HasNoTeam() {
        return this.Team == TeamColour.None;
    }
    #endregion
}
