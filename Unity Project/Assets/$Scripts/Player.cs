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
    public void ChangeTeam(Team newTeam) {
        this.Team = newTeam;
    }
    public void IncrementTeam() {
        int numOfTeams = 2;
        this.Team++;
        if ((int)this.Team > numOfTeams) this.Team = 0;
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
