using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIChat : MonoBehaviour {

    private static ITextBox[] textBoxes;
    private static List<ITextBox> chatBoxes;
    private static List<ITextBox> playerLists;

    public static void FindChatBoxes() {
        chatBoxes = new List<ITextBox>();
        playerLists = new List<ITextBox>();

        textBoxes = GameObject.FindObjectsOfType<ChangeableText>();

        foreach (ITextBox textBox in textBoxes) {
            if (textBox.GetTextType() == "chat") {
                chatBoxes.Add(textBox);
            } else if (textBox.GetTextType() == "playerList") {
                playerLists.Add(textBox);
            }
        }
        UpdateChatBoxes();
        UpdatePlayerLists();
    }
    public static void UpdateChatBoxes() {
        foreach (ITextBox chatBox in chatBoxes) {
            chatBox.SetText(ChatManager.SubmittedChat);
        }
    }
    public static void UpdatePlayerLists() {
        foreach (ITextBox playerListBox in playerLists) {
            playerListBox.SetText(PlayerListText());
        }
    }

    private static string PlayerListText() {
        bool showScores = GameManager.instance.GameInProgress;

        string playerNames = "Players:\n";
        foreach (Player player in NetworkManager.connectedPlayers) {
            playerNames += player.Name;
            if (showScores) {
                playerNames += " " + player.Score;
            }
            playerNames += "\n";
        }
        return playerNames;
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
