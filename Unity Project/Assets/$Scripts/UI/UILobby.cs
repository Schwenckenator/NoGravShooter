using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UILobby : MonoBehaviour {

    private Text playerList;
    private Text chat;

	// Use this for initialization
	void Start () {
        LobbyInit();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void LobbyInit() {
        Canvas lobby = UIManager.GetCanvas(Menu.Lobby);

    }
}
