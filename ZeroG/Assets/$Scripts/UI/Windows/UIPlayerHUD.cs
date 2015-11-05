﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIPlayerHUD : MonoBehaviour {

    public static bool IsShown {
        get {
            return playerUI.gameObject.activeInHierarchy;
        }
    }

    public static UIPlayerHUD singleton { get; private set; }

    static ChangeableText ammo;
    static ChangeableText grenade;
    static ChangeableText promptText;
    static ChangeableText tutorialPromptText;
    
    static ChangeableImage crosshair;
    static ChangeableImage sniperScope;
    static ChangeableImage sniperBackground;
    static ChangeableImage tutorialPromptImage;
    static ChangeableImage fuelBarBack;

    static ActorJetpackFuel jetpackFuel;
    static ActorGrenades actorGrenades;
    static WeaponInventory weaponInventory;
    static IDamageable playerHealth;
    static IActorStats actorStats;

    static Canvas playerUI;
    static Canvas sniperUI;

    static IHideable promptUI;
    static IHideable tutorialPromptUI;

    static bool tutorialPromptActive = false;

    public Sprite testSprite;
    public MovingBar healthBar;
    public MovingBar fuelBar;

    void Awake() {
        singleton = this;
    }

    void Start() {
        Init();
        playerUI = GetComponent<Canvas>();

        tutorialPromptUI.Show(false);
        RemovePrompt();
    }

    public void Init() {
        IChangeable[] changers = gameObject.GetInterfacesInChildren<IChangeable>();

        foreach (IChangeable changer in changers) {
            if (changer.IsType("ammo")) ammo = changer as ChangeableText;
            else if (changer.IsType("grenade")) grenade = changer as ChangeableText;
            else if (changer.IsType("promptText")) promptText = changer as ChangeableText;
            else if (changer.IsType("promptHide")) promptUI = changer as HideableUI;
            else if (changer.IsType("tutorialPromptText")) tutorialPromptText = changer as ChangeableText;
            else if (changer.IsType("tutorialPromptUI")) tutorialPromptUI = changer as HideableUI;
            else if (changer.IsType("tutorialPromptImage")) tutorialPromptImage = changer as ChangeableImage;
            else if (changer.IsType("fuelBarBack")) fuelBarBack = changer as ChangeableImage;
            else if (changer.IsType("chat") || changer.IsType("playerList")) UIChat.ConnectChatBox(changer as ChangeableText);
        }
    }



    public static void ShowSniperScope(bool showSniper) {
        GameClock.ShowClock(!showSniper);
        // TODO
    }

    public void SetupPlayer(GameObject actor) {
        jetpackFuel = actor.GetComponent<ActorJetpackFuel>();
        actorGrenades = actor.GetComponent<ActorGrenades>();
        playerHealth = actor.GetInterface<IDamageable>();
        weaponInventory = actor.GetComponent<WeaponInventory>();

        healthBar.SetMaxValue(playerHealth.MaxHealth);
        fuelBar.SetMaxValue(ActorJetpackFuel.GetDefaultMaxFuel());
    }

    void Update() {
        if(PlayerManager.isMyActorSpawned) {
            float temp = playerHealth.Health;
            healthBar.SetValue(temp);
            fuelBar.SetValue(jetpackFuel.GetFuel());

            ammo.SetText(MakeAmmoString());
            grenade.SetText(MakeGrenadeString());
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            UIManager.singleton.OpenReplace(UIManager.singleton.initiallyOpen); // Initially open is menu
            GameManager.SetCursorVisibility(true);
        }

        // TEST
        if (Input.GetKeyDown(KeyCode.F5)) {
            TutorialPrompt("This is a test prompt.", testSprite);
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
        switch (actorGrenades.GetCurrentGrenadeType()) {
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
        grenadeText += " " + actorGrenades.GetCurrentGrenadeCount().ToString();
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
    public static void TutorialPrompt(string input, Sprite sprite = null) {
        // If not tutorial, this is not allowed. Throw exception!
        if (!GameManager.IsSceneTutorial()) throw new TutorialCodeRunningInMultiplayerException();

        tutorialPromptText.SetText(input);
        if (sprite != null) {
            tutorialPromptImage.SetVisible(true);
            tutorialPromptImage.SetImage(sprite);
        } else {
            tutorialPromptImage.SetVisible(false);
        }
        tutorialPromptUI.Show(true);

        Time.timeScale = 0;
        tutorialPromptActive = true;
    }
    public static void RemoveTutorialPrompt() {
        tutorialPromptUI.Show(false);
        Time.timeScale = 1;
        tutorialPromptActive = false;
        GameManager.SetCursorVisibility(false); // Make sure cursor is locked.
    }

    static Color red = new Color(1, 0, 0, 1);
    static Color black = new Color(0, 0, 0, 1);

    public static void JetpackDisabled(bool disabled) {
        Color backCol = disabled ? red : black;
        fuelBarBack.SetColour(backCol);
    }
}
