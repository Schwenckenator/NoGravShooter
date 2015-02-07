using UnityEngine;
using System.Collections;

public class DEBUGSummonBoxes : MonoBehaviour {
    [SerializeField]
	private GameObject box;

    //private NetworkManager networkManager;

    //void Start() {
    //    networkManager = GetComponent<NetworkManager>();
    //}

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.F2)){
			GameObject currentPlayer = null;
			GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
			foreach(GameObject player in players){
				if(player.networkView.isMine){
					currentPlayer = player;
				}
			}
			if(currentPlayer == null) return;

            Network.Instantiate(box, currentPlayer.transform.position + currentPlayer.transform.forward * 3, Quaternion.identity, 0);
		}
	}

}
