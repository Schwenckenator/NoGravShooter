using UnityEngine;
using System.Collections;

public class UIDebugMenu : MonoBehaviour {

    static GameObject instance;
    static bool showWindow;

    void Awake() {
        instance = gameObject;
        Show(false);
    }
    public static void ToggleShow() {
        Show(!showWindow);
    }
    static void Show(bool show) {
        showWindow = show;
        instance.SetActive(show);
    }

    public void ToggleAdminMode(bool value) {
        DebugManager.adminMode = value;
    }
    public void ToggleDebugMode(bool value) {
        DebugManager.debugMode = value;
    }
    public void ToggleAllWeapon(bool value) {
        DebugManager.allWeapon = value;
    }
    public void ToggleAllAmmo(bool value) {
        DebugManager.allAmmo = value;
    }
    public void ToggleAllGrenade(bool value) {
        DebugManager.allGrenade = value;
    }
    public void ToggleAllFuel(bool value) {
        DebugManager.allFuel = value;
    }
    public void ClearPlayerPrefs() {
        PlayerPrefs.DeleteAll();
    }

}
