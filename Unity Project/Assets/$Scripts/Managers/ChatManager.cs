using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Chat manager manages chat, submission to chat and RPCs of chat
/// </summary>
public class ChatManager : MonoBehaviour {
    private SettingsManager settingsManager;

    public static string currentChat;

    private int maxChatLines = 50;

    private static List<string> submittedChatList;
    private static string submittedChat;
    public static string SubmittedChat {
        get { return ChatManager.submittedChat; }
    }

    void Start() {
        settingsManager = GetComponent<SettingsManager>();
        currentChat = "";
        submittedChatList = new List<string>();
    }

    public void SubmitTextToChat(string input, bool addPlayerPrefix = true) {
        if (input != "") {
            string newChat = "";
            if (addPlayerPrefix) {
                newChat += settingsManager.PlayerName + ": ";
            }
            newChat += input;

            networkView.RPC("UpdateChat", RPCMode.All, newChat);
        }
    }
    [RPC]
    private void UpdateChat(string newChat) {
        submittedChatList.Add(newChat);
        PruneChatList(maxChatLines);
        UpdateChatString();
    }
    private void PruneChatList(int maxLines) {
        if (submittedChatList.Count > maxLines) {
            int numToRemove = submittedChatList.Count - maxLines;
            submittedChatList.RemoveRange(0, numToRemove); // Remove from front of list, oldest messages first
        }
    }
    private static void UpdateChatString() {
        submittedChat = "";
        foreach (string line in submittedChatList) {
            submittedChat += line + "\n";
        }
    }
    public static void ClearAllChat() {
        submittedChat = "";
        currentChat = "";
        submittedChatList.Clear();
    }
    public static void ClearCurrentChat() {
        currentChat = "";
    }
}
