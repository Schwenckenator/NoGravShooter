using UnityEngine;
using System.Collections;

public class GameMenu : MonoBehaviour {

    public GameObject HUD;
    public GameObject menu;

    private static GameMenu singleton;

    void Awake() {
        singleton = this;
    }
	
	// Update is called once per frame
	void Update () {
	    if (!LobbyManager.IsSceneMenu && Input.GetKeyDown(KeyCode.Escape)) {
            if (UIPauseSpawn.IsShown) {
                OpenHUD();
            } else {
                OpenMenu();
            }
        }
	}

    public static void OpenMenu() {
        UIManager.SetCursorVisibility(true);
        UIManager.singleton.OpenReplace(singleton.menu);
    }
    public static void OpenHUD() {
        UIManager.SetCursorVisibility(false);
        UIManager.singleton.OpenReplace(singleton.HUD);
    }
}
