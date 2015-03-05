using UnityEngine;
using System.Collections;

public class PlayerColour : MonoBehaviour {

    public Material[] playerColours;
    public string playerGraphicsName = "Test_Rig";
	
    public void AssignPlayerColour() {
        Player currentPlayer = NetworkManager.GetPlayer(Network.player);
        if (currentPlayer.HasNoTeam()) {
            //Has no team, apply colour from settings
            ApplySettingsColour();
        } else {
            //Has team, apply team colour
            ApplyTeamColour(currentPlayer.Team);
        }

    }
    private void ApplySettingsColour() {
        networkView.RPC("RPCAssignPlayerColour", RPCMode.AllBuffered, SettingsManager.instance.ColourR, SettingsManager.instance.ColourG, SettingsManager.instance.ColourB);
    }
    private void ApplyTeamColour(Team team) {
        float red = SettingsManager.instance.ColourR;
        float green = SettingsManager.instance.ColourG;
        float blue = SettingsManager.instance.ColourB;
		//just an idea i had while watching redvsblue, can always take it out later
		//allows multiple shades while keeping within the general colour pallet of the team
        switch (team) {
            case Team.Red:
				//limits colours to red(duh), light red, dark red, pink and orange
				if(red < 0.85f){red = 0.85f;}
				if(green > 0.5f){green = 0.5f;}
				if(blue > 0.75f){blue = 0.75f;}
                break;
            case Team.Blue:
				//limits colours to blue(duh), light blue, dark blue, purple and teal
				if(red > 0.5f){red = 0.5f;}
				if(green > 0.75f){green = 0.75f;}
				if(blue < 0.85){blue = 0.85f;}
                break;
        }
        networkView.RPC("RPCAssignPlayerColour", RPCMode.AllBuffered, red, green, blue);
    }

    [RPC]
    private void RPCAssignPlayerColour(float RedVal, float GreenVal, float BlueVal) {
		transform.FindChild(playerGraphicsName).renderer.material.color = new Color(RedVal, GreenVal, BlueVal);
    }
}