using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

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

    public enum Menu { ChangeKeybind, Connecting, CreateGame, Debug, EditKeybind, GameSettings, GraphicsSettings, JoinByIP, JoinGame, Lobby, MainMenu, Options, PasswordInput, PauseMenu, PlayerHUD }
    public GameObject[] menus; // Only for initialisation
    private static List<Canvas> windows;
    private static int currentWindow = 0;

    void Start() {
        windows = new List<Canvas>();

        foreach (GameObject menu in menus) {
            //Create, then hide menu windows
            GameObject newMenu = Instantiate(menu) as GameObject;
            Canvas newCanvas = newMenu.GetComponent<Canvas>();
            newCanvas.enabled = false;
            windows.Add(newCanvas);
        }
        //Enable Main menu
        SetMenuWindow(Menu.MainMenu);
        ShowMenuWindow(Menu.Debug, true);
        MenuWindowInit();
    }

    #region MenuWindowInit
    void MenuWindowInit() {
        MainMenuInit();
        CreateGameInit();
    }
    void MainMenuInit() {
        Canvas mainMenu = GetCanvas(Menu.MainMenu);
        InputField playerName = mainMenu.gameObject.GetComponentInChildren<InputField>();
        playerName.text = SettingsManager.instance.PlayerName;
    }
    void CreateGameInit() {
        Canvas createGame = GetCanvas(Menu.CreateGame);
        InputField[] inputs = createGame.gameObject.GetComponentsInChildren<InputField>(true);
        inputs[0].text = SettingsManager.instance.ServerNameServer;
        inputs[1].text = SettingsManager.instance.PortNumStr;
        inputs[2].text = SettingsManager.instance.PasswordServer;
    }
    #endregion

    #region SetWindow
    public void SetMenuWindow(Menu newWindow) {
        SetMenuWindow((int)newWindow);
    }
    public void ShowMenuWindow(Menu newWindow, bool show) {
        ShowMenuWindow((int)newWindow, show);
    }
    
    public void SetMenuWindow(int newWindow) {
        windows[currentWindow].enabled = false;
        windows[newWindow].enabled = true;

        currentWindow = newWindow;
    }
    public void ShowMenuWindow(int newWindow, bool show) {
        windows[newWindow].enabled = show;
    }
    #endregion

    #region MenuWindowMethods
    public void GoChangeKeybind(bool value) {
        ShowMenuWindow(Menu.ChangeKeybind, value);
    }
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

    public Canvas GetCanvas(Menu value) {
        return windows[(int)value];
    }
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

    private static void RemoveAllGUI() {
        // If editor, clear instantiated windows
        // Because I want to look at something I made.
        foreach (Canvas canvas in windows) {
            canvas.enabled = false;
        }
    }
}
