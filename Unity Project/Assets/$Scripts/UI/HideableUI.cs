using UnityEngine;
using System.Collections;


[RequireComponent(typeof(CanvasGroup))]
public class HideableUI : MonoBehaviour, IChangeable, IHideable {

    public string _type; // Only for editor
    private CanvasGroup canvasGroup;

	// Use this for initialization
	void Awake () {
        canvasGroup = GetComponent<CanvasGroup>();
	}

    public string type {
        get { return _type; }
    }

    public bool IsType(string otherType) {
        return _type == otherType;
    }

    public void Show(bool show) {
        canvasGroup.alpha = show ? 1 : 0;
    }

    public bool IsVisible() {
        return canvasGroup.alpha > 0;
    }
}
