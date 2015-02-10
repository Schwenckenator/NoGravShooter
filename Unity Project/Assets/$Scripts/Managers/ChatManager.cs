using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Chat manager manages chat, submission to chat and RPCs of chat
/// </summary>
public class ChatManager : MonoBehaviour {
    private SettingsManager settingsManager;

    public static string currentChat;

    private static int maxChatLines = 50;

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

    public void AddToChat(string input, bool addPlayerPrefix = true) {
        if (input != "") {
            string newChat = "";
            if (addPlayerPrefix) {
                newChat += settingsManager.PlayerName + ": ";
            }
            newChat += input;

            networkView.RPC("UpdateChatRPC", RPCMode.All, newChat);
        }
    }
    /// <summary>
    /// Only sends the message to the local chat. No RPC
    /// </summary>
    /// <param name="input"></param>
    public static void AddToLocalChat(string input) {
        if (input != "") {
            UpdateChat(input);
        }
    }
    [RPC]
    private void UpdateChatRPC(string newChat) {
        UpdateChat(newChat);
    }

    private static void UpdateChat(string newChat) {
        submittedChatList.Add(newChat);
        PruneChatList(maxChatLines);
        UpdateChatString();
    }
    private static void PruneChatList(int maxLines) {
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
    
    public static void PrintMessageIfDebug(string message) {
        if (!DebugManager.IsDebugMode()) return;

        Debug.Log(message);
        ChatManager.AddToLocalChat(message);
    }

}
