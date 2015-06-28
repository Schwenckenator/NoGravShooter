﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum Menu { ChangeKeybind, Connecting, CreateGame, Debug, EditKeybind, GameSettings, GraphicsSettings, JoinByIP, JoinGame, Lobby, MainMenu, Options, PasswordInput, PauseMenu, PlayerHUD, TutorialMenu }

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
    private static List<Canvas> windows;
    private static int currentWindow = 0;

    private static TextChangeFromConnectionType[] textChangers;
    private static SelectableHideFromConnectionType[] buttonHiders;

    void Awake() {
        DontDestroyOnLoad(gameObject);

        windows = new List<Canvas>();
        foreach (GameObject menu in menus) {
            //Create, then hide menu windows
            GameObject newMenu = Instantiate(menu) as GameObject;
            DontDestroyOnLoad(newMenu);
            Canvas newCanvas = newMenu.GetComponent<Canvas>();
            windows.Add(newCanvas);
        }
    }

    void Start() {
        windows[(int)Menu.Debug].gameObject.SetActive(false); // Don't show debug
        MenuWindowInit();
    }

    void Update() {
        GetKeyStrokes();
        GetDebugKeyStrokes();
    }
    void OnGUI() {
        if (DebugManager.IsAdminMode()) {
            GUI.Label(new Rect(Screen.width -100, 70, 100, 20), "TEST MODE");
        }
        if (DebugManager.IsDebugMode()) {
            GUI.Label(new Rect(Screen.width -100, 100, 100, 20), "DEBUG: On");
        }
    }
    private static void GetDebugKeyStrokes() {
        if (Input.GetKeyDown(KeyCode.F1)) {
            DebugManager.ToggleTestMode();
        }
        if (Input.GetKeyDown(KeyCode.F2)) {
            DebugManager.ToggleDebugMode();
        }
        if (Input.GetKeyDown(KeyCode.F3)) {
            if (DebugManager.IsAdminMode()) {
                PlayerPrefs.DeleteAll();
                Debug.Log("PlayerPrefs Wiped!");
            }
        }

    }
    private void GetKeyStrokes() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (GameManager.instance.IsPlayerSpawned()) {
                UIPauseSpawn.PauseMenuSwitch();
            }
        }
    }

    public static bool IsChangeKeybindWindow() {
        return windows[(int)Menu.ChangeKeybind].gameObject.activeInHierarchy;
    }
    public static Canvas GetCanvas(Menu value) {
        return windows[(int)value];
    }
    public static bool IsCurrentMenuWindow(Menu value) {
        return currentWindow == (int)value;
    }
    public static List<IChangeable> FindChangeables(Canvas canvas) {
        List<IChangeable> changers = new List<IChangeable>();
        MovingBar[] bars = canvas.GetComponentsInChildren<MovingBar>();
        changers.AddRange(bars);
        ChangeableText[] texts = canvas.GetComponentsInChildren<ChangeableText>();
        changers.AddRange(texts);
        ChangeableImage[] images = canvas.GetComponentsInChildren<ChangeableImage>();
        changers.AddRange(images);
        ChangeableInputField[] fields = canvas.GetComponentsInChildren<ChangeableInputField>();
        changers.AddRange(fields);
        return changers;
    }

    #region MenuWindowInit
    void MenuWindowInit() {
        MainMenuInit();
        UIChat.FindChatBoxes();
        ListInit();
<<<<<<< HEAD:Unity Project/Assets/$Scripts/UI/Windows/UIManager.cs

        gameObject.SendMessage("UIWindowInitialised", SendMessageOptions.RequireReceiver);
=======
        PauseMenuInit();
        InitUI();
>>>>>>> origin/Unity5Upgrade:ZeroG/Assets/$Scripts/UI/Windows/UIManager.cs
    }
    void MainMenuInit() {
        Canvas mainMenu = GetCanvas(Menu.MainMenu);
        InputField playerName = mainMenu.gameObject.GetComponentInChildren<InputField>();
        playerName.text = SettingsManager.instance.PlayerName;
    }
    void ListInit() {
        textChangers = GameObject.FindObjectsOfType<TextChangeFromConnectionType>();
        buttonHiders = GameObject.FindObjectsOfType<SelectableHideFromConnectionType>();
    }
    void HideWindows() {
        foreach (Canvas window in windows) {
            window.gameObject.SetActive(false);
        }
        //Enable Main menu
        SetMenuWindow(Menu.MainMenu);
    }
    /// <summary>
    /// Hide all menu windows, then set menu window as Main menu
    /// </summary>
    void InitUI() {
        foreach (var ui in windows) {
            ui.gameObject.SetActive(false);
        }
        //Enable Main menu
        SetMenuWindow(Menu.MainMenu);
    }
    #endregion

    #region MainMenuMethods
    public void LoadTutorial() {
        GameManager.instance.LoadTutorial();
    }
    public void QuitGame() {
        if (!Application.isWebPlayer && !Application.isEditor) {
            Application.Quit();
        } else {
            RemoveAllGUI();
        }
    }
    #endregion

    #region SetWindow
    public void SetMenuWindow(Menu newWindow) {
        SetMenuWindow((int)newWindow);
        if (newWindow == Menu.JoinGame) {
            UIJoinGame.instance.StartRefresh();
        }
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
    public void GoConnecting() {
        SetMenuWindow(Menu.Connecting);
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
        // If editor, clear instantiated windows
        // Because I want to look at something I made.
        foreach (Canvas canvas in windows) {
            canvas.gameObject.SetActive(false);
        }
    }
}