using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChatBox : MonoBehaviour {

    private static bool isDirty = false;
    static bool debug = false;
    public static void Dirty() {
        if (debug) {
            Debug.Log("ChatBox Marked Dirty");
        }
        isDirty = true;
    }

    private Text myText;

    void OnEnable() {
        myText = GetComponent<Text>();
        Dirty();
    }

    void Update() {
        if (isDirty) {
            UpdateChatText();
        }
    }

    void UpdateChatText() {
        if (debug) {
            Debug.Log("Update Chat Text.");
        }
        isDirty = false;

        myText.text = ChatManager.SubmittedChat;
    }
}
