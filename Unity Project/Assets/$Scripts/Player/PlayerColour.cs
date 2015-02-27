using UnityEngine;
using System.Collections;

public class PlayerColour : MonoBehaviour {

    public Material[] playerColours;
    public string playerGraphicsName = "Test_Rig";
	
	private SettingsManager settingsManager;

    private int pickedColour = -1;
	
	private float RedVal;
	private float GreenVal;
	private float BlueVal;
	
	
	/// <summary>
    /// If colour is not picked, pick colour.
    /// Apply colour to player
    /// </summary>
    private void AssignPlayerColour() {
        if (pickedColour < 0) { // Need to pick colour
            pickedColour = PickColourRandom();
        }
        settingsManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<SettingsManager>();

        networkView.RPC("RPCAssignPlayerColour", RPCMode.AllBuffered, settingsManager.ColourR, settingsManager.ColourG, settingsManager.ColourB);
    }

    [RPC]
    private void RPCAssignPlayerColour(float RedVal, float GreenVal, float BlueVal) {
		transform.FindChild(playerGraphicsName).renderer.material.color = new Color(RedVal, GreenVal, BlueVal);
    }

    public void ResetPickedColour() {
        pickedColour = -1;
    }

    public int PickColourRandom() {
        return Random.Range(0, playerColours.Length);
    }
    public void SetPickedColour(int value) {
        pickedColour = value;
        AssignPlayerColour();
    }
}
