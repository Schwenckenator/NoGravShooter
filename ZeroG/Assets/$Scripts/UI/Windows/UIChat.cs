using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIChat : MonoBehaviour {

    public void SubmitMessage(string newChat) {
        if(newChat != "" && Input.GetButtonDown("Submit")){
            ChatManager.singleton.AddToChat(newChat, true);
        }
    }
    public void FocusTextBox(InputField value) {
        if (Input.GetButtonDown("Submit")) {
            value.text = "";
            value.ActivateInputField();
        }
    }
}
