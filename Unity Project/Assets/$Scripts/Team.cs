using UnityEngine;
using System.Collections;

public enum TeamColour {
    None,
    Red,
    Blue
}
//Add new colours if needed.

public class Team {

    public TeamColour Type{ get; private set; }
    public string Name { get; private set; }
    public int Score { get; private set; }

    public Team(TeamColour team) {
        if (team == TeamColour.None) {
            throw new System.ArgumentException("None is not a valid team type");
        }
        Type = team;
        this.Name = "Team " + this.Type.ToString();
        this.Score = 0;
    }

    public int GetScore() {
        return this.Score;
    }
    public void AddScore(int addAmount) {
        this.Score += addAmount;
    }
    public void MinusScore(int minusAmount) {
        this.Score -= minusAmount;
    }
    public bool IsScoreEqualOrOverAmount(int amount) {
        return this.Score >= amount;
    }
    public void ClearScore() {
        this.Score = 0;
    }
}
