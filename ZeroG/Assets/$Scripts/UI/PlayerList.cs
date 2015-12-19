using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerList : MonoBehaviour {
    public Text myText;
    static bool listDirty = false;

    static bool debug = false;

    public static void Dirty() {
        if (debug) {
            Debug.Log("Player List Marked Dirty");
        }
        listDirty = true;
    }

    void OnEnable() {
        Dirty();
    }

    void Update() {
        if (listDirty) {
            listDirty = false;
            UpdateList();
        }
    }

    void UpdateList() {
        if (debug) {
            Debug.Log("Player List Update");
        }
        myText.text = "Players: \n" + NetworkInfoWrapper.singleton.playerListString; // ERROR here on clients
    }
}
