using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChatSubmitEvent : MonoBehaviour {

    [SerializeField]
    private InputField chatInputField = null;
    private Button chatEnterButton = null;

    void Start() {
        InputField.SubmitEvent submitEvent = new InputField.SubmitEvent();
        submitEvent.AddListener(SubmitChat);
        chatInputField.onEndEdit = submitEvent;

    }

    private void SubmitChat(string chat) {
        ChatManager.instance.AddToChat(chat);
    }
}
