using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OptionColourBox : MonoBehaviour {

    private Image image;

    void Awake() {
        image = GetComponent<Image>();
    }

    public void ChangeColour(Color colour) {
        image.color = colour;
    }
}
