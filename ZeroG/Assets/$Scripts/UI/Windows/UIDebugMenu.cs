using UnityEngine;
using System.Collections;

public class UIDebugMenu : MonoBehaviour {

    static GameObject instance;
    static bool showWindow;

    void Awake() {
        instance = gameObject;
        UIDebugMenu.Show(false);
    }
    public static void ToggleShow() {
        UIDebugMenu.Show(!showWindow);
    }
    static void Show(bool show) {
        showWindow = show;
        instance.SetActive(show);
    }

    public void ToggleAdminMode(bool value) {
        DebugManager.SetAdminMode(value);
    }
    public void ToggleDebugMode(bool value) {
        DebugManager.SetDebugMode(value);
    }
    public void ToggleAllWeapon(bool value) {
        DebugManager.SetAllWeapon(value);
    }
    public void ToggleAllAmmo(bool value) {
        DebugManager.SetAllAmmo(value);
    }
    public void ToggleAllGrenade(bool value) {
        DebugManager.SetAllGrenade(value);
    }
    public void ToggleAllFuel(bool value) {
        DebugManager.SetAllFuel(value);
    }


}
