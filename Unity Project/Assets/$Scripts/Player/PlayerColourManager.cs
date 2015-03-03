using UnityEngine;
using System.Collections;

public class PlayerColourManager : MonoBehaviour {
    public void AssignColour(GameObject player) {
        PlayerColour playerColour = player.GetComponent<PlayerColour>();

        playerColour.AssignPlayerColour();
    }

}
