﻿using UnityEngine;
using UnityEngine.UI;
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
    static ChangeableImage tutorialPromptImage;

    static PlayerResources playerResource;
    static WeaponInventory weaponInventory;
    static IDamageable playerHealth;
    static IActorStats ActorStats;

    static IHideable playerUI;
    static IHideable sniperUI;
    static IHideable promptUI;
    static IHideable tutorialPromptUI;

    static bool tutorialPromptActive = false;

    public Sprite testSprite;

    void Start() {
        Init();

        gameObject.SendMessage("UIWindowInitialised", SendMessageOptions.RequireReceiver);
    }
    public static void Init() {
        Canvas canvas = UIManager.GetCanvas(Menu.PlayerHUD);
        //List<IChangeable> changers = UIManager.FindChangeables(canvas);
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
            else if (changer.IsType("tutorialPromptImage")) tutorialPromptImage = changer as ChangeableImage;
        }

        health.SetMaxValue(ActorHealth.GetDefaultMaxHealth());
        fuel.SetMaxValue(PlayerResources.GetMaxFuel());
        ShowSniperScope(false); // Set to default
        RemoveTutorialPrompt();
    }

    public static void ShowSniperScope(bool showSniper) {
        playerUI.Show(!showSniper);
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
    }
}
