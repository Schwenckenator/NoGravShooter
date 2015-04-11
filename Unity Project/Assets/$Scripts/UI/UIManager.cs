using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum Menu { ChangeKeybind, Connecting, CreateGame, Debug, EditKeybind, GameSettings, GraphicsSettings, JoinByIP, JoinGame, Lobby, MainMenu, Options, PasswordInput, PauseMenu, PlayerHUD }

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
    
    // Keybinds
    private static List<Text> keybindButtonText;
    private static int editedBinding = 0;

    //Graphics
    private static List<Resolution> resolutions;
    private Text btnResolutionText;

    private static int resolutionIndex = 0;
    public int resolutionMinWidth = 800;
    public int resolutionMinHeight = 600;
    
    void Start() {
        windows = new List<Canvas>();
        keybindButtonText = new List<Text>();
        resolutions = ResolutionListPrune();

        foreach (GameObject menu in menus) {
            //Create, then hide menu windows
            GameObject newMenu = Instantiate(menu) as GameObject;
            Canvas newCanvas = newMenu.GetComponent<Canvas>();
            newCanvas.enabled = false;
            windows.Add(newCanvas);
        }
        //Enable Main menu
        SetMenuWindow(Menu.MainMenu);
        windows[(int)Menu.Debug].enabled = false; // Don't show debug
        MenuWindowInit();
    }
    void OnGUI() { // Dirty old system, but I can't see a way around it
        if (windows[(int)Menu.ChangeKeybind].enabled) {
            ChangeKeybindUpdate();
        }
    }

    #region MenuWindowInit
    void MenuWindowInit() {
        MainMenuInit();
        CreateGameInit();
        EditKeybindInit();
        GraphicsOptionsInit();
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
    void EditKeybindInit() {
        Canvas editKeybind = GetCanvas(Menu.EditKeybind);
        Button[] buttons = editKeybind.gameObject.GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons) {
            Text butText = button.GetComponentInChildren<Text>();
            if (butText.text == "Back") continue;
            keybindButtonText.Add(butText);
        }
        EditKeybindTextRefresh();
    }
    void GraphicsOptionsInit() {
        int maxIndex = resolutions.Count;
        for (int i = 0; i < maxIndex; i++) {
            if (Screen.height == resolutions[i].height && Screen.width == resolutions[i].width) {
                resolutionIndex = i;
            }
        }
        fullscreen = Screen.fullScreen;
        Canvas canvas = GetCanvas(Menu.GraphicsSettings);
        Button[] buttons = canvas.GetComponentsInChildren<Button>(true);
        btnResolutionText = buttons[0].GetComponentInChildren<Text>();
        GraphicsOptionsButtonRefresh();
    }

    #endregion

    void EditKeybindTextRefresh() {
        for (int i = 0; i < keybindButtonText.Count; i++) {
            keybindButtonText[i].text = SettingsManager.keyBindings[i].ToString();
        }
    }
    void GraphicsOptionsButtonRefresh() {
        btnResolutionText.text = resolutions[resolutionIndex].width.ToString() + " x " + resolutions[resolutionIndex].height.ToString();
    }
    public void ResolutionChange() {
        resolutionIndex++;
        GraphicsOptionsButtonRefresh();
    }

    #region SetWindow
    public void SetMenuWindow(Menu newWindow) {
        SetMenuWindow((int)newWindow);
    }
    public void ShowMenuWindow(Menu newWindow, bool show) {
        ShowMenuWindow((int)newWindow, show);
    }
    
    private void SetMenuWindow(int newWindow) {
        windows[currentWindow].enabled = false;
        windows[newWindow].enabled = true;

        currentWindow = newWindow;
    }
    private void ShowMenuWindow(int newWindow, bool show) {
        windows[newWindow].enabled = show;
        windows[currentWindow].GetComponent<CanvasGroup>().interactable = !show;
    }
    #endregion

    #region MenuWindowMethods
    public void OpenChangeKeybind(int key) {
        editedBinding = key;
        ShowMenuWindow(Menu.ChangeKeybind, true);
    }
    public void CloseChangeKeybind() {
        ShowMenuWindow(Menu.ChangeKeybind, false);
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

    #region SaveSettings
    public bool fullscreen = false;
    public void SaveGraphicsSettings() {
        // Save settings logic
        Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, fullscreen);
        SetMenuWindow(Menu.Options);
    }
    public void SaveKeybinds() {
        SettingsManager.instance.SaveKeyBinds();
        SetMenuWindow(Menu.Options);
    }

    #endregion

    List<Resolution> ResolutionListPrune() {
        List<Resolution> resList = new List<Resolution>();
        Resolution[] resTemp = Screen.resolutions;

        foreach (Resolution res in resTemp) {
            if (res.width >= resolutionMinWidth && res.height >= resolutionMinHeight) {
                resList.Add(res);
            }
        }

        return resList;
    }



    void ChangeKeybindUpdate() {
        bool done = false;
        if (Event.current.isKey) {
            if (Event.current.keyCode != KeyCode.Escape) {
                SettingsManager.keyBindings[editedBinding] = Event.current.keyCode;
            }
            done = true;
        } else if (Event.current.shift) {
            SettingsManager.keyBindings[editedBinding] = KeyCode.LeftShift;
            done = true;
        }

        if (done) {
            EditKeybindTextRefresh();
            ShowMenuWindow(Menu.ChangeKeybind, false);
        }
    }

    public static Canvas GetCanvas(Menu value) {
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

    public void CreateGame(bool online) {
        SettingsManager.instance.ParsePortNumber();

        if (SettingsManager.instance.PortNum >= 0) { // Check for error
            NetworkManager.SetServerDetails(GameManager.MaxPlayers, SettingsManager.instance.PortNum, online);
            NetworkManager.InitialiseServer();
            SettingsManager.instance.SaveSettings();
            SetMenuWindow(Menu.Lobby);
        }
    }
}
