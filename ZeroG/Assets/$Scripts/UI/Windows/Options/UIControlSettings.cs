using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIControlSettings : MonoBehaviour {
    private InputField[] inputFields;
    private Slider[] sliders;
    private Toggle[] toggles;

	// Use this for initialization
	void Start () {
        sliders = GetComponentsInChildren<Slider>(true);
        inputFields = GetComponentsInChildren<InputField>(true);
        toggles = GetComponentsInChildren<Toggle>(true);

        SliderUpdate();
        InputFieldUpdate();
        ToggleUpdate();
	}

    private void ToggleUpdate() {
        toggles[0].isOn = (SettingsManager.instance.MouseYDirection == 1);
    }

    private void InputFieldUpdate() {
        inputFields[0].text = SettingsManager.instance.MouseSensitivityX.ToString();
        inputFields[1].text = SettingsManager.instance.MouseSensitivityY.ToString();
    }

    private void SliderUpdate() {
        sliders[0].value = SettingsManager.instance.MouseSensitivityX * 100;
        sliders[1].value = SettingsManager.instance.MouseSensitivityY * 100;
    }

    public void MouseSenXSliderUpdate(float value) {
        SettingsManager.instance.MouseSensitivityX = value / 100f;
        InputFieldUpdate();
    }
    public void MouseSenYSliderUpdate(float value) {
        SettingsManager.instance.MouseSensitivityY = value / 100f;
        InputFieldUpdate();
    }
    public void InvertToggleUpdate(bool value) {
        SettingsManager.instance.MouseYDirection = value ? 1 : -1;
    }

}
