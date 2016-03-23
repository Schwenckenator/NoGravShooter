using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerListEntry : MonoBehaviour {
    public Player myPlayer;
    public Text myNameText;
    public Text myReadyText;
	// Use this for initialization
	void Start () {
        UpdateName();   
	}
	
    public void UpdateName() {
        myNameText.text = myPlayer.Name;
    }

    public void UpdateToggle(bool ready) {
        UpdateReadyText(ready);
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
}
