using UnityEngine;
using System.Collections;

public class InputConverter {

    public static bool GetKeyDown(KeyBind input) {
        return Input.GetKeyDown(SettingsManager.keyBindings[(int)input]);
    }

    public static bool GetKey(KeyBind input) {
        return Input.GetKey(SettingsManager.keyBindings[(int)input]);
    }

    public static string GetKeyName(KeyBind input) {
        return SettingsManager.keyBindings[(int)input].ToString();
    }
}
