using UnityEngine;
using System.Collections;

public class ScreenFadeInOut : MonoBehaviour {
    public CanvasGroup fader;
    public bool startDark = true;
    public float fadeSpeed = 1.5f;

    private bool fading = false;
    private bool clearing = false;

	// Use this for initialization
	void Awake () {
        if (startDark) {
            fader.alpha = 1;
        } else {
            fader.alpha = 0;
        }
	}
    void Update() {
        if (clearing) {
            Clear();
        }else if (fading) {
            Fade();
        }
    }
    // Externals
    public void FadeToBlack() {
        fading = true;
    }
    public void FadeToClear() {
        clearing = true;
    }

    void Clear() {
        fader.alpha = Mathf.Lerp(fader.alpha, 0f, fadeSpeed * Time.deltaTime);
        if (fader.alpha < 0.05f) {
            fader.alpha = 0;
            clearing = false;
        }
    }
    void Fade() {
        fader.alpha = Mathf.Lerp(fader.alpha, 1f, fadeSpeed * Time.deltaTime);
        if (fader.alpha > 0.95f) {
            fader.alpha = 1;
            fading = false;
        }
    }
}
