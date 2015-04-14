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

    private static Text playerList;
    private static Text chat;

    private static bool countdown = false;

	// Use this for initialization
	void Start () {
        LobbyInit();
	}

    void LobbyInit() {
        Canvas lobby = UIManager.GetCanvas(Menu.Lobby);

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

    public void Disconnect() {
        UILobby.instance.DisconnectStatic();
    }
    private void DisconnectStatic() {
        NetworkManager.Disconnect();
        if (countdown) StopCoroutine("CountdownStartGame");
        countdown = false;
    }
}
