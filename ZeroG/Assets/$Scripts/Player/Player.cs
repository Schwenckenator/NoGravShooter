using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public struct PlayerInfo {
    public int id;
    public string name;
    public int team;

    public PlayerInfo(int id, string name, int team) {
        this.id = id;
        this.name = name;
        this.team = team;
    }
}

public class Player : NetworkBehaviour{
    [SyncVar]
    public int Score;
    [SyncVar]
    public PlayerInfo info;

    public TeamColour Team; // Shut up errors

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
        info.team = (int)newTeam;
    }
    public void ChangeTeam(bool sendRPC = true) {
        if (SettingsManager.singleton.IsTeamGameMode()) {
            SwapTeam();
        } else {
            info.team = (int)TeamColour.None;
        }

        if (sendRPC) { // Sends RPC by default
            NetworkManager.single.PlayerChangedTeam(this, (TeamColour)info.team);
        }
    }
    private void SwapTeam() {
        if ((TeamColour)info.team == TeamColour.Red) info.team = (int)TeamColour.Blue;
        else info.team = (int)TeamColour.Red;
    }
    /// <summary>
    /// Returns false if this.Team = Team.None
    /// </summary>
    /// <param name="team"></param>
    /// <returns>If team == this.team true</returns>
    public bool IsOnTeam(TeamColour team) {
        return (TeamColour)info.team != TeamColour.None && (TeamColour)info.team == team;
    }
    public bool HasNoTeam() {
        return (TeamColour)info.team == TeamColour.None;
    }
    #endregion

    public override void OnStartServer() {
        base.OnStartServer();
        info.id = NetworkManager.GetNextID();
        NetworkInfoWrapper.singleton.AddPlayer(info);
    }
    public override void OnStartLocalPlayer() {
        Debug.Log("Player started");
        CmdSetName(SettingsManager.singleton.PlayerName);
        NetworkManager.myPlayer = this;
    }

    [Command]
    private void CmdSetName(string name) {
        info.name = name;
        NetworkInfoWrapper.singleton.UpdateInfo(info);
    }
    [Command]
    public void CmdChangeTeam(int newTeam) {
        if (SettingsManager.singleton.IsTeamGameMode()) {
            info.team = newTeam;
        } else {
            info.team = 0;
        }
        //OnTeamChange(info.team);
    }

    private void OnTeamChange(int newTeam) {
        info.team = newTeam;
    }

    [Command]
    public void CmdSendChatMessage(string message, bool addPrefix) {
        Debug.Log("CmdSendChatMessage called");
        string newMessage = "";
        if (addPrefix) {
            newMessage += info.name + ": ";
        }
        newMessage += message;

        NetworkInfoWrapper.singleton.RpcChatMessage(newMessage);
    }
}
