using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerList : MonoBehaviour {
    public Text myText;
    static bool listDirty = false;

    public static void Dirty() {
        Debug.Log("Player List Marked Dirty");
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
        Debug.Log("Player List Update");
        myText.text = "Players: \n" + NetworkInfoWrapper.singleton.playerListString;
    }
}
