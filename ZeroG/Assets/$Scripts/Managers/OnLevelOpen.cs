using UnityEngine;
using System.Collections;

public class OnLevelOpen : MonoBehaviour {

    public GameObject menu;
    public GameObject game;



    void Start() {
        // Always starts on menu
        Invoke("GoMenu", 0.1f); // Wait a god damn second
    }

    void OnLevelWasLoaded() {
        if (GameManager.IsSceneMenu()) {
            GoMenu();
        } else {
            GoGame();
        }
    }

    void GoGame() {
        SetMenu(false);
        UIManager.singleton.OpenReplace(UIManager.singleton.pauseMenu);
    }
    void GoMenu() {
        SetMenu(true);
        if (NetworkManager.single.isNetworkActive) {
            UIManager.singleton.OpenReplace(UIManager.singleton.lobbyMenu);
        } else {
            UIManager.singleton.OpenReplace(UIManager.singleton.mainMenu);
        }
    }
    void SetMenu(bool value) {
        menu.SetActive(value);
        game.SetActive(!value);
    }
}
