using UnityEngine;
using System.Collections;

public class PlayerColour : MonoBehaviour {

    public string playerGraphicsName = "Test_Rig";

    //NetworkView //NetworkView;
    void Awake() {
        //NetworkView = GetComponent<//NetworkView>();
    }
	
    public void AssignPlayerColour() {
        LobbyPlayer currentPlayer = NetworkManager.MyPlayer();
        
        // No need to check for team
        ApplyColour(currentPlayer.Team);

    }
    private void ApplyColour(TeamColour team) {
		//just an idea i had while watching redvsblue, can always take it out later
		//allows multiple shades while keeping within the general colour pallet of the team

        //Color newColour = PlayerColourManager.singleton.LimitTeamColour(team, SettingsManager.singleton.GetPlayerColour());

        //NetworkView.RPC("RPCAssignPlayerColour", RPCMode.AllBuffered, newColour.r, newColour.g, newColour.b);
    }

    //[RPC]
    private void RPCAssignPlayerColour(float RedVal, float GreenVal, float BlueVal) {
		transform.FindChild(playerGraphicsName).GetComponent<Renderer>().material.color = new Color(RedVal, GreenVal, BlueVal);
    }
}