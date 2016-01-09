using UnityEngine;
using System.Collections;

public class OnLevelOpen : MonoBehaviour {

    public GameObject menu;
    public GameObject game;


    void Start() {
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame() {
        yield return null; // Wait 2 frames
        yield return null;
        GoMenu(); // Always starts on menu
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
