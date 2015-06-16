﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UICreateGame : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Canvas createGame = UIManager.GetCanvas(Menu.CreateGame);
        InputField[] inputs = createGame.gameObject.GetComponentsInChildren<InputField>(true);
        inputs[0].text = SettingsManager.instance.ServerNameServer;
        inputs[1].text = SettingsManager.instance.PortNumStr;
        inputs[2].text = SettingsManager.instance.PasswordServer;

        gameObject.SendMessage("UIWindowInitialised", SendMessageOptions.RequireReceiver);
	}

    public void CreateGame(bool online) {
        SettingsManager.instance.ParsePortNumber();

        if (SettingsManager.instance.PortNum >= 0) { // Check for error
            NetworkManager.SetServerDetails(GameManager.MaxPlayers, SettingsManager.instance.PortNum, online);
            NetworkManager.InitialiseServer();
            SettingsManager.instance.SaveSettings();
            UIManager.instance.SetMenuWindow(Menu.Lobby);
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
