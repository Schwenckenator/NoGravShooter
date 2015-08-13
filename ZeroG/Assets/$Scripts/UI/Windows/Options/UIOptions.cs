using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIOptions : MonoBehaviour {

    // Option
    private static InputField[] inputFields;
    private static Slider[] sliders;
    private static OptionColourBox[] playerColours;
    private static Toggle[] toggles;

	// Use this for initialization
	void Start () {
        // Turn self off after initialsation
        gameObject.SetActive(false);
	}
	

    public void GoPlayerSettings() {
        UIManager.instance.SetMenuWindow(Menu.PlayerSettings);
    }

    public void SaveOptionsGoMainMenu() {
        SettingsManager.instance.SaveSettings();
        UIManager.instance.SetMenuWindow(Menu.MainMenu);
    }
    public void GoAudioSettings() {
        UIManager.instance.SetMenuWindow(Menu.AudioSettings);
    }
    public void GoGraphicsSettings() {
        UIGraphicsSettings.instance.GraphicsOptionsButtonRefresh();
        UIManager.instance.SetMenuWindow(Menu.GraphicsSettings);
    }
}
