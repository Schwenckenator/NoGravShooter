using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Chat manager manages chat, submission to chat and RPCs of chat
/// </summary>
public class ChatManager : MonoBehaviour {
    #region Instance
    //Here is a private reference only this class can access
    private static ChatManager _instance;
    //This is the public reference that other classes will use
    public static ChatManager instance {
        get {
            //If _instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<ChatManager>();
            }
            return _instance;
        }
    }
    #endregion

    public static string currentChat;

    private static int maxChatLines = 50;



    private static List<string> submittedChatList;
    private static string submittedChat;
    public static string SubmittedChat {
        get { return ChatManager.submittedChat; }
    }

    void Start() {
        currentChat = "";
        submittedChatList = new List<string>();
    }


    public void AddToChat(string input, bool addPlayerPrefix = false) {
        if (input != "") {
            string newChat = "";
            if (addPlayerPrefix) {
                newChat += SettingsManager.instance.PlayerName + ": ";
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
        if (!DebugManager.IsDebugMode()) return;
        
        if (Application.isEditor) { Debug.Log(message); }
        ChatManager.AddToLocalChat(message);
    }

}
