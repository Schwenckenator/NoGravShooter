using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIJoinByIP : MonoBehaviour {

    void Start() {
        
        InputField[] fields = GetComponentsInChildren<InputField>(true);

        fields[0].text = SettingsManager.singleton.IpAddress;
        fields[1].text = SettingsManager.singleton.PortNumStr;

    }

    public void SetIPAddress(string value) {
        SettingsManager.singleton.IpAddress = value;
    }
    public void SetPortNum(string value) {
        SettingsManager.singleton.PortNumStr = value;
    }
    public void SetPassword(string value) {
        SettingsManager.singleton.PasswordClient = value;
    }
    public void JoinGame(){
        SettingsManager.singleton.ParsePortNumber();

        if (SettingsManager.singleton.PortNum >= 0) { // Check for error
            SettingsManager.singleton.SaveSettings();

            NetworkManager.SetClientDetails(SettingsManager.singleton.IpAddress, SettingsManager.singleton.PortNum);
            NetworkManager.single.StartClient();
        }
        UIMessage.ShowMessage("Connecting To Server", false);
    }
}
