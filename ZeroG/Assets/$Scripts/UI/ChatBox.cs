using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChatBox : MonoBehaviour {

    private static bool isDirty = false;
    public static void Dirty() {
        Debug.Log("Marked Dirty");
        isDirty = true;
    }

    private Text myText;

    void OnEnable() {
        myText = GetComponent<Text>();
        Dirty();
    }

    void Update() {
        if (isDirty) {
            RefreshText();
        }
    }

    void RefreshText() {
        Debug.Log("Refreshing Text.");
        isDirty = false;

        myText.text = ChatManager.SubmittedChat;
    }
}
