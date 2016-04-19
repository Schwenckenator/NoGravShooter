using UnityEngine;
using System.Collections;

public class UISpawner : MonoBehaviour {

    public GameObject menuWindows;
    public GameObject gameMenu;

    private GameObject _menuWindows;
    private GameObject _gameMenu;
    // Use this for initialization
    void Start () {
        //Always start on menu
        _menuWindows = Instantiate(menuWindows);
        _gameMenu = Instantiate(gameMenu);
        LoadWindows(true);
    }

    void OnLevelWasLoaded() {
        if (LobbyManager.IsSceneMenu) {
            LoadWindows(true);
        } else {
            LoadWindows(false);
        }
    }

    void LoadWindows(bool isMenu) {
        _menuWindows.SetActive(isMenu);
        _gameMenu.SetActive(!isMenu);
    }
	
}
