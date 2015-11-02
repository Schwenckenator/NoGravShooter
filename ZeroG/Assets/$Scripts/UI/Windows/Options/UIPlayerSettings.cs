using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIPlayerSettings : MonoBehaviour {
    static GameObject instance;

    private static InputField[] inputFields;
    private static Slider[] sliders;
    private static OptionColourBox[] playerColours;
    private static Toggle[] toggles;

	// Use this for initialization
	void Start () {
        instance = gameObject;

        sliders = GetComponentsInChildren<Slider>(true);
        inputFields = GetComponentsInChildren<InputField>(true);
        playerColours = GetComponentsInChildren<OptionColourBox>(true);
        toggles = GetComponentsInChildren<Toggle>(true);

        SliderUpdate();
        InputFieldUpdate();
        ColoursUpdate();
        ToggleUpdate();

        gameObject.SetActive(false);
	}

    private void InputFieldUpdate() {
        inputFields[0].text = SettingsManager.singleton.FieldOfView.ToString();
        inputFields[1].text = SettingsManager.singleton.ColourR.ToString();
        inputFields[2].text = SettingsManager.singleton.ColourG.ToString();
        inputFields[3].text = SettingsManager.singleton.ColourB.ToString();
    }
    private void SliderUpdate() {
        sliders[0].value = SettingsManager.singleton.FieldOfView * 10;
        sliders[1].value = SettingsManager.singleton.ColourR * 100;
        sliders[2].value = SettingsManager.singleton.ColourG * 100;
        sliders[3].value = SettingsManager.singleton.ColourB * 100;
    }
    private void ColoursUpdate() {
        playerColours[0].ChangeColour(PlayerColourManager.singleton.LimitTeamColour(TeamColour.Red, SettingsManager.singleton.GetPlayerColour()));
        playerColours[1].ChangeColour(PlayerColourManager.singleton.LimitTeamColour(TeamColour.None, SettingsManager.singleton.GetPlayerColour()));
        playerColours[2].ChangeColour(PlayerColourManager.singleton.LimitTeamColour(TeamColour.Blue, SettingsManager.singleton.GetPlayerColour()));
    }
    private void ToggleUpdate() {
        toggles[0].isOn = SettingsManager.singleton.AutoPickup;
    }

    public void FOVSliderUpdate(float value) {
        SettingsManager.singleton.FieldOfView = value / 10f;
        InputFieldUpdate();
    }
    public void ColourRSliderUpdate(float value) {
        SettingsManager.singleton.ColourR = value / 100f;
        InputFieldUpdate();
        ColoursUpdate();
    }
    public void ColourGSliderUpdate(float value) {
        SettingsManager.singleton.ColourG = value / 100f;
        InputFieldUpdate();
        ColoursUpdate();
    }
    public void ColourBSliderUpdate(float value) {
        SettingsManager.singleton.ColourB = value / 100f;
        InputFieldUpdate();
        ColoursUpdate();
    }
    public void AutoPickupToggleUpdate(bool value) {
        SettingsManager.singleton.AutoPickup = value;
    }
    public void RandomColour() {
        SettingsManager.singleton.ColourR = Random.Range(0f, 1f);
        SettingsManager.singleton.ColourG = Random.Range(0f, 1f);
        SettingsManager.singleton.ColourB = Random.Range(0f, 1f);

        SliderUpdate();
        ColoursUpdate();
    }
}
