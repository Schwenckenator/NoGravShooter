using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIJoinByIP : MonoBehaviour {

    void Start() {
        Canvas canvas = UIManager.GetCanvas(Menu.JoinByIP);
        InputField[] fields = canvas.GetComponentsInChildren<InputField>(true);

        fields[0].text = SettingsManager.instance.IpAddress;
        fields[1].text = SettingsManager.instance.PortNumStr;

        //gameObject.SendMessage("UIWindowInitialised", SendMessageOptions.RequireReceiver);
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
    }
}
