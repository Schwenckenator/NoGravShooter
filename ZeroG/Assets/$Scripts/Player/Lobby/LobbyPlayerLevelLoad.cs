using UnityEngine;
using System.Collections;

public class LobbyPlayerLevelLoad : MonoBehaviour {

    public LobbyPlayer myPlayer;

    void OnLevelWasLoaded() {
        Invoke("Spawn", 0.1f);
    }
    void Spawn() {
        myPlayer.SpawnListEntry();
    }
}
