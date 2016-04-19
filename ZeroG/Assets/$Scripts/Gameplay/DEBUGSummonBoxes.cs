using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class DEBUGSummonBoxes : MonoBehaviour {

    private bool[] allowedBonuses;
    GameObject currentPlayer = null;

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.F4)){
			if(currentPlayer == null) {
                if (!FindMyPlayer()) return;
            }

            SpawnBonuses.SpawnRandomBonus(currentPlayer.transform.position + currentPlayer.transform.forward * 3, Quaternion.identity);
		}
	}

    // True on found
    private bool FindMyPlayer() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players) {
            if (player.GetComponent<NetworkIdentity>().isLocalPlayer) {
                currentPlayer = player;
            }
        }

        return currentPlayer != null;
    }
}
