using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIWinnerSplash : MonoBehaviour {

    public Text myText;
    private static UIWinnerSplash instance;
    private Canvas myCanvas;

    public static void SetWinner(string winnerText) {
        instance.SetText(winnerText);
    }
    private void SetText(string winnerText) {
        myCanvas.enabled = true;

        string newText = winnerText + "\n wins!";
        myText.text = newText;
    }
    void OnLevelWasLoaded() {
        myCanvas.enabled = false;
    }
    void Start() {
        instance = this;
        myCanvas = GetComponent<Canvas>();
        myCanvas.enabled = false;
    }
}
