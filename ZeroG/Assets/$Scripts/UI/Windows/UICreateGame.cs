using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UICreateGame : MonoBehaviour {

	// Use this for initialization
	void Start () {
        InputField[] inputs = GetComponentsInChildren<InputField>(true);
        inputs[0].text = SettingsManager.instance.ServerNameServer;
        inputs[1].text = SettingsManager.instance.PortNumStr;
        inputs[2].text = SettingsManager.instance.PasswordServer;

        // Turn self off after initialsation
        gameObject.SetActive(false);
	}

    public void CreateGame(bool online) {
        SettingsManager.instance.ParsePortNumber();

        if (SettingsManager.instance.PortNum >= 0) { // Check for error
            UIManager.instance.SetMenuWindow(Menu.Lobby);
            NetworkManager.SetServerDetails(GameManager.MaxPlayers, SettingsManager.instance.PortNum, online);
            NetworkManager.InitialiseServer();
            SettingsManager.instance.SaveSettings();
        }
    }

    public void SetServerName(string value) {
        SettingsManager.instance.ServerNameServer = value;
    }
    public void SetPortNum(string value) {
        SettingsManager.instance.PortNumStr = value;
    }
    public void SetPassword(string value) {
        SettingsManager.instance.PasswordServer = value;
    }
}
