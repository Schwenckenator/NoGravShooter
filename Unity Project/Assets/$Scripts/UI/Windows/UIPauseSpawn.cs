﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIPauseSpawn : MonoBehaviour {

    private static ChangeableText spawnButton;
    private static string spawn = "Spawn";
    private static string unpause = "Return to Game";

    private static ChangeableText serverName;

    public static void Init() {
        Canvas canvas = UIManager.GetCanvas(Menu.PauseMenu);
        FindTexts(canvas);
        
        PlayerDied(); // Initialises button text
    }

    /// <summary>
    /// Finds the Changeable texts in the canvas, and assigns variables
    /// </summary>
    /// <param name="canvas"></param>
    private static void FindTexts(Canvas canvas) {
        ChangeableText[] texts = canvas.GetComponentsInChildren<ChangeableText>();

        foreach (ChangeableText text in texts) {
            if (text.IsType("spawnButton")) {
                spawnButton = text;
            } else if (text.IsType("serverName")) {
                serverName = text;
            }
        }
    }


    public static void PlayerSpawned() {
        spawnButton.SetText(unpause);
        if (GameManager.instance.GameInProgress)
            ReturnToGame();
    }
    public static void PlayerDied() {
        spawnButton.SetText(spawn);
        if (GameManager.instance.GameInProgress)
            PauseMenu();
    }

    public static void SetServerNameText(string newText) {
        serverName.SetText(newText);
    }

    private static void ReturnToGame() {
        UIManager.instance.SetMenuWindow(Menu.PlayerHUD);
        GameManager.SetCursorVisibility(false);
        GameManager.instance.SetPlayerMenu(false);
    }
    private static void PauseMenu() {
        UIManager.instance.SetMenuWindow(Menu.PauseMenu);
        GameManager.SetCursorVisibility(true);
        GameManager.instance.SetPlayerMenu(true);
    }

    /// <summary>
    /// Switches between paused and not paused, based on state
    /// </summary>
    public static void PauseMenuSwitch() {
        if(UIManager.IsCurrentMenuWindow(Menu.PauseMenu)){
            ReturnToGame();
        } else {
            PauseMenu();
        }
    }

    public void PauseSpawnPress() {
        Debug.Log("Spawn button pressed");
        if (GameManager.instance.IsPlayerSpawned()) {
            PauseMenuSwitch();
        } else {
            GameManager.instance.SpawnActor();
        }
    }
    public void ReturnToLobbyPress() {
        GameManager.instance.ReturnToLobby();
        UIManager.instance.SetMenuWindow(Menu.Lobby);
    }
}
