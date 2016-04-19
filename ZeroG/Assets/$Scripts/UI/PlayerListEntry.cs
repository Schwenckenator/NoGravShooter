using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class PlayerListEntry : MonoBehaviour {
    //public PlayerController myPlayer;
    public Text myNameText;
    public Text myScoreText;
	// Use this for initialization
	void Start () {
        UpdateName();
        UpdateScore();
	}
	
    public void UpdateName() {
        //myNameText.text = myPlayer.Name;
    }

    public void UpdateScore() {
        //myScoreText.text = myPlayer.Score.ToString();
    }
}
