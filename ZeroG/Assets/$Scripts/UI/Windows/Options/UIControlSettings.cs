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
        toggles[0].isOn = (SettingsManager.singleton.MouseYDirection == 1);
    }

    private void InputFieldUpdate() {
        inputFields[0].text = SettingsManager.singleton.MouseSensitivityX.ToString();
        inputFields[1].text = SettingsManager.singleton.MouseSensitivityY.ToString();
    }

    private void SliderUpdate() {
        sliders[0].value = SettingsManager.singleton.MouseSensitivityX * 100;
        sliders[1].value = SettingsManager.singleton.MouseSensitivityY * 100;
    }

    public void MouseSenXSliderUpdate(float value) {
        SettingsManager.singleton.MouseSensitivityX = value / 100f;
        InputFieldUpdate();
    }
    public void MouseSenYSliderUpdate(float value) {
        SettingsManager.singleton.MouseSensitivityY = value / 100f;
        InputFieldUpdate();
    }
    public void InvertToggleUpdate(bool value) {
        SettingsManager.singleton.MouseYDirection = value ? 1 : -1;
    }

}
