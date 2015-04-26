using UnityEngine;
using System.Collections;

public class UIPlayerHUD : MonoBehaviour {

    static MovingBar health;
    static MovingBar fuel;

    static PlayerResources playerResource;


    void Start() {
        Init();
    }
    public static void Init() {
        Canvas canvas = UIManager.GetCanvas(Menu.PlayerHUD);

        MovingBar[] bars = canvas.GetComponentsInChildren<MovingBar>();
        foreach (MovingBar bar in bars) {
            if (bar.GetBarType() == "fuel") fuel = bar;
            if (bar.GetBarType() == "health") health = bar;
        }

        health.SetMaxValue(PlayerResources.GetMaxHealth());
        fuel.SetMaxValue(PlayerResources.GetMaxFuel());
    }

    public static void SetPlayerResource(PlayerResources res) {
        playerResource = res;
    }

    void Update() {
        if(GameManager.instance.IsPlayerSpawned()){
            float temp = playerResource.GetHealth();
            health.SetValue(temp);

            fuel.SetValue(playerResource.GetFuel());
        }
    }
}
