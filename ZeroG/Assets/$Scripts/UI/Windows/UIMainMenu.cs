using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIMainMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
        InputField playerName = gameObject.GetComponentInChildren<InputField>();
        playerName.text = SettingsManager.singleton.PlayerName;
	}
    public void CreateGame() {
        if (CheckName()) {
            UIManager.singleton.SetMenuWindow(Menu.CreateGame);
        }
    }
    public void JoinGame() {
        if (CheckName()) {
            UIManager.singleton.SetMenuWindow(Menu.JoinGame);
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
        } else {
            UIManager.RemoveAllGUI();
        }
    }
}
