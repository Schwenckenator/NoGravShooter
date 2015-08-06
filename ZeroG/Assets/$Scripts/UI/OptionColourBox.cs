using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OptionColourBox : MonoBehaviour {

    private Image image;

    void Awake() {
        image = GetComponent<Image>();
    }

    public void ChangeColour(Color colour) {
		colour.r = colour.r/2;
		colour.g = colour.g/2;
		colour.b = colour.b/2;
        image.color = colour;
    }
}
