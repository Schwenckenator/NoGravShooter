using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UILobby : MonoBehaviour {

    public static UILobby singleton { get; private set; }

    public Text serverSettings;
    private static Text startButtonText;
    private static ChangeableText serverNameText;
    private static Button[] buttons;
    private static Button changeTeamButton;

    private static bool countdown = false;

    private static bool teamGame = false;

	// Use this for initialization
	void Start () {
        singleton = this;
        LobbyInit();

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

    void LobbyInit() {
        buttons = GetComponentsInChildren<Button>();
        startButtonText = buttons[0].GetComponentInChildren<Text>();
        changeTeamButton = buttons[2];
        ChangeableText[] texts = GetComponentsInChildren<ChangeableText>();
        foreach (var text in texts) {
            if (text.IsType("serverName")) serverNameText = text;
            if (text.IsType("chat") || text.IsType("playerList")) UIChat.ConnectChatBox(text);
        }
        ShowChangeTeamButton(false);
    }
    #region StartGame
    public void StartCountdown() {
        UILobby.singleton.StartCountdownStatic();
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
        if (DebugManager.IsAdminMode()) {
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
        GameManager.singleton.LoadLevel();
        countdown = false;
        ChangeButtonText(countdown);
    }
    #endregion

    private void ChangeButtonText(bool count) {
        startButtonText.text = count ? "Cancel" : "Start Game";
    }
    public void ChangeTeam() {
        NetworkManager.MyPlayer().ChangeTeam();
    }
    public void Disconnect() {
        UILobby.singleton.DisconnectStatic();
    }
    public void GoGameSettings() {
        UILobby.singleton.StopCountdown();
    }


    
    private void DisconnectStatic() {
        NetworkManager.Disconnect();
        StopCountdown();
    }

    public void SetServerName() {
        Debug.Log("Server Name is: " + SettingsManager.singleton.ServerNameClient);
        serverNameText.SetText(SettingsManager.singleton.ServerNameClient);

        string text = "IP: " + Network.player.ipAddress;
        text += ", Port: " + Network.player.port;
        serverSettings.text = text;
    }

    //[RPC]
    void SetPauseMenuText() {
        UIPauseSpawn.SetServerNameText();
    }

    public void CancelCountdown() {
        StopCountdown();
    }
}
