using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIKeybindSettings : MonoBehaviour {

    public GameObject changeKeybind;

    private static List<Text> keybindButtonText;
    private static int editedBinding = 0;

    

	// Use this for initialization
	void Awake () {
        keybindButtonText = new List<Text>();
        EditKeybindInit();

        // Turn self off after initialsation
        gameObject.SetActive(false);
	}

    void OnGUI() { // Dirty old system, but I can't see a way around it
        if (OldUIManager.IsChangeKeybindWindow()) {
            ChangeKeybindUpdate();
        }
    }

    void EditKeybindInit() {
        Canvas editKeybind = GetComponent<Canvas>();
        Button[] buttons = editKeybind.gameObject.GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons) {
            Text butText = button.GetComponentInChildren<Text>();
            if (butText.text == "Back") continue;
            keybindButtonText.Add(butText);
        }
        EditKeybindTextRefresh();
    }

    void EditKeybindTextRefresh() {
        for (int i = 0; i < keybindButtonText.Count; i++) {
            keybindButtonText[i].text = SettingsManager.keyBindings[i].ToString();
        }
    }

    void ChangeKeybindUpdate() {
        bool done = false;
        if (Event.current.isKey) {
            if (Event.current.keyCode != KeyCode.Escape) {
                SettingsManager.keyBindings[editedBinding] = Event.current.keyCode;
            }
            done = true;
        } else if (Event.current.shift) {
            SettingsManager.keyBindings[editedBinding] = KeyCode.LeftShift;
            done = true;
        }

        if (done) {
            EditKeybindTextRefresh();
            CloseChangeKeybind();
        }
    }

    public void OpenChangeKeybind(int key) {
        editedBinding = key;
        UIManager.singleton.Open(changeKeybind);
    }

    public void CloseChangeKeybind() {
        UIManager.singleton.Close(changeKeybind);
    }
}
