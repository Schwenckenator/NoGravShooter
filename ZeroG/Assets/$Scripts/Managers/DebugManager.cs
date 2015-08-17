using UnityEngine;
using System.Collections;

static class DebugManager {
    
    private static bool debugMode = false;
    private static bool adminMode = false;
    private static bool allWeapon = false;
    private static bool allAmmo = false;
    private static bool allGrenade = false;
    private static bool allFuel = false;
    private static bool paintballMode = false;

    public static bool IsAllWeapon() {
        return allWeapon;
    }
    public static bool IsAllAmmo() {
        return allAmmo;
    }
    public static bool IsAllGrenade() {
        return allGrenade;
    }
    public static bool IsAllFuel() {
        return allFuel;
    }
    public static bool IsAdminMode() {
        return adminMode;
    }
    public static bool IsDebugMode() {
        return debugMode;
    }
    public static bool IsPaintballMode() {
        return paintballMode;
    }

    public static void SetAllWeapon(bool value){
        allWeapon = value;
    }
    public static void SetAllAmmo(bool value) {
        allAmmo = value;
    }
    public static void SetAllGrenade(bool value) {
        allGrenade = value;
    }
    public static void SetAllFuel(bool value) {
        allFuel = value;
    }
    public static void SetAdminMode(bool value) {
        adminMode = value;
    }
    public static void SetDebugMode(bool value) {
        debugMode = value;
    }
    public static void SetPaintballMode(bool value) {
        paintballMode = value;
    }
    
    public static void ToggleTestMode() {
        adminMode = !adminMode;

        allWeapon = adminMode;
        allGrenade = adminMode;
        allAmmo = adminMode;
        allFuel = adminMode;
    }
    public static void ToggleDebugMode() {
        debugMode = !debugMode;
    }
    /// <summary>
    /// Draws line in colour with depth test, but also draws line in red without.
    /// Effect is dual coloured line, to see what is obscured
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="colour"></param>
    /// <param name="duration"></param>
    public static void DrawLine(Vector3 start, Vector3 end, Color colour, float duration) {
        Debug.DrawLine(start, end, Color.red, duration, false);
        Debug.DrawLine(start, end, colour, duration, true);
    }

 
}

