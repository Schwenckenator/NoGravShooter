﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UILobby : MonoBehaviour {

    #region Instance
    //Here is a private reference only this class can access
    private static UILobby _instance;
    //This is the public reference that other classes will use
    public static UILobby instance {
        get {
            //If _instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<UILobby>();
            }
            return _instance;
        }
    }
    #endregion

    public static Text playerList;
    public static ChatBox chat;
    private static Text startButtonText;
    private static Text disconnectButtonText;
    private static Button[] buttons;

    private static bool countdown = false;

	// Use this for initialization
	void Start () {
        LobbyInit();
	}

    void LobbyInit() {
        Canvas lobby = UIManager.GetCanvas(Menu.Lobby);
        chat = lobby.GetComponentInChildren<ChatBox>();
        buttons = lobby.GetComponentsInChildren<Button>();
        startButtonText = buttons[0].GetComponentInChildren<Text>();
        disconnectButtonText = buttons[2].GetComponentInChildren<Text>();
    }
    #region StartGame
    public void StartCountdown() {
        UILobby.instance.StartCountdownStatic();
    }
    private void StartCountdownStatic() {
        if (countdown) {
            StopCoroutine("CountdownStartGame");
            ChatManager.instance.AddToChat("Countdown cancelled.");
            countdown = false;
        } else {
            StartCoroutine("CountdownStartGame");
        }
        ChangeButtonText(countdown);
    }
    IEnumerator CountdownStartGame() {
        if (DebugManager.IsAdminMode()) {
            GameManager.instance.LoadLevel();
            yield break;
        }
        countdown = true;
        int waitSeconds = 5;
        ChatManager.instance.AddToChat("Game starting in...");
        do {
            ChatManager.instance.AddToChat(waitSeconds.ToString() + "...");
            yield return new WaitForSeconds(1.0f);
        } while (waitSeconds-- > 0);
        GameManager.instance.LoadLevel();
        countdown = false;
    }
    #endregion

    private void ModifyLobby() {
        ChangeButtonText(countdown);
        ChangeDisconnectButtonText();
    }

    private void ChangeButtonText(bool count) {
        startButtonText.text = count ? "Cancel" : "Start Game";
    }
    private void ChangeDisconnectButtonText() {
        disconnectButtonText.text = Network.isServer ? "Shutdown Server" : "Disconnect";
    }
    public void ShowHideButtons() {
        bool show = Network.isServer;
        buttons[0].enabled = show;
        buttons[1].enabled = show;
        
    }

    public void Disconnect() {
        UILobby.instance.DisconnectStatic();
    }
    private void DisconnectStatic() {
        NetworkManager.Disconnect();
        if (countdown) StopCoroutine("CountdownStartGame");
        countdown = false;
    }
}
