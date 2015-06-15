using UnityEngine;
using System.Collections;

public class HideableUI : MonoBehaviour, IChangeable, IHideable {

    public string _type; // Only for editor
    private CanvasGroup canvasGroup;

    public string type {
        get { return _type; }
    }

    public bool IsType(string otherType) {
        return _type == otherType;
    }

    public void Show(bool show) {
        gameObject.SetActive(show);
    }

    public bool IsVisible() {
        return gameObject.activeInHierarchy;
    }
}
