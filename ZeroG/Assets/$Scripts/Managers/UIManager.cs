using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {

    public static UIManager singleton { get; private set; }

    public GameObject connectedMenu;
    public GameObject mainMenu;

    private GameObject currentlyOpen;

    public bool debugMode = true;

    Logger log;

    static public void SetCursorVisibility(bool visible) {
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = visible;
    }

    void Awake() {
        singleton = this;
        log = new Logger(debugMode);
    }

    void Start() {
        if (LobbyManager.s_Singleton.isNetworkActive) {
            OpenConnected();
        } else {
            OpenReplace(mainMenu);
        }
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

    public void OpenConnected() {
        OpenReplace(connectedMenu);
    }
}
