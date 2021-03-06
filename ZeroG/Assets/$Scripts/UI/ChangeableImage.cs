﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChangeableImage : MonoBehaviour, IChangeable {
    #region Interface implementation
    public string imageType;

    public string type {
        get { return imageType; }
    }

    public bool IsType(string otherType) {
        return this.type == otherType;
    }
    #endregion

    private Image myImage;

	// Use this for initialization
	void Awake () {
        myImage = this.GetComponentInChildren<Image>();	
	}

    public void SetImage(Sprite image) {
        myImage.sprite = image;
    }

    public void SetColour(Color colour) {
        myImage.color = colour;
    }

    public void SetVisible(bool visible) {
        myImage.enabled = visible;
    }

    
}
