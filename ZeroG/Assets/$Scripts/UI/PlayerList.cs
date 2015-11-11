using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerList : MonoBehaviour {
    public static bool listDirty = false;
    public Text myText;

    void Update() {
        if (listDirty) {
            PopulateList();
        }
    }

    //IEnumerator PopulateList() {
    //    listDirty = false;
    //    yield return null;
    //    string text = "Players:\n";
    //    foreach(string name in NetworkInfoWrapper.singleton.playerNames) {
    //        text += name + "\n";
    //    }
    //    myText.text = text;
    //}

    void PopulateList() {
        listDirty = false;
        string text = "Players:\n";
        foreach (string name in NetworkInfoWrapper.singleton.playerNames) {
            text += name + "\n";
        }
        myText.text = text;
    }
}
