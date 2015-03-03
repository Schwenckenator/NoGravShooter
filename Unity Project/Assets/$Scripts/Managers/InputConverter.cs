using UnityEngine;
using System.Collections;

public class InputConverter : MonoBehaviour {

    public static bool GetKeyDown(KeyBind input) {
        return Input.GetKeyDown(SettingsManager.keyBindings[(int)input]);
    }

    public static bool GetKey(KeyBind input) {
        return Input.GetKey(SettingsManager.keyBindings[(int)input]);
    }

}
