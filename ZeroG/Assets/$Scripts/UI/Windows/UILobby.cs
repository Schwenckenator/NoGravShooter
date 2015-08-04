using UnityEngine;
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

    private static Text startButtonText;
    private static ChangeableText serverNameText;
    private static Button[] buttons;
    private static Button changeTeamButton;

    private static bool countdown = false;

    private static bool teamGame = false;

	// Use this for initialization
	void Start () {
        LobbyInit();

        // Turn self off after initialsation
        gameObject.SetActive(false);
	}
    void Update() {
        if (teamGame != SettingsManager.instance.IsTeamGameMode()) {
            ShowChangeTeamButton(SettingsManager.instance.IsTeamGameMode());
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
    private void StopCountdown() {
        if (countdown) {
            StopCoroutine("CountdownStartGame");
            ChatManager.instance.AddToChat("Countdown cancelled.");
            countdown = false;
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
        UILobby.instance.DisconnectStatic();
    }
    public void GoGameSettings() {
        UIManager.instance.GoGameSettings(true);
        UILobby.instance.StopCountdown();
    }


    
    private void DisconnectStatic() {
        NetworkManager.Disconnect();
        StopCountdown();
    }

    public void SetServerName() {
        serverNameText.SetText(SettingsManager.instance.ServerNameClient);
    }

    [RPC]
    void SetPauseMenuText() {
        UIPauseSpawn.SetServerNameText();
    }
}
