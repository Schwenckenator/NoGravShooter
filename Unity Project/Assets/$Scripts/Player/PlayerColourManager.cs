using UnityEngine;
using System.Collections;

public class PlayerColourManager : MonoBehaviour {


    private int playerColourIndex = -1;


    public void AssignColour(GameObject player) {
        PlayerColour playerColour = player.GetComponent<PlayerColour>();

        playerColour.SetPickedColour(playerColourIndex);
    }

    public int PickColour(PlayerColour playerColour) {
        // abstracted for teams later
        return playerColour.PickColourRandom();
    }

}
