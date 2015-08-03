using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIJoinByIP : MonoBehaviour {

    void Start() {
        
        InputField[] fields = GetComponentsInChildren<InputField>(true);

        fields[0].text = SettingsManager.instance.IpAddress;
        fields[1].text = SettingsManager.instance.PortNumStr;

        // Turn self off after initialsation
        gameObject.SetActive(false);
    }

    public void SetIPAddress(string value) {
        SettingsManager.instance.IpAddress = value;
    }
    public void SetPortNum(string value) {
        SettingsManager.instance.PortNumStr = value;
    }
    public void SetPassword(string value) {
        SettingsManager.instance.PasswordClient = value;
    }
    public void JoinGame(){
        SettingsManager.instance.ParsePortNumber();

        if (SettingsManager.instance.PortNum >= 0) { // Check for error
            SettingsManager.instance.SaveSettings();

            NetworkManager.SetClientDetails(SettingsManager.instance.IpAddress, SettingsManager.instance.PortNum);
            NetworkManager.ConnectToServer();
        }
        UIManager.instance.GoJoinByIP(false);
        UIMessage.ShowMessage("Connecting To Server", false);
    }
}
