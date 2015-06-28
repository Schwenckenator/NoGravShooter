using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ServerListEntry : MonoBehaviour {

    public Text Name;
    public Text Status;
    public Text PlayerCount;
    public Button JoinServer;

    public HostData hostData;

    private bool pressed;

    public bool IsPressed() {
        return pressed;
    }
    public void ResetButton() {
        pressed = false;
    }

    public void JoinButtonPressed() {
        pressed = true;
        UIJoinGame.instance.JoinButtonPressed();
    }

}
