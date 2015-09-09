using UnityEngine;
using System.Collections;

public class BloodyScreen : MonoBehaviour {
    private static BloodyScreen instance;
    private static CanvasGroup canvasGroup;

    private static float flashSpeed = 7;

	// Use this for initialization
	void Start () {
        instance = this;
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
	}

    public static void Show(bool show) {
        if (show) {
            instance.StopAllCoroutines();
            instance.StartCoroutine(instance.ShowBloodyScreen());
        } else {
            canvasGroup.alpha = 0;
        }
    }

    public static void Flash() {
        instance.StartCoroutine(instance.FlashBloodyScreen());
    }

    IEnumerator ShowBloodyScreen() {
        while (!IncreaseAlpha(Time.deltaTime * flashSpeed)) {
            Debug.Log("Show Bloody Screen.");
            yield return null;
        }
    }
    IEnumerator FlashBloodyScreen() {
        while (!IncreaseAlpha(Time.deltaTime * flashSpeed)) {
            Debug.Log("Flash Bloody Screen UP.");
            yield return null;
        }
        //yield return new WaitForSeconds(0.1f); // Hold for 1/10th of second
        while (!DecreaseAlpha(Time.deltaTime * flashSpeed)) {
            Debug.Log("Flash Bloody Screen DOWN.");
            yield return null;
        }
    }

    static bool IncreaseAlpha(float increase) {
        canvasGroup.alpha += increase;

        if (canvasGroup.alpha >= 0.99f) {
            canvasGroup.alpha = 1;
            return true;
        } else {
            return false;
        }
    }
    static bool DecreaseAlpha(float decrease) {
        canvasGroup.alpha -= decrease;

        if (canvasGroup.alpha <= 0.01f) {
            canvasGroup.alpha = 0;
            return true;
        } else {
            return false;
        }
    }

    void OnLevelWasLoaded() {
        BloodyScreen.Show(false);
    }
}
