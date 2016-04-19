using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking.Match;
using System.Collections;

public class UICreateGame : MonoBehaviour {

	// Use this for initialization
	void OnEnable () {
        InputField[] inputs = GetComponentsInChildren<InputField>(true);
        inputs[0].text = SettingsManager.singleton.ServerName;
        inputs[1].text = SettingsManager.singleton.PortNumStr;
        inputs[2].text = SettingsManager.singleton.PasswordServer;

	}

    public void CreateGame(bool useMatchmaking) {
        SettingsManager.singleton.ParsePortNumber();
        // Check for error
        if (SettingsManager.singleton.ServerName == "") {
            UIMessage.ShowMessage("Server name required. Please enter a name.");
            return;
        }
        if (SettingsManager.singleton.PortNum < 0) {
            UIMessage.ShowMessage("Port number invalid. Please enter a valid port number.");
            return;
        }

        LobbyManager.s_Singleton.CreateGame(useMatchmaking);
        //NetworkManager.SetServerDetails(GameManager.MaxPlayers, SettingsManager.singleton.PortNum);
        //if (online) {
        //    CreateMatchRequest req = new CreateMatchRequest();
        //    req.name = SettingsManager.singleton.ServerName;
        //    req.size = GameManager.MaxPlayers;
        //    req.advertise = true;
        //    req.password = "";
        //    NetworkManager.single.matchMaker.CreateMatch(req, NetworkManager.single.OnMatchCreate);
        //} else {
        //    NetworkManager.single.StartHost();
        //}
        SettingsManager.singleton.SaveSettings();
        
    }

    public void SetServerName(string value) {
        SettingsManager.singleton.ServerName = value;
    }
    public void SetPortNum(string value) {
        SettingsManager.singleton.PortNumStr = value;
    }
    public void SetPassword(string value) {
        SettingsManager.singleton.PasswordServer = value;
    }
}
