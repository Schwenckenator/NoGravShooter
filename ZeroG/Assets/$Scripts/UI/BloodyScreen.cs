using UnityEngine;
using System.Collections;

public class BloodyScreen : MonoBehaviour {

    private static Canvas myCanvas;

	// Use this for initialization
	void Start () {
        myCanvas = GetComponent<Canvas>();
        myCanvas.enabled = false;
	}

    public static void Show(bool show) {
        myCanvas.enabled = show;
    }
}
