using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIChat : MonoBehaviour {

    private static ChangeableText[] textBoxes;
    private static List<ChangeableText> chatBoxes;
    private static List<ChangeableText> playerLists;

    public static void FindChatBoxes() {
        chatBoxes = new List<ChangeableText>();
        playerLists = new List<ChangeableText>();

        textBoxes = GameObject.FindObjectsOfType<ChangeableText>();

        foreach (ChangeableText textBox in textBoxes) {
            if (textBox.IsType("chat")) {
                chatBoxes.Add(textBox);
            } else if (textBox.IsType("playerList")) {
                playerLists.Add(textBox);
            }
        }
        UpdateChatBoxes();
        UpdatePlayerLists();
    }
    public static void UpdateChatBoxes() {
        foreach (ChangeableText chatBox in chatBoxes) {
            chatBox.SetText(ChatManager.SubmittedChat);
        }
    }
    public static void UpdatePlayerLists() {
        foreach (ChangeableText playerListBox in playerLists) {
            playerListBox.SetText(ScoreVictoryManager.UpdateScoreBoard());
        }
    }

    public void SubmitMessage(string newChat) {
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
}
