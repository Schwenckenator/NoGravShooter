using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIGraphicsSettings : MonoBehaviour {
    //Graphics
    private static List<Resolution> resolutions;
    //private static Text btnResolutionText;
    public Text btnResolutionText;
    public GameObject resolutionButton;
    public RectTransform ComboParent;
    public GameObject mask;

    private static int resolutionIndex = 0;
    public int resolutionMinWidth = 800;
    public int resolutionMinHeight = 600;

    public static UIGraphicsSettings instance;
	// Use this for initialization
	void Start () {
        instance = this;
        resolutions = ResolutionListPrune();
        GraphicsOptionsInit();

        // Turn self off after initialsation
        gameObject.SetActive(false);
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
        int index = 0;
        foreach (Resolution res in resolutions) {
            GameObject buttonObj = Instantiate(resolutionButton);
            buttonObj.GetComponentInChildren<Text>().text = resolutions[index].width.ToString() + " x " + resolutions[index].height.ToString();
            buttonObj.transform.SetParent(ComboParent);
            ComboParent.sizeDelta = new Vector2(0, ComboParent.rect.height + 30);
            
            buttonObj.GetComponent<ButtonEventListener>().buttonIndex = index;
            
            index++;
        }
    }

    public void GraphicsOptionsButtonRefresh() {
        btnResolutionText.text = resolutions[resolutionIndex].width.ToString() + " x " + resolutions[resolutionIndex].height.ToString();
    }
    public void ResolutionChange() {
        resolutionIndex++;
        resolutionIndex %= resolutions.Count;
        GraphicsOptionsButtonRefresh();
    }
    public void ResolutionChange(int index) {
        resolutionIndex = index;
        mask.SetActive(false);
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
        //Screen.SetResolution(1280, 720, false);
    }
}
