using UnityEngine;
using System.Collections;

public class PlayerColour : MonoBehaviour {

    public Material[] playerColours;
    public string playerGraphicsName = "Test_Rig";
	
	private SettingsManager settingsManager;
	
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
        settingsManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<SettingsManager>();
        networkView.RPC("RPCAssignPlayerColour", RPCMode.AllBuffered, settingsManager.ColourR, settingsManager.ColourG, settingsManager.ColourB);
    }
    private void ApplyTeamColour(Team team) {
        float red = 0;
        float green = 0;
        float blue = 0;
        switch (team) {
            case Team.Red:
                red = 1;
                green = 0;
                blue = 0;
                break;
            case Team.Blue:
                red = 0;
                green = 0;
                blue = 1;
                break;
        }
        networkView.RPC("RPCAssignPlayerColour", RPCMode.AllBuffered, red, green, blue);
    }

    [RPC]
    private void RPCAssignPlayerColour(float RedVal, float GreenVal, float BlueVal) {
		transform.FindChild(playerGraphicsName).renderer.material.color = new Color(RedVal, GreenVal, BlueVal);
    }
}