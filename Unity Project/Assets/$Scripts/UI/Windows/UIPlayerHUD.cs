using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIPlayerHUD : MonoBehaviour {

    static MovingBar health;
    static MovingBar fuel;
    static ChangeableText ammo;
    static ChangeableText grenade;
    static ChangeableText prompt;
    
    static ChangeableImage crosshair;
    static ChangeableImage sniperScope;
    static ChangeableImage sniperBackground;

    static PlayerResources playerResource;
    static WeaponInventory weaponInventory;
    static IDamageable playerHealth;
    static IActorStats ActorStats;

    static CanvasGroup playerUI;
    static CanvasGroup sniperUI;
    static CanvasGroup promptUI;


    void Start() {
        Init();
    }
    public static void Init() {
        Canvas canvas = UIManager.GetCanvas(Menu.PlayerHUD);
        List<IChangeable> changers = UIManager.FindChangeables(canvas);
        
        foreach (IChangeable changer in changers) {
            if (changer.IsType("fuel")) fuel = changer as MovingBar;
            else if (changer.IsType("health")) health = changer as MovingBar;
            else if (changer.IsType("ammo")) ammo = changer as ChangeableText;
            else if (changer.IsType("grenade")) grenade = changer as ChangeableText;
            else if (changer.IsType("prompt")) prompt = changer as ChangeableText;
        }

        CanvasGroup[] canvasGroups = canvas.GetComponentsInChildren<CanvasGroup>();
        playerUI = canvasGroups[0];
        promptUI = canvasGroups[1];
        sniperUI = canvasGroups[2];

        health.SetMaxValue(ActorHealth.GetDefaultMaxHealth());
        fuel.SetMaxValue(PlayerResources.GetMaxFuel());
        SetCrosshairImage(false); // Set to default
    }

    public static void SetCrosshairImage(bool showSniper) {
        playerUI.alpha = showSniper ? 0 : 1;
        sniperUI.alpha = showSniper ? 1 : 0;
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
            prompt.SetText(input);
            lastPrompt = input;
        }
        promptUI.alpha = 1;
        promptTimer = 10;
    }
    public static bool IsPrompt() {
        return promptUI.alpha == 1;
    }
    public static void RemovePrompt() {
        promptUI.alpha = 0;
    }
}
