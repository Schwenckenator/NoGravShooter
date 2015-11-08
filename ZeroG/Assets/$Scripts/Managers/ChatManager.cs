using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Chat manager manages chat, submission to chat and RPCs of chat
/// </summary>
public class ChatManager : MonoBehaviour {

    public static ChatManager singleton { get; private set; }

    public static string currentChat;

    private static int maxChatLines = 50;



    private static List<string> submittedChatList;
    private static string submittedChat;
    public static string SubmittedChat {
        get { return ChatManager.submittedChat; }
    }

    ////NetworkView //NetworkView;
    void Awake() {
        singleton = this;
        currentChat = "";
        submittedChatList = new List<string>();
    }


    public void AddToChat(string input, bool addPlayerPrefix = false) {
        if (input != "") {
            string newChat = "";
            if (addPlayerPrefix) {
                newChat += SettingsManager.singleton.PlayerName + ": ";
            }
            newChat += input;

            ////NetworkView.RPC("UpdateChatRPC", RPCMode.All, newChat);
        }
    }
    public static void TutorialChat(string input) {
        if (input == "") return;

        ClearAllChat();
        UpdateChat(input);
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
    ////[RPC]
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
        UIChat.UpdateChatBoxes();
    }
    public static void ClearAllChat() {
        submittedChat = "";
        currentChat = "";
        submittedChatList.Clear();
        UIChat.UpdateChatBoxes();
    }
    public static void ClearCurrentChat() {
        currentChat = "";
    }
    
    public static void DebugMessage(string message) {
        if (!DebugManager.debugMode) return;
        
        if (Application.isEditor) { Debug.Log(message); }
        ChatManager.AddToLocalChat(message);
    }

}
