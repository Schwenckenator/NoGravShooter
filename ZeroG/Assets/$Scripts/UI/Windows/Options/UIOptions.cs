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
        UIManager.singleton.SetMenuWindow(Menu.PlayerSettings);
    }

    public void CloseOptions() {
        SettingsManager.singleton.SaveSettings();

        if (GameManager.singleton.GameInProgress) {
            if (GameManager.IsSceneTutorial()) {
                UIManager.singleton.SetMenuWindow(Menu.TutorialMenu);
            } else {
                UIManager.singleton.SetMenuWindow(Menu.PauseMenu);
            }
        } else if(Network.isServer || Network.isClient){
            UIManager.singleton.SetMenuWindow(Menu.Lobby);
        }else {
            UIManager.singleton.SetMenuWindow(Menu.MainMenu);
        }
    }
    public void GoAudioSettings() {
        UIManager.singleton.SetMenuWindow(Menu.AudioSettings);
    }
    public void GoGraphicsSettings() {
        UIGraphicsSettings.instance.GraphicsOptionsButtonRefresh();
        UIManager.singleton.SetMenuWindow(Menu.GraphicsSettings);
    }
}
