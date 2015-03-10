using System;
using System.Collections.Generic;
using UnityEngine;

public enum Team {
    None,
    Red,
    Blue
}
//Add new colours if needed.

public class Player {
    public NetworkPlayer ID { get; protected set; }
    public string Name { get; protected set; }
    public int Score { get; protected set; }
    public Team Team { get; protected set; }

    public Player(NetworkPlayer id, string name) {
        this.ID = id;
        this.Name = name;
        this.Score = 0;
        this.Team = Team.None;
    }

    // Score
    public bool IsScoreEqualOrOver(int value) {
        return (this.Score >= value);
    }
    public void AddScore(int value) {
        this.Score += value;
    }
    public void MinusScore(int value) {
        this.Score -= value;
    }
    public void ClearScore() {
        this.Score = 0;
    }

    // Team
    public void ChangeTeam(Team newTeam, bool sendRPC = true) {
        this.Team = newTeam;

        if (sendRPC) { // Sends RPC by default
            NetworkManager.instance.PlayerChangedTeam(this, newTeam);
        }
    }
    public void ChangeTeam(bool sendRPC = true) {
        if (SettingsManager.instance.IsTeamGameMode()) {
            SwapTeam();
        } else {
            this.Team = Team.None;
        }

        if (sendRPC) { // Sends RPC by default
            NetworkManager.instance.PlayerChangedTeam(this, this.Team);
        }
    }
    private void SwapTeam() {
        if (this.Team == Team.Red) this.Team = Team.Blue;
        else this.Team = Team.Red;
    }
    /// <summary>
    /// Returns false if this.Team = Team.None
    /// </summary>
    /// <param name="team"></param>
    /// <returns></returns>
    public bool IsOnTeam(Team team) {
        return this.Team != Team.None && this.Team == team;
    }
    public bool HasNoTeam() {
        return this.Team == Team.None;
    }
}
