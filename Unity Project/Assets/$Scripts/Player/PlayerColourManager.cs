using UnityEngine;
using System.Collections;

public class PlayerColourManager : MonoBehaviour {


    private int playerColourIndex = -1;

    void OnLevelWasLoaded() {
        ResetColourIndex();
    }

    public void AssignColour(GameObject player) {
        PlayerColour playerColour = player.GetComponent<PlayerColour>();
        
        if (!IsColourPicked()) { // Needs picking
            playerColourIndex = PickColour(playerColour);
        }

        playerColour.SetPickedColour(playerColourIndex);
    }

    public int PickColour(PlayerColour playerColour) {
        // abstracted for teams later
        return playerColour.PickColourRandom();
    }

    public void ResetColourIndex() {
        playerColourIndex = -1;
    }

    private bool IsColourPicked() {
        return playerColourIndex != -1;
    }
}
