using UnityEngine;
using System.Collections;

/// <summary>
/// On the Actor. Stores the team value for radar
/// </summary>
public class ActorTeam : MonoBehaviour {

    private Team team;

    public Team GetTeam(){
        return team;
    }
    public void SetTeam(Team newTeam) {
        team = newTeam;
        networkView.RPC("RPCSetTeam", RPCMode.OthersBuffered, (int)newTeam);
    }
    [RPC]
    private void RPCSetTeam(int newTeam) {
        team = (Team)newTeam;
    }
}
