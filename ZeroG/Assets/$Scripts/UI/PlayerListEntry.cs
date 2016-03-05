using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerListEntry : MonoBehaviour {
    public Player myPlayer;
    public Text myText;
	// Use this for initialization
	void Start () {
        UpdateName();   
	}
	
    public void UpdateName() {
        myText.text = myPlayer.Name;
    }
}
