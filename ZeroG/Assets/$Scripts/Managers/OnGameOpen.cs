using UnityEngine;
using System.Collections;

public class OnGameOpen : MonoBehaviour {

    public GameObject[] spawns;

	void Awake(){
		if(GameObject.FindGameObjectsWithTag("GameController").Length == 0){
			foreach(GameObject spawn in spawns) {
                Instantiate(spawn);
            }
		}
		Destroy(gameObject);
	}
	
}
