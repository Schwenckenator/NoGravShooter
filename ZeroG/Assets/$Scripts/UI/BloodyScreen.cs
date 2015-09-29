using UnityEngine;
using System.Collections;

public class BloodyScreen : MonoBehaviour {
    private static BloodyScreen instance;
    private static CanvasGroup canvasGroup;

    private static float fadeSpeed = 4;

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


    void OnLevelWasLoaded() {
        BloodyScreen.Show(false);
    }
}
