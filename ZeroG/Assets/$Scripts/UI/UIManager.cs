using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {

    public static UIManager singleton { get; private set; }

    public GameObject lobbyMenu;
    public GameObject mainMenu;
    public GameObject pauseMenu;

    private GameObject currentlyOpen;

    public bool debugMode = true;

    Logger log;

    void Awake() {
        singleton = this;
        log = new Logger(debugMode);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.F12)) {
            UIDebugMenu.ToggleShow();
        }
    }

    public void OpenReplace(GameObject menu) {
        
        if (currentlyOpen) {
            currentlyOpen.SetActive(false);
        }
        
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
