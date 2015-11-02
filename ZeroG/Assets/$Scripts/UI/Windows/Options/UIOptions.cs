using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIOptions : MonoBehaviour {

    public static UIOptions singleton { get; private set; }

    private GameObject backWindow;

    // Option
    private static InputField[] inputFields;
    private static Slider[] sliders;
    private static OptionColourBox[] playerColours;
    private static Toggle[] toggles;

    public void SetBackWindow(GameObject menu) {
        backWindow = menu;
    }
    public void CloseOptions() {
        UIManager.singleton.OpenReplace(backWindow);
    }
}
