using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class UILobby : MonoBehaviour {

    public static UILobby singleton { get; private set; }

    public Text serverNameText;
    public Text serverSettings;
    public Text disconnectButtonText;

    public Button startGameButton;
    public Button changeTeamButton;

    private Text startButtonText;

    private static bool countdown = false;
    private static bool teamGame = false;

    private static Canvas myCanvas;

	// Use this for initialization
	void Awake () {
        singleton = this;
        myCanvas = GetComponent<Canvas>();

        ShowChangeTeamButton(false);
        startButtonText = startGameButton.GetComponentInChildren<Text>();
	}
    void OnEnable() {
        //myCanvas.enabled = false;
        SetShutdownButtonText();
        ChangeButtonText(false);
        startGameButton.interactable = true;
    }

    private void SetShutdownButtonText() {
        if (NetworkManager.isServer) {
            disconnectButtonText.text = "Shutdown";
        } else {
            disconnectButtonText.text = "Disconnect";
        }
    }

    void Update() {
        if (teamGame != SettingsManager.singleton.IsTeamGameMode()) {
            ShowChangeTeamButton(SettingsManager.singleton.IsTeamGameMode());
        }
    }
    
    void ShowChangeTeamButton(bool show) {
        teamGame = show;
        changeTeamButton.gameObject.SetActive(show);
    }
    #region StartGame
    public void StartCountdown() {
        StartCountdownStatic();
    }
    private void StartCountdownStatic() {
        if (countdown) {
            StopCoroutine("CountdownStartGame");
            ChatManager.singleton.AddToChat("Countdown cancelled.");
            countdown = false;
        } else {
            StartCoroutine("CountdownStartGame");
        }
        ChangeButtonText(countdown);
    }
    private void StopCountdown() {
        if (countdown) {
            StopCoroutine("CountdownStartGame");
            ChatManager.singleton.AddToChat("Countdown cancelled.");
            countdown = false;
        }
        ChangeButtonText(countdown);
    }
    IEnumerator CountdownStartGame() {
        if (DebugManager.adminMode) {
            startGameButton.interactable = false;
            GameManager.singleton.LoadLevel();
            yield break;
        }
        countdown = true;
        int waitSeconds = 5;
        ChatManager.singleton.AddToChat("Game starting in...");
        do {
            ChatManager.singleton.AddToChat(waitSeconds.ToString() + "...");
            yield return new WaitForSeconds(1.0f);
        } while (waitSeconds-- > 0);
        startGameButton.interactable = false;
        GameManager.singleton.LoadLevel();
        countdown = false;
    }
    #endregion

    private void ChangeButtonText(bool count) {
        startButtonText.text = count ? "Cancel" : "Start Game";
    }
    public void ChangeTeam() {
        NetworkManager.MyPlayer().ChangeTeam();
    }
    public void Disconnect() {
        DisconnectStatic();
    }
    
    private void DisconnectStatic() {
        NetworkManager.Disconnect();
        StopCountdown();
    }

    public void SetServerName() { // Only call when NetworkInfoWrapper is ready
        Debug.Log("Server Name is: " + NetworkInfoWrapper.singleton.ServerName);
        serverNameText.text = NetworkInfoWrapper.singleton.ServerName;

        string text = "IP: " + NetworkManager.single.networkAddress;
        text += ", Port: " + NetworkManager.single.networkPort;
        serverSettings.text = text;

        myCanvas.enabled = true; // Show canvas, it's ready
    }

    //[RPC]
    void SetPauseMenuText() {
        UIPauseSpawn.SetServerNameText();
    }

    public void CancelCountdown() {
        StopCountdown();
    }
}
