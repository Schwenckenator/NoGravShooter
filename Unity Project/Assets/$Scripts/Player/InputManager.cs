using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

    public static bool GetKeyDown(SettingsManager.KeyBind input) {
        return Input.GetKeyDown(SettingsManager.keyBindings[(int)input]);
    }

}
