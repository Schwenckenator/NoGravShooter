using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking.Match;

public class ServerListEntry : MonoBehaviour {

    public Text Name;
    public Text Status;
    public Text PlayerCount;
    public Button JoinServer;

    public MatchDesc match;

    private bool pressed;

    public bool IsPressed() {
        return pressed;
    }
    public void ResetButton() {
        pressed = false;
    }

    public void JoinButtonPressed() {
        pressed = true;
        UIJoinGame.singleton.JoinButtonPressed();
    }

}
