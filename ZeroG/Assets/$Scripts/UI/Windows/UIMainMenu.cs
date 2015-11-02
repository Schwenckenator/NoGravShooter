using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIMainMenu : MonoBehaviour {
    public InputField playerName;
    // Use this for initialization
    void Start () {
        playerName.text = SettingsManager.singleton.PlayerName;
	}
    public void CreateGame(GameObject menu) {
        if (CheckName()) {
            UIManager.singleton.OpenReplace(menu);
        }
    }
    public void JoinGame(GameObject menu) {
        if (CheckName()) {
            UIManager.singleton.OpenReplace(menu);
            UIJoinGame.instance.StartRefresh();
        }
    }

    private bool CheckName() {
        bool hasName = SettingsManager.singleton.PlayerName != "";
        if (!hasName) {
            UIMessage.ShowMessage("Please enter a Player Name.", true);
        }
        return hasName;
    }

    public void LoadTutorial() {
        GameManager.singleton.LoadTutorial();
    }
    public void QuitGame() {
        if (!Application.isWebPlayer && !Application.isEditor) {
            Application.Quit();
        }
    }
}
