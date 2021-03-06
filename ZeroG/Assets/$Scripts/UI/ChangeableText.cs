﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChangeableText : MonoBehaviour, IChangeable {
    /// <summary>
    /// The type of text this control holds. Used for sorting.
    /// </summary>
    public string textType;

    private Text myText;
    private string currentText;

    void Awake() {
        myText = GetComponentInChildren<Text>();
    }

    public void SetText(string newText) {
        if (currentText == newText) return;

        currentText = newText;
        myText.text = newText;
    }

    // Interface
    public bool IsType(string otherType) {
        return type == otherType;
    }

    public string type {
        get { return textType; }
    }
}
