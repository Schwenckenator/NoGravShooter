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
    private static List<Text> keybindButtonText;
    private static int editedBinding = 0;
    
    // Option
    private static InputField[] optionInputFields;
    private static Slider[] optionSliders;
    private static OptionColourBox[] optionPlayerColours;


    void Start() {
        windows = new List<Canvas>();
        keybindButtonText = new List<Text>();

        foreach (GameObject menu in menus) {
            //Create, then hide menu windows
            GameObject newMenu = Instantiate(menu) as GameObject;
            Canvas newCanvas = newMenu.GetComponent<Canvas>();
            newCanvas.enabled = false;
            windows.Add(newCanvas);
        }
        //Enable Main menu
        SetMenuWindow(Menu.MainMenu);
        windows[(int)Menu.Debug].enabled = true;
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
        OptionsInit();
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
    void OptionsInit() {
        Canvas options = GetCanvas(Menu.Options);
        optionSliders = options.gameObject.GetComponentsInChildren<Slider>(true);
        optionInputFields = options.gameObject.GetComponentsInChildren<InputField>(true);
        optionPlayerColours = options.gameObject.GetComponentsInChildren<OptionColourBox>(true);
        // Order
        /* Mouse sen X
         * Mouse sen Y
         * Fov
         * Colour Red
         * Colour Green
         * Colour Blue
         */

        OptionsSliderUpdate();
        OptionsInputFieldUpdate();
        OptionsColoursUpdate();
}

    #endregion

    void EditKeybindTextRefresh() {
        for (int i = 0; i < keybindButtonText.Count; i++) {
            keybindButtonText[i].text = SettingsManager.keyBindings[i].ToString();
        }
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
    public void SaveGraphicsSettings() {
        // Save settings logic
        SetMenuWindow(Menu.Options);
    }
    public void SaveKeybinds() {
        SettingsManager.instance.SaveKeyBinds();
        SetMenuWindow(Menu.Options);
    }
    public void SaveOptions() {
        SettingsManager.instance.SaveSettings();
        SetMenuWindow(Menu.MainMenu);
    }
    #endregion

    #region Options

    private void OptionsInputFieldUpdate() {
        optionInputFields[0].text = SettingsManager.instance.MouseSensitivityX.ToString();
        optionInputFields[1].text = SettingsManager.instance.MouseSensitivityY.ToString();
        optionInputFields[2].text = SettingsManager.instance.FieldOfView.ToString();
        optionInputFields[3].text = SettingsManager.instance.ColourR.ToString();
        optionInputFields[4].text = SettingsManager.instance.ColourG.ToString();
        optionInputFields[5].text = SettingsManager.instance.ColourB.ToString();
    }
    private void OptionsSliderUpdate() {
        optionSliders[0].value = SettingsManager.instance.MouseSensitivityX * 100;
        optionSliders[1].value = SettingsManager.instance.MouseSensitivityY * 100;
        optionSliders[2].value = SettingsManager.instance.FieldOfView * 10;
        optionSliders[3].value = SettingsManager.instance.ColourR * 100;
        optionSliders[4].value = SettingsManager.instance.ColourG * 100;
        optionSliders[5].value = SettingsManager.instance.ColourB * 100;
    }
    private void OptionsColoursUpdate() {
        optionPlayerColours[0].ChangeColour(PlayerColourManager.instance.LimitTeamColour(TeamColour.Red, SettingsManager.instance.GetPlayerColour()));
        optionPlayerColours[1].ChangeColour(PlayerColourManager.instance.LimitTeamColour(TeamColour.None, SettingsManager.instance.GetPlayerColour()));
        optionPlayerColours[2].ChangeColour(PlayerColourManager.instance.LimitTeamColour(TeamColour.Blue, SettingsManager.instance.GetPlayerColour()));
    }

    public void MouseSenXSliderUpdate(float value) {
        SettingsManager.instance.MouseSensitivityX = value / 100f;
        OptionsInputFieldUpdate();
    }
    public void MouseSenYSliderUpdate(float value) {
        SettingsManager.instance.MouseSensitivityY = value / 100f;
        OptionsInputFieldUpdate();
    }
    public void FOVSliderUpdate(float value) {
        SettingsManager.instance.FieldOfView = value / 10f;
        OptionsInputFieldUpdate();
    }
    public void ColourRSliderUpdate(float value) {
        SettingsManager.instance.ColourR = value / 100f;
        OptionsInputFieldUpdate();
        OptionsColoursUpdate();
    }
    public void ColourGSliderUpdate(float value) {
        SettingsManager.instance.ColourG = value / 100f;
        OptionsInputFieldUpdate();
        OptionsColoursUpdate();
    }
    public void ColourBSliderUpdate(float value) {
        SettingsManager.instance.ColourB = value / 100f;
        OptionsInputFieldUpdate();
        OptionsColoursUpdate();
    }
    #endregion
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
