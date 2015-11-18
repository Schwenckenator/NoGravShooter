using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {

    public static UIManager singleton { get; private set; }

    public GameObject initiallyOpen;
    public GameObject connectedOpen;

    private GameObject currentlyOpen;

    public bool debugMode = true;

    void Start() {
        singleton = this;
        currentlyOpen = NetworkManager.single.isNetworkActive ? connectedOpen : initiallyOpen;
        Open(currentlyOpen);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.F12)) {
            UIDebugMenu.ToggleShow();
        }
    }

    public void OpenReplace(GameObject menu) {
        currentlyOpen.SetActive(false);
        menu.SetActive(true);
        currentlyOpen = menu;
    }
    public void Open(GameObject menu) {
        menu.SetActive(true);
    }
    public void Close(GameObject menu) {
        menu.SetActive(false);
    }

    public void SaveSettings() {
        SettingsManager.singleton.SaveSettings();
    }
    public void SaveKeybinds() {
        SettingsManager.singleton.SaveKeyBinds();
    }
}
