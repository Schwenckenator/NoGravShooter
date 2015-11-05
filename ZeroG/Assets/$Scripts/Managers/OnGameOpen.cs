using UnityEngine;
using System.Collections;

public class OnGameOpen : MonoBehaviour {

    public GameObject[] spawns;

	void Awake(){
        Spawn();
		Destroy(gameObject);
	}
	
    void Spawn() {
        if (GameObject.FindGameObjectsWithTag("GameController").Length == 0) {
            foreach (GameObject spawn in spawns) {
                Instantiate(spawn);
            }
        }
    }
}
