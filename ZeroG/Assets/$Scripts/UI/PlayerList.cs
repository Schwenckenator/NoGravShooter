using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerList : MonoBehaviour {
    public static bool listDirty = false;
    public Text myText;

    void Update() {
        if (listDirty) {
            listDirty = false;
            Invoke("PopulateList", 0.25f);
        }
    }

    void PopulateList() {
        string text = "Players:\n";
        foreach (string name in NetworkInfoWrapper.singleton.playerNames) {
            text += name + "\n";
        }
        myText.text = text;
    }
}
