using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIPauseSpawn : MonoBehaviour {

    // TODO
    public static bool IsShown {
        get {
            if (myObj == null) return false;
            return myObj.activeInHierarchy;
        }
    }
    public GameObject playerHUD;
    public Toggle AutoSpawn;

    public Text ShutdownButtonText;

    private static ChangeableText spawnButton;
    private static string spawn = "Spawn";
    private static string unpause = "Return to Game";

    private static ChangeableText serverName;
    

    private static IHideable playerList;
    private static IHideable returnToLobby;

    static GameObject myObj;
    static UIPauseSpawn singleton;

    void Start() {
        singleton = this;
        myObj = gameObject;
        Init();
    }

    public void Init() {
        
        FindTexts();
        FindHideables();
        SetToggle();
        
        PlayerDied(); // Initialises button text
    }

    /// <summary>
    /// Finds the Changeable texts in the canvas, and assigns variables
    /// </summary>
    /// <param name="canvas"></param>
    private void FindTexts() {
        ChangeableText[] texts = GetComponentsInChildren<ChangeableText>();

        foreach (ChangeableText text in texts) {
            if (text.IsType("spawnButton")) {
                spawnButton = text;
            } else if (text.IsType("serverName")) {
                serverName = text;
            }
        }
    }

    private void FindHideables() {
        IChangeable[] changeables = gameObject.GetInterfacesInChildren<IChangeable>();

        foreach (IChangeable changer in changeables) {
            if (changer.IsType("playerListHide")) {
                playerList = changer as IHideable;
            } else if (changer.IsType("returnToLobby")) {
                returnToLobby = changer as IHideable;
            }
        }
    }
    private void SetToggle() {
        if (AutoSpawn != null) {
            AutoSpawn.isOn = SettingsManager.singleton.AutoSpawn;
        }
    }
    public void ToggleAutoSpawn(bool value) {
        SettingsManager.singleton.AutoSpawn = value;
        SettingsManager.singleton.SaveSettings();
    }

    public static void PlayerSpawned() {
        spawnButton.SetText(unpause);
        GameMenu.OpenHUD();
    }
    public static void PlayerDied() {
        spawnButton.SetText(spawn);
        PauseMenu();
    }

    public static void SetServerNameText() {
        string newText = NetworkInfoWrapper.singleton.ServerName;
        newText += ", " + NetworkInfoWrapper.singleton.GameModeName;
        serverName.SetText(newText);
    }
    public static void TutorialModeActive(bool isTutorial) {
        playerList.Show(!isTutorial);
        returnToLobby.Show(!isTutorial);
    }
    private static void PauseMenu(bool isTutorial = false) {
        if (!isTutorial) {
            returnToLobby.Show(NetworkManager.isServer);
            singleton.ShutdownButtonText.text = NetworkManager.isServer ? "Shutdown Server" : "Disconnect";
        }
        GameMenu.OpenMenu();
    }

    /// <summary>
    /// Switches between paused and not paused, based on state
    /// </summary>
    public static void PauseMenuSwitch() {
        if(!IsShown){
            PauseMenu(GameManager.IsSceneTutorial()); 
        } else {
            GameMenu.OpenHUD();
        }
    }

    public void PauseSpawnPress() {
        if (ActorManager.isMyActorSpawned) {
            PauseMenuSwitch();
        } else {
            ActorManager.singleton.SpawnActor();
        }
    }
    public void ReturnToLobbyPress() {
        GameManager.singleton.ReturnToLobby();
    }
    public void Disconnect() {
        NetworkManager.Disconnect();
    }
}
