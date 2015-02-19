using System;
using System.Collections.Generic;
using UnityEngine;

public class Player {
    public NetworkPlayer ID { get; protected set; }
    public string Name { get; protected set; }
    public int Score { get; private set; }

    public Player(NetworkPlayer id, string name) {
        this.ID = id;
        this.Name = name;
        this.Score = 0;
    }
    
    public bool IsScoreEqualOrOver(int value) {
        return (this.Score >= value);
    }

    public bool Equals(NetworkPlayer value){
        return (this.ID == value);
    }

    // Score
    public void AddScore(int value) {
        this.Score += value;
    }
    public void MinusScore(int value) {
        this.Score -= value;
    }
    public void ClearScore() {
        this.Score = 0;
    }
}
