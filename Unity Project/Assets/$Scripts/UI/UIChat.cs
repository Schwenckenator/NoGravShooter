using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIChat : MonoBehaviour {

    private static ChatBox[] chatBoxes;

    public static void FindChatBoxes() {
        chatBoxes = GameObject.FindObjectsOfType<ChatBox>();
        UpdateChatBoxes();
    }
    public static void UpdateChatBoxes() {
        foreach (ChatBox chatBox in chatBoxes) {
            chatBox.SetText(ChatManager.SubmittedChat);
        }
    }

    public void SubmitChat(string newChat) {
        if(newChat != "" && Input.GetButtonDown("Submit")){
            ChatManager.instance.AddToChat(newChat, true);
        }
    }
    public void FocusTextBox(InputField value) {
        if (Input.GetButtonDown("Submit")) {
            value.text = "";
            value.ActivateInputField();
        }
    }
    public void SubmitChat() {

    }
}
