using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIPauseSpawn : MonoBehaviour {

    private static ChangeableText spawnButton;
    private static string spawn = "Spawn";
    private static string unpause = "Return to Game";

    private static ChangeableText serverName;

    private static IHideable playerList;
    private static IHideable returnToLobby;

    void Awake() {
        Init();

        //gameObject.SendMessage("UIWindowInitialised", SendMessageOptions.RequireReceiver);
    }

    public static void Init() {
        Canvas canvas = UIManager.GetCanvas(Menu.PauseMenu);
        FindTexts(canvas);
        FindHideables(canvas);
        
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

    private static void FindHideables(Canvas canvas) {
        IChangeable[] changeables = canvas.gameObject.GetInterfacesInChildren<IChangeable>();

        foreach (IChangeable changer in changeables) {
            if (changer.IsType("playerListHide")) {
                playerList = changer as IHideable;
            } else if (changer.IsType("returnToLobby")) {
                returnToLobby = changer as IHideable;
            }
        }
    }


    public static void PlayerSpawned() {
        spawnButton.SetText(unpause);
        if (GameManager.instance.GameInProgress || GameManager.IsSceneTutorial())
            ReturnToGame();
    }
    public static void PlayerDied() {
        spawnButton.SetText(spawn);
        if (GameManager.instance.GameInProgress || GameManager.IsSceneTutorial())
            PauseMenu();
    }

    public static void SetServerNameText() {
        string newText = SettingsManager.instance.ServerNameClient;
        newText += ", " + SettingsManager.instance.GameModeName;
        serverName.SetText(newText);
    }
    public static void TutorialModeActive(bool isTutorial) {
        playerList.Show(!isTutorial);
        returnToLobby.Show(!isTutorial);
    }
    private static void ReturnToGame() {
        UIManager.instance.SetMenuWindow(Menu.PlayerHUD);
        GameManager.SetCursorVisibility(false);
        GameManager.instance.SetPlayerMenu(false);
    }
    private static void PauseMenu(bool isTutorial = false) {
        Menu window = isTutorial ? Menu.TutorialMenu : Menu.PauseMenu;
        UIManager.instance.SetMenuWindow(window);
        GameManager.SetCursorVisibility(true);
        GameManager.instance.SetPlayerMenu(true);
    }

    /// <summary>
    /// Switches between paused and not paused, based on state
    /// </summary>
    public static void PauseMenuSwitch() {
        if(UIManager.IsCurrentMenuWindow(Menu.PlayerHUD)){
            PauseMenu(GameManager.IsSceneTutorial()); 
        } else {
            ReturnToGame();
        }
    }

    public void PauseSpawnPress() {
        if (GameManager.instance.IsPlayerSpawned()) {
            PauseMenuSwitch();
        } else {
            GameManager.instance.SpawnActor();
        }
    }
    public void ReturnToLobbyPress() {
        GameManager.instance.ReturnToLobby();
    }
}
