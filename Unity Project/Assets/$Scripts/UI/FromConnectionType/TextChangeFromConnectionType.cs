using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// This script will change the text of a button depending on whether the game is a client or server
/// It will need to be updated manually outside
/// </summary>
public class TextChangeFromConnectionType : MonoBehaviour {

    public string clientText;
    public string serverText;

    private Text myText;

    void Awake() {
        myText = GetComponentInChildren<Text>();
    }

    public void UpdateText() {
        string temp = Network.isServer ? serverText : clientText;
        SetText(temp);
    }

    private void SetText(string value) {
        myText.text = value;
    }
}
