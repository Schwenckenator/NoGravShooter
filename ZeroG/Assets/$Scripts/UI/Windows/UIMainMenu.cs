using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIMainMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
        InputField playerName = gameObject.GetComponentInChildren<InputField>();
        playerName.text = SettingsManager.instance.PlayerName;
	}
    public void CreateGame() {
        if (CheckName()) {
            UIManager.instance.GoCreateGame();
        }
    }
    public void JoinGame() {
        if (CheckName()) {
            UIManager.instance.GoJoinGame();
        }
    }

    private bool CheckName() {
        bool hasName = SettingsManager.instance.PlayerName != "";
        if (!hasName) {
            UIMessage.ShowMessage("Please enter a Player Name.", true);
        }
        return hasName;
    }

    public void LoadTutorial() {
        GameManager.instance.LoadTutorial();
    }
    public void QuitGame() {
        if (!Application.isWebPlayer && !Application.isEditor) {
            Application.Quit();
        } else {
            UIManager.RemoveAllGUI();
        }
    }
}
