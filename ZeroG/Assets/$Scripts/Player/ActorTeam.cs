using UnityEngine;
using System.Collections;

/// <summary>
/// On the Actor. Stores the team value for radar
/// </summary>
public class ActorTeam : MonoBehaviour {

    private TeamColour team;

    public TeamColour GetTeam(){
        return team;
    }
    public void SetTeam(TeamColour newTeam) {
        team = newTeam;
        networkView.RPC("RPCSetTeam", RPCMode.OthersBuffered, (int)newTeam);
    }
    [RPC]
    private void RPCSetTeam(int newTeam) {
        team = (TeamColour)newTeam;
    }

    NetworkView networkView;
    void Start() {
        networkView = GetComponent<NetworkView>();
    }
}
