﻿using UnityEngine;
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

    public void SaveSettingsGoOption() {
        SettingsManager.instance.SaveSettings();
        UIManager.instance.SetMenuWindow(Menu.Options);
    }
    private void InputFieldUpdate() {
        inputFields[0].text = SettingsManager.instance.FieldOfView.ToString();
        inputFields[1].text = SettingsManager.instance.ColourR.ToString();
        inputFields[2].text = SettingsManager.instance.ColourG.ToString();
        inputFields[3].text = SettingsManager.instance.ColourB.ToString();
    }
    private void SliderUpdate() {
        sliders[0].value = SettingsManager.instance.FieldOfView * 10;
        sliders[1].value = SettingsManager.instance.ColourR * 100;
        sliders[2].value = SettingsManager.instance.ColourG * 100;
        sliders[3].value = SettingsManager.instance.ColourB * 100;
    }
    private void ColoursUpdate() {
        playerColours[0].ChangeColour(PlayerColourManager.instance.LimitTeamColour(TeamColour.Red, SettingsManager.instance.GetPlayerColour()));
        playerColours[1].ChangeColour(PlayerColourManager.instance.LimitTeamColour(TeamColour.None, SettingsManager.instance.GetPlayerColour()));
        playerColours[2].ChangeColour(PlayerColourManager.instance.LimitTeamColour(TeamColour.Blue, SettingsManager.instance.GetPlayerColour()));
    }
    private void ToggleUpdate() {
        toggles[0].isOn = SettingsManager.instance.AutoPickup;
    }

    public void FOVSliderUpdate(float value) {
        SettingsManager.instance.FieldOfView = value / 10f;
        InputFieldUpdate();
    }
    public void ColourRSliderUpdate(float value) {
        SettingsManager.instance.ColourR = value / 100f;
        InputFieldUpdate();
        ColoursUpdate();
    }
    public void ColourGSliderUpdate(float value) {
        SettingsManager.instance.ColourG = value / 100f;
        InputFieldUpdate();
        ColoursUpdate();
    }
    public void ColourBSliderUpdate(float value) {
        SettingsManager.instance.ColourB = value / 100f;
        InputFieldUpdate();
        ColoursUpdate();
    }
    public void AutoPickupToggleUpdate(bool value) {
        SettingsManager.instance.AutoPickup = value;
    }
    public void RandomColour() {
        SettingsManager.instance.ColourR = Random.Range(0f, 1f);
        SettingsManager.instance.ColourG = Random.Range(0f, 1f);
        SettingsManager.instance.ColourB = Random.Range(0f, 1f);

        SliderUpdate();
        ColoursUpdate();
    }
}
