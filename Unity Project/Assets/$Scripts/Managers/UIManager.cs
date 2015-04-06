using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    public enum Menu { MainMenu = 0, CreateGame, JoinGame, Options, Quit, Lobby, GameSettings, JoinByIP, Connecting, Keybind, ChangeKeybind, GraphicsOptions, PasswordInput }
    public GameObject[] menus; // Only for initialisation
    private static List<Canvas> windows;
    private static int currentWindow;

    void Start() {
        windows = new List<Canvas>();

        SetCurrentWindow(Menu.MainMenu);

        foreach (GameObject menu in menus) {
            //Create, then hide menu windows
            GameObject newMenu = Instantiate(menu) as GameObject;
            Canvas newCanvas = newMenu.GetComponent<Canvas>();
            newCanvas.enabled = false;
            windows.Add(newCanvas);
        }
        //Enable Main menu
        windows[0].enabled = true;
    }
    #region SetWindow
    private static void SetCurrentWindow(Menu menu){
        // Only changes currentWindow
        currentWindow = (int)menu;
    }
    public void SetMenuWindow(Menu newWindow) {
        SetMenuWindow((int)newWindow);
    }
    public void SetMenuWindow(int newWindow) {
        windows[currentWindow].enabled = false;
        windows[newWindow].enabled = true;

        currentWindow = newWindow;
    }
    #endregion

    public void QuitGame() {
        if (!Application.isWebPlayer && !Application.isEditor) {
            Application.Quit();
        } else {
            // If editor, clear instantiated windows
            // Because I want to look at something I made.
            foreach (Canvas canvas in windows) {
                canvas.enabled = false;
            }
        }
    }
}
