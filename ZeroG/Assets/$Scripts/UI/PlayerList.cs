using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerList : MonoBehaviour {
    public static bool listDirty = false;
    public Text myText;

    void Update() {
        if (listDirty) {
            StartCoroutine(PopulateList());
        }
    }
    


    IEnumerator PopulateList() {
        listDirty = false;
        yield return null;
        string text = "Players:\n";
        foreach(Player player in NetworkManager.connectedPlayers) {
            text += player.Name + "\n";
        }
        myText.text = text;
    }
}
