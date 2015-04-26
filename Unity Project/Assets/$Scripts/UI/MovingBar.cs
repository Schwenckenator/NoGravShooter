using UnityEngine;
using System.Collections;

public class MovingBar : MonoBehaviour {

    public string barType;

    private float maxValue;
    private float currentValue;
    private RectTransform myTransform;

    void Awake() {
        myTransform = GetComponent<RectTransform>();
    }
    public string GetBarType() {
        return barType;
    }
    public void SetMaxValue(float max) {
        maxValue = max;
    }
    private float BarXValue(float value) {
        return (value / maxValue) * 300;
    }
    public void SetValue(float value) {
        if (currentValue == value) return;

        currentValue = value;
        MoveBar();
    }

    private void MoveBar() {
        myTransform.sizeDelta = new Vector2(BarXValue(currentValue), 50);
        myTransform.anchoredPosition = new Vector2(-BarXValue(currentValue), 0);
    }


}
