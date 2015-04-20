using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChatBox : MonoBehaviour, ITextBox {

    public string textType;
    private Text myText;

    void Awake() {
        myText = GetComponent<Text>();
    }

    public void SetText(string newText) {
        myText.text = newText;
    }
}
