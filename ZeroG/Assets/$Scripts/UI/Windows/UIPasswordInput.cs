using UnityEngine;
using System.Collections;

public class UIPasswordInput : MonoBehaviour {

    public void PasswordEndEdit(string value) {
        SettingsManager.singleton.PasswordClient = value;
    }
    public void SubmitPassword() {
        //UIJoinGame.singleton.ConnectToServer();
    }
}
