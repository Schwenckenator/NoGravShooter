using UnityEngine;
using System.Collections;

public class DEBUGSummonBoxes : MonoBehaviour {

    private bool[] allowedBonuses;

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.F4)){
			GameObject currentPlayer = null;
			GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
			foreach(GameObject player in players){
				if(player.networkView.isMine){
					currentPlayer = player;
				}
			}
			if(currentPlayer == null) return;

            SpawnBonuses.SpawnRandomBonus(currentPlayer.transform.position + currentPlayer.transform.forward * 3, Quaternion.identity);
		}
	}

}
