using UnityEngine;
using System.Collections;

static class DebugManager {
    
    private static bool debugMode = false;
    private static bool adminMode = false;
    private static bool allWeapon = false;
    private static bool allGrenade = false;

    public static bool IsAllWeapon() {
        return allWeapon;
    }
    public static bool IsAllGrenade() {
        return allGrenade;
    }
    public static bool IsAdminMode() {
        return adminMode;
    }
    public static bool IsDebugMode() {
        return debugMode;
    }
    
    public static void ToggleTestMode() {
        allWeapon = !allWeapon;
        allGrenade = !allGrenade;
        adminMode = !adminMode;
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

