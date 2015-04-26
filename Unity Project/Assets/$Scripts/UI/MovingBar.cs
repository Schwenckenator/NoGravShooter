using UnityEngine;
using System.Collections;

public class MovingBar : MonoBehaviour {
    public enum Anchor { Right, Left}

    public Anchor anchorSide;
    public string barType;

    private float maxValue;
    private float currentValue;
    private RectTransform myTransform;
    

    private float width;
    private float height;

    void Awake() {
        myTransform = GetComponent<RectTransform>();
        width = myTransform.rect.width;
        height = myTransform.rect.height;
    }
    public string GetBarType() {
        return barType;
    }
    public void SetMaxValue(float max) {
        maxValue = max;
    }
    private float BarXValue(float value) {
        return (value / maxValue) * width;
    }
    public void SetValue(float value) {
        if (currentValue == value) return;

        currentValue = value;
        MoveBar();
    }

    private void MoveBar() {
        myTransform.sizeDelta = new Vector2(BarXValue(currentValue), height);
        
        if(anchorSide == Anchor.Right)
            myTransform.anchoredPosition = new Vector2(-BarXValue(currentValue), 0);
    }


}
