using UnityEngine;
using System.Collections;

/// <summary>
/// Chat manager manages chat, submission to chat and RPCs of chat
/// </summary>
public class ChatManager : MonoBehaviour {
    private GameManager gameManager;

    public static string currentChat;

    private static string submittedChat;
    public static string SubmittedChat {
        get { return ChatManager.submittedChat; }
    }

    void Start() {
        gameManager = GetComponent<GameManager>();
        currentChat = "";
    }

    public void SubmitTextToChat(string input, bool addPlayerPrefix = true) {
        if (input != "") {
            string newChat = "";
            if (addPlayerPrefix) {
                newChat += gameManager.currentPlayerName + ": ";
            }
            newChat += input + "\n";

            networkView.RPC("UpdateChat", RPCMode.All, newChat);
        }
    }
    [RPC]
    private void UpdateChat(string newChat) {
        submittedChat += newChat;
    }
    public static void ClearChat() {
        submittedChat = "";
        currentChat = "";
    }
}
