using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonEventListener : MonoBehaviour, IPointerClickHandler {

    public int buttonIndex;

    public void OnPointerClick(PointerEventData data) {
        Debug.Log("I got clicked!");
        UIGraphicsSettings.instance.ResolutionChange(buttonIndex);
    }
}
