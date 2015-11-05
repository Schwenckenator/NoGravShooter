using UnityEngine;
using System.Collections;

public class BloodyScreen : MonoBehaviour {
    private static BloodyScreen instance;
    private static CanvasGroup canvasGroup;

    private static float fadeSpeed = 4;

    private static bool isWavering = false;
    private static float waverSpeed = 0;
    private static float waverIntensity = 0; // From 0-1

	// Use this for initialization
	void Start () {
        instance = this;
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
	}

    public static void Show(bool show) {
        instance.StopAllCoroutines();
        canvasGroup.alpha = show ? 1 : 0;
    }

    public static void Flash() {
        instance.StartCoroutine(instance.FlashBloodyScreen());
    }

    IEnumerator FlashBloodyScreen() {
        canvasGroup.alpha = 1f;
        yield return null;

        float t = 0f;
        while (canvasGroup.alpha > 0.01f) {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t * fadeSpeed);
            t += Time.deltaTime;
            yield return null;
        }
    }

    public static void Waver(float speed, float intensity) {
        Debug.Log("Told to waver");
        Debug.Log("Is wavering already? "+isWavering.ToString());
        if (!isWavering) {
            Debug.Log("Starting co-routine");
            isWavering = true;
            instance.StartCoroutine(instance.WaverBloodyScreen());
        }
        waverSpeed = speed;
        waverIntensity = intensity;
    }

    public static void StopWaver() {
        Debug.Log("Told to Stop");
        isWavering = false;
    }

    IEnumerator WaverBloodyScreen() {
        float t;
        while (isWavering) {
            t = 0;
            while (canvasGroup.alpha < waverIntensity) {
                canvasGroup.alpha = Mathf.Lerp(waverIntensity / 4, waverIntensity, t * waverSpeed);
                t += Time.deltaTime;
                yield return null;
            }
            t = 0;
            while (canvasGroup.alpha > waverIntensity / 4) {
                canvasGroup.alpha = Mathf.Lerp(waverIntensity, waverIntensity/4, t * waverSpeed);
                t += Time.deltaTime;
                yield return null;
            }
        }
        // Wavering stopped
        float startAlpha = canvasGroup.alpha;
        t = 0;
        while (canvasGroup.alpha > 0.01f) {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, t);
            t += Time.deltaTime;
            yield return null;
        }
    }
}
