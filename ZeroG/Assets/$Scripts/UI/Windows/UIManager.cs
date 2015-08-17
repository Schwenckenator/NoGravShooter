using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum Menu { 
    ChangeKeybind, 
    Message, 
    CreateGame, 
    Debug, 
    EditKeybind, 
    GameSettings, 
    GraphicsSettings, 
    JoinByIP, 
    JoinGame, 
    Lobby, 
    MainMenu, 
    Options, 
    PasswordInput, 
    PauseMenu, 
    PlayerHUD, 
    TutorialMenu,
    GameClock,
    BloodyScreen,
    PlayerSettings,
    AudioSettings
}

public class UIManager : MonoBehaviour {
    #region Instance
    //Here is a private reference only this class can access
    private static UIManager _instance;
    //This is the public reference that other classes will use
    public static UIManager instance {
        get {
            //If _instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<UIManager>();
            }
            return _instance;
        }
    }
    #endregion

    
    public GameObject[] menus; // Only for initialisation
    private static List<GameObject> windows;
    private static int currentWindow = 0;

    private static TextChangeFromConnectionType[] textChangers;
    private static SelectableHideFromConnectionType[] buttonHiders;

    void Awake() {
        DontDestroyOnLoad(gameObject);

        windows = new List<GameObject>();
        foreach (GameObject menu in menus) {
            //Create, then hide menu windows
            GameObject newMenu = Instantiate(menu) as GameObject;
            DontDestroyOnLoad(newMenu);
            windows.Add(newMenu);
        }
        ListInit();
    }

    void Start() {
        windows[(int)Menu.Debug].gameObject.SetActive(false); // Don't show debug
        SetMenuWindow(Menu.MainMenu);
        
    }

    void Update() {
        GetKeyStrokes();
        GetDebugKeyStrokes();
    }
    void OnGUI() {
        if (DebugManager.IsAdminMode()) {
            GUI.Label(new Rect(Screen.width -100, 70, 100, 20), "ADMIN MODE");
        }
        if (DebugManager.IsDebugMode()) {
            GUI.Label(new Rect(Screen.width -100, 100, 100, 20), "DEBUG: On");
        }
        if (DebugManager.IsPaintballMode()) {
            GUI.Label(new Rect(Screen.width - 100, 40, 100, 20), "Paintball: On");
        }

    }
    private static void GetDebugKeyStrokes() {
        if (Input.GetKeyDown(KeyCode.F12)) {
            UIDebugMenu.ToggleShow();
        }
        if (Input.GetKeyDown(KeyCode.F1)) {
            DebugManager.SetPaintballMode(!DebugManager.IsPaintballMode());
        }
        //if (Input.GetKeyDown(KeyCode.F3)) {
        //    if (DebugManager.IsAdminMode()) {
        //        PlayerPrefs.DeleteAll();
        //        Debug.Log("PlayerPrefs Wiped!");
        //    }
        //}

    }
    private void GetKeyStrokes() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (GameManager.instance.IsPlayerSpawned()) {
                UIPauseSpawn.PauseMenuSwitch();
            }
        }
    }

    public static bool IsChangeKeybindWindow() {
        return windows[(int)Menu.ChangeKeybind].activeInHierarchy;
    }
    public static bool IsCurrentMenuWindow(Menu value) {
        return currentWindow == (int)value;
    }

    void ListInit() {
        textChangers = GameObject.FindObjectsOfType<TextChangeFromConnectionType>();
        buttonHiders = GameObject.FindObjectsOfType<SelectableHideFromConnectionType>();
    }

    #region SetWindow
    public void SetMenuWindow(Menu newWindow) {
        SetMenuWindow((int)newWindow);
    }
    public void ShowMenuWindow(Menu newWindow, bool show) {
        ShowMenuWindow((int)newWindow, show);
    }
    
    private void SetMenuWindow(int newWindow) {
        windows[currentWindow].gameObject.SetActive(false);
        windows[newWindow].gameObject.SetActive(true);

        currentWindow = newWindow;
    }
    private void ShowMenuWindow(int newWindow, bool show) {
        windows[newWindow].gameObject.SetActive(show);
        windows[currentWindow].GetComponent<CanvasGroup>().interactable = !show;
    }
    #endregion

    #region MenuWindowMethods
    public void GoMessage(bool value) {
        ShowMenuWindow(Menu.Message, value);
    }
    public void GoCreateGame() {
        SetMenuWindow(Menu.CreateGame);
    }
    public void GoDebug(bool value) {
        ShowMenuWindow(Menu.Debug, value);
    }
    public void GoEditKeybind() {
        SetMenuWindow(Menu.EditKeybind);
    }
    public void GoGameSettings(bool value) {
        ShowMenuWindow(Menu.GameSettings, value);
    }
    public void GoGraphicsSettings() {
        SetMenuWindow(Menu.GraphicsSettings);
    }
    public void GoJoinByIP(bool value) {
        ShowMenuWindow(Menu.JoinByIP, value);
    }
    public void GoJoinGame() {
        SetMenuWindow(Menu.JoinGame);
    }
    public void GoLobby() {
        SetMenuWindow(Menu.Lobby);
    }
    public void GoMainMenu() {
        SetMenuWindow(Menu.MainMenu);
    }
    public void GoOptions() {
        SetMenuWindow(Menu.Options);
    }
    public void GoPasswordInput(bool value) {
        ShowMenuWindow(Menu.PasswordInput, value);
    }
    public void GoPauseMenu() {
        SetMenuWindow(Menu.PauseMenu);
    }
    public void GoPlayerHUD() {
        SetMenuWindow(Menu.PlayerHUD);
    }
    #endregion

    public void SetPlayerName(string value) {
        SettingsManager.instance.PlayerName = value;
    }
    /// <summary>
    /// For the arrays with changing text based of network connection type, loop through and update the text for all.
    /// </summary>
    public void UpdateArraysFromNetworkConnection() {
        foreach (var text in textChangers) {
            text.UpdateText();
        }
        foreach (var hider in buttonHiders) {
            hider.UpdateVisibility();
        }
    }

    public static void RemoveAllGUI() {
        // Clear instantiated windows
        // Because I want to look at something I made.
        foreach (GameObject canvas in windows) {
            canvas.SetActive(false);
        }
    }
}
