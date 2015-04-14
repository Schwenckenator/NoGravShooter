using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIGraphicsSettings : MonoBehaviour {
    //Graphics
    private static List<Resolution> resolutions;
    private static Text btnResolutionText;

    private static int resolutionIndex = 0;
    public int resolutionMinWidth = 800;
    public int resolutionMinHeight = 600;


	// Use this for initialization
	void Start () {
        resolutions = ResolutionListPrune();

	}

    void GraphicsOptionsInit() {
        int maxIndex = resolutions.Count;
        for (int i = 0; i < maxIndex; i++) {
            if (Screen.height == resolutions[i].height && Screen.width == resolutions[i].width) {
                resolutionIndex = i;
            }
        }
        fullscreen = Screen.fullScreen;
        Canvas canvas = UIManager.GetCanvas(Menu.GraphicsSettings);
        Button[] buttons = canvas.GetComponentsInChildren<Button>(true);
        btnResolutionText = buttons[0].GetComponentInChildren<Text>();
        GraphicsOptionsButtonRefresh();
    }

    void GraphicsOptionsButtonRefresh() {
        btnResolutionText.text = resolutions[resolutionIndex].width.ToString() + " x " + resolutions[resolutionIndex].height.ToString();
    }
    public void ResolutionChange() {
        resolutionIndex++;
        resolutionIndex %= resolutions.Count;
        GraphicsOptionsButtonRefresh();
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
    }
}
