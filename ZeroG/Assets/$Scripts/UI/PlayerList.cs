using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerList : MonoBehaviour {
    public Text myText;
    static bool listDirty = false;

    public static void Dirty() {
        listDirty = true;
    }

    void Update() {
        if (listDirty) {
            listDirty = false;
            UpdateList();
        }
    }

    void UpdateList() {
        myText.text = "Players: \n" + NetworkInfoWrapper.singleton.playerListString;
    }
}
