using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class UIGraphicsSettings : MonoBehaviour {
    //Graphics
    private static List<Resolution> resolutions;

    private static int resolutionIndex = 0;
    public int resolutionMinWidth = 800;
    public int resolutionMinHeight = 600;

    public static UIGraphicsSettings instance;

    public Dropdown dropdown;
	// Use this for initialization
	void Start () {
        instance = this;
        resolutions = ResolutionListPrune();
        GraphicsOptionsInit();
	}

    void GraphicsOptionsInit() {
        int maxIndex = resolutions.Count;
        for (int i = 0; i < maxIndex; i++) {
            if (Screen.height == resolutions[i].height && Screen.width == resolutions[i].width) {
                resolutionIndex = i;
            }
        }
        fullscreen = Screen.fullScreen;
        
        GraphicsOptionsButtonRefresh();
        CreateDropdownButtons();
    }

    private void CreateDropdownButtons() {
        foreach (Resolution res in resolutions) {
            dropdown.options.Add(new Dropdown.OptionData(res.width.ToString() + " x " + res.height.ToString()));
        }
    }
    public void GraphicsOptionsButtonRefresh() {
        dropdown.value = resolutionIndex;
    }
    public void ResolutionChange(int index) {
        resolutionIndex = index;
    }

    List<Resolution> ResolutionListPrune() {
        List<Resolution> resList = new List<Resolution>();
        Resolution[] resTemp = Screen.resolutions;

        foreach (Resolution res in resTemp) {
            if ((res.width >= resolutionMinWidth && res.height >= resolutionMinHeight) || Application.isEditor) {
                resList.Add(res);
            }
        }

        return resList;
    }

    private static bool fullscreen = false;
    public void SetFullscreen(bool value) {
        fullscreen = value;
    }
    public void SaveGraphicsSettings() {
        // Save settings logic
        Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, fullscreen);

        // This is workaround for Canvas rendering bug
        SceneManager.LoadScene(0); // Reloads menu scene
    }
}
