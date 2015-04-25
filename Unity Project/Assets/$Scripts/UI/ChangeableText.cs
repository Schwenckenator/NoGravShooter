using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChangeableText : MonoBehaviour, ITextBox {
    /// <summary>
    /// The type of text this control holds. Used for sorting.
    /// </summary>
    public string textType;

    private Text myText;

    void Awake() {
        myText = GetComponentInChildren<Text>();
    }

    public void SetText(string newText) {
        myText.text = newText;
    }

    public string GetTextType() {
        return textType;
    }
}
