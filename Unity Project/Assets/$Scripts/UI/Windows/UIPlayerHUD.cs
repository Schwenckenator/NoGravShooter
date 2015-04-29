using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIPlayerHUD : MonoBehaviour {

    static MovingBar health;
    static MovingBar fuel;
    static ChangeableText ammo;
    static ChangeableText grenade;

    static PlayerResources playerResource;


    void Start() {
        Init();
    }
    public static void Init() {
        Canvas canvas = UIManager.GetCanvas(Menu.PlayerHUD);

        List<IChangeable> changers = FindChangeables(canvas);
        
        foreach (IChangeable changer in changers) {
            if (changer.IsType("fuel")) fuel = changer as MovingBar;
            else if (changer.IsType("health")) health = changer as MovingBar;
            else if (changer.IsType("ammo")) ammo = changer as ChangeableText;
            else if (changer.IsType("grenade")) grenade = changer as ChangeableText;
        }

        health.SetMaxValue(PlayerResources.GetMaxHealth());
        fuel.SetMaxValue(PlayerResources.GetMaxFuel());
    }

    private static List<IChangeable> FindChangeables(Canvas canvas) {
        List<IChangeable> changers = new List<IChangeable>();
        MovingBar[] bars = canvas.GetComponentsInChildren<MovingBar>();
        changers.AddRange(bars);
        ChangeableText[] texts = canvas.GetComponentsInChildren<ChangeableText>();
        changers.AddRange(texts);

        return changers;
    }

    public static void SetPlayerResource(PlayerResources res) {
        playerResource = res;
    }

    void Update() {
        if(GameManager.instance.IsPlayerSpawned()){
            float temp = playerResource.GetHealth();
            health.SetValue(temp);
            fuel.SetValue(playerResource.GetFuel());

            ammo.SetText(MakeAmmoString());
            grenade.SetText(MakeGrenadeString());
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
        if (playerResource.IsHoldingWeapon()) {
            ammoText = playerResource.GetCurrentClip().ToString();
            if (playerResource.GetRemainingAmmo() > 0) {
                ammoText += "/" + playerResource.GetRemainingAmmo().ToString();
            }
        }
        return ammoText;
    }
}
