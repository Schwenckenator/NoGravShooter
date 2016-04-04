using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class PlayerListEntry : MonoBehaviour {
    public LobbyPlayer myPlayer;
    public Text myNameText;
    public Text myReadyText;
    public Toggle myReadyToggle;
	// Use this for initialization
	void Start () {
        ClearToggle();
        UpdateName();
        if (!myPlayer.isMine) {
            myReadyToggle.enabled = false;
        }
	}
	
    public void UpdateName() {
        myNameText.text = myPlayer.Name;
    }

    public void UpdateToggle(bool ready) {
        Debug.Log("UpdateToggle!");
        UpdateReadyText(ready);
        if (ready) {
            myPlayer.SendReadyToBeginMessage();
        } else {
            myPlayer.SendNotReadyToBeginMessage();
        }
        
    }

    private void UpdateReadyText(bool isReady) {
        if (isReady) {
            myReadyText.text = "Ready!";
        } else {
            if (myPlayer.isMine) { // Is the player the local client
                myReadyText.text = "Ready?";
            } else {
                myReadyText.text = "";
            }
        }
    }

    void ClearToggle() {
        myReadyToggle.isOn = false;
        UpdateReadyText(false);
    }

    void OnLevelWasLoaded() {
        ClearToggle();
    }
}
