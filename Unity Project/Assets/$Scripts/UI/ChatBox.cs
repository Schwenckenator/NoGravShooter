using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChatBox : MonoBehaviour {

    private Text myText;

    void Awake() {
        myText = GetComponent<Text>();
    }

    public void UpdateChatText(string newText) {
        myText.text = newText;
    }
}
