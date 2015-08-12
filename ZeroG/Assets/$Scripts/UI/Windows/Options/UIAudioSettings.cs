using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIAudioSettings : MonoBehaviour {

    private static InputField[] inputFields;
    private static Slider[] sliders;

	// Use this for initialization
	void Start () {
        sliders = GetComponentsInChildren<Slider>(true);
        inputFields = GetComponentsInChildren<InputField>(true);

        SliderUpdate();
        InputFieldUpdate();

        gameObject.SetActive(false);
	}

    private void InputFieldUpdate() {
        inputFields[0].text = (SettingsManager.instance.VolumeMaster * 100).ToString();
        inputFields[1].text = (SettingsManager.instance.VolumeMusic * 100).ToString();
        inputFields[2].text = (SettingsManager.instance.VolumeEffects * 100).ToString();
    }
    private void SliderUpdate() {
        sliders[0].value = SettingsManager.instance.VolumeMaster * 100;
        sliders[1].value = SettingsManager.instance.VolumeMusic * 100;
        sliders[2].value = SettingsManager.instance.VolumeEffects * 100;
    }

    public void MasterVolume(float value) {
        SettingsManager.instance.VolumeMaster = value / 100f;
        InputFieldUpdate();
        SetAudioLevels();
    }
    public void MusicVolume(float value) {
        SettingsManager.instance.VolumeMusic = value / 100f;
        InputFieldUpdate();
        SetAudioLevels();
    }
    public void EffectsVolume(float value) {
        SettingsManager.instance.VolumeEffects = value / 100f;
        InputFieldUpdate();
        SetAudioLevels();
    }
    public void SaveSettingsGoOption() {
        SettingsManager.instance.SaveSettings();
        UIManager.instance.SetMenuWindow(Menu.Options);
    }

    void SetAudioLevels() {
        AudioMasterVolume.UpdateMasterVolume();
        AudioMusicVolume.UpdateMusicVolume();
    }
}
