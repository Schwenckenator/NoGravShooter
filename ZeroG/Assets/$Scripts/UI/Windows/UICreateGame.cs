using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UICreateGame : MonoBehaviour {

	// Use this for initialization
	void Start () {
        InputField[] inputs = GetComponentsInChildren<InputField>(true);
        inputs[0].text = SettingsManager.singleton.ServerNameServer;
        inputs[1].text = SettingsManager.singleton.PortNumStr;
        inputs[2].text = SettingsManager.singleton.PasswordServer;

	}

    public void CreateGame(bool online) {
        SettingsManager.singleton.ParsePortNumber();
        // Check for error
        if (SettingsManager.singleton.ServerNameServer == "") {
            UIMessage.ShowMessage("Server name required. Please enter a name.", true);
            return;
        }
        if (SettingsManager.singleton.PortNum < 0) {
            UIMessage.ShowMessage("Port number invalid. Please enter a valid port number.", true);
            return;
        }
         

        NetworkManager.SetServerDetails(GameManager.MaxPlayers, SettingsManager.singleton.PortNum);
        NetworkManager.single.StartHost();
        SettingsManager.singleton.SaveSettings();
        
    }

    public void SetServerName(string value) {
        SettingsManager.singleton.ServerNameServer = value;
    }
    public void SetPortNum(string value) {
        SettingsManager.singleton.PortNumStr = value;
    }
    public void SetPassword(string value) {
        SettingsManager.singleton.PasswordServer = value;
    }
}
