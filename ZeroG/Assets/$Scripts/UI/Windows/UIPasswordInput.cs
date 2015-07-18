using UnityEngine;
using System.Collections;

public class UIPasswordInput : MonoBehaviour {

	// Use this for initialization
	void Start () {
        // Turn self off after initialsation
        gameObject.SetActive(false);
	}

    public void PasswordEndEdit(string value) {
        SettingsManager.instance.PasswordClient = value;
    }
    public void SubmitPassword() {
        UIManager.instance.ShowMenuWindow(Menu.PasswordInput, false);
        UIJoinGame.instance.ConnectToServer();
    }
}
