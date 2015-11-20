using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerList : MonoBehaviour {
    public Text myText;
    static float waitTime = 0.2f;
    static bool listDirty = false;

    public static void Dirty() {
        listDirty = true;
    }

    void Update() {
        if (listDirty) {
            listDirty = false;
            Invoke("PopulateList", waitTime);
        }
    }

    void PopulateList() {
        string text = "Players:\n";
        SyncListPlayerInfo players = NetworkInfoWrapper.singleton.GetPlayers();
        for (int i=0; i< players.Count; i++) {
            text += "Player: "+ players[i].name + "\n"; // TODO remove "Player: "
        }
        myText.text = text;
    }
}
