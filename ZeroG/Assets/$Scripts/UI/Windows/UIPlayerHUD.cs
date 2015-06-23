using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIPlayerHUD : MonoBehaviour {

    static MovingBar health;
    static MovingBar fuel;
    static ChangeableText ammo;
    static ChangeableText grenade;
    static ChangeableText promptText;
    static ChangeableText tutorialPromptText;
    
    static ChangeableImage crosshair;
    static ChangeableImage sniperScope;
    static ChangeableImage sniperBackground;

    static PlayerResources playerResource;
    static WeaponInventory weaponInventory;
    static IDamageable playerHealth;
    static IActorStats ActorStats;

    static IHideable playerUI;
    static IHideable sniperUI;
    static IHideable promptUI;
    static IHideable tutorialPromptUI;

    static bool tutorialPromptActive = false;

    void Awake() {
        Init();
    }
    public static void Init() {
        Canvas canvas = UIManager.GetCanvas(Menu.PlayerHUD);
        IChangeable[] changers = canvas.gameObject.GetInterfacesInChildren<IChangeable>();

        foreach (IChangeable changer in changers) {
            if (changer.IsType("fuel")) fuel = changer as MovingBar;
            else if (changer.IsType("health")) health = changer as MovingBar;
            else if (changer.IsType("ammo")) ammo = changer as ChangeableText;
            else if (changer.IsType("grenade")) grenade = changer as ChangeableText;
            else if (changer.IsType("promptText")) promptText = changer as ChangeableText;
            else if (changer.IsType("promptHide")) promptUI = changer as HideableUI;
            else if (changer.IsType("playerUI")) playerUI = changer as HideableUI;
            else if (changer.IsType("sniperUI")) sniperUI = changer as HideableUI;
            else if (changer.IsType("tutorialPromptText")) tutorialPromptText = changer as ChangeableText;
            else if (changer.IsType("tutorialPromptUI")) tutorialPromptUI = changer as HideableUI;
        }

        health.SetMaxValue(ActorHealth.GetDefaultMaxHealth());
        fuel.SetMaxValue(PlayerResources.GetMaxFuel());
    }
    void Start() {
        ShowSniperScope(false); // Set to default
        RemoveTutorialPrompt();
        RemovePrompt();
    }

    public static void ShowSniperScope(bool showSniper) {
        playerUI.Show(!showSniper);
        GameClock.ShowClock(!showSniper);
        sniperUI.Show(showSniper);
    }

    public static void SetupPlayer(GameObject actor) {
        playerResource = actor.GetComponent<PlayerResources>();
        playerHealth = actor.GetInterface<IDamageable>();
        weaponInventory = actor.GetComponent<WeaponInventory>();

        health.SetMaxValue(playerHealth.GetMaxHealth());
    }

    void Update() {
        if(GameManager.instance.IsPlayerSpawned()){
            float temp = playerHealth.GetHealth();
            health.SetValue(temp);
            fuel.SetValue(playerResource.GetFuel());

            ammo.SetText(MakeAmmoString());
            grenade.SetText(MakeGrenadeString());
        }

        // TEST
        if (Input.GetKeyDown(KeyCode.F5)) {
            TutorialPrompt("This is a test prompt.");
        }

        if (tutorialPromptActive && (Input.GetAxisRaw("Submit") > 0)) {
            RemoveTutorialPrompt();
        }
    }

    void FixedUpdate() {
        if (promptTimer > 1) {
            promptTimer--;
        } else if (promptTimer == 1) {
            RemovePrompt();
            promptTimer--;
        }
    }

    private string MakeGrenadeString() {
        string grenadeText = "";
        switch (playerResource.GetCurrentGrenadeType()) {
            case 0: // Black hole
                grenadeText = "BH";
                break;
            case 1:
                grenadeText = "EMP";
                break;
            case 2:
                grenadeText = "Frag";
                break;
        }
        grenadeText += " " + playerResource.GetCurrentGrenadeCount().ToString();
        return grenadeText;
    }

    string MakeAmmoString() {
        string ammoText = "";
        if (weaponInventory.currentWeapon != null) {
            ammoText = weaponInventory.currentWeapon.currentClip.ToString();
            if (weaponInventory.currentWeapon.remainingAmmo > 0) {
                ammoText += "/" + weaponInventory.currentWeapon.remainingAmmo.ToString();
            }
        }
        return ammoText;
    }

    static string lastPrompt = "";
    static int promptTimer = 0;
    public static void Prompt(string input) {
        if (input != lastPrompt) {
            promptText.SetText(input);
            lastPrompt = input;
        }
        promptUI.Show(true);
        promptTimer = 10;
    }
    public static bool IsPrompt() {
        return promptUI.IsVisible();
    }
    public static void RemovePrompt() {
        promptUI.Show(false);
    }

    /// <summary>
    /// Displays prompt until player removes it.
    /// Pauses time, so this cannot be used for multiplayer
    /// </summary>
    /// <param name="input"></param>
    public static void TutorialPrompt(string input) {
        // If not tutorial, this is not allowed. Throw exception!
        if (!GameManager.IsSceneTutorial()) throw new TutorialCodeRunningInMultiplayerException();

        tutorialPromptText.SetText(input);
        tutorialPromptUI.Show(true);
        Time.timeScale = 0;
        tutorialPromptActive = true;
    }
    public static void RemoveTutorialPrompt() {
        tutorialPromptUI.Show(false);
        Time.timeScale = 1;
        tutorialPromptActive = false;
    }
}
