using UnityEngine;
using System.Collections;

static class DebugManager {
    private static bool debugMode = false;

    public static bool IsDebugMode() {
        return debugMode;
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

