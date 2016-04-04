using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIMessage : MonoBehaviour {
    
    public Button myButton;
    public Text myText;

    static GameObject myObj;
    static Button button;
    static Text messageText;
    static Text buttonText;


	// Use this for initialization
	void Start () {
        myObj = gameObject;
        button = myButton;
        messageText = myText;
        buttonText = button.GetComponentInChildren<Text>();

        CloseMessage();
	}

    public static void ShowMessage(string message, bool buttonAvailable = true, string buttonMessage = "OK") {
        //Enable message canvas
        myObj.SetActive(true);
        
        //Change text to message
        messageText.text = message;
        //Hide or show button
        button.gameObject.SetActive(buttonAvailable);
        if (buttonAvailable) {
            buttonText.text = buttonMessage;
        }
    }

    public static void CloseMessage() {
        // Hide message canvas
        if (myObj == null) return;
        myObj.SetActive(false);
    }
    public void CloseMessagePress() {
        CloseMessage();
    }
}
