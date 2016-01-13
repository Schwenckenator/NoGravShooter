using UnityEngine;
using System.Collections;

public class OnGameOpen : MonoBehaviour {

    public GameObject parentPrefab;
    public GameObject[] spawns;

	void Awake(){
        Spawn();
		Destroy(gameObject);
	}
	
    void Spawn() {
        if (GameObject.FindGameObjectsWithTag("GameController").Length == 0) {
            GameObject managerParent = Instantiate(parentPrefab);
            foreach (GameObject spawn in spawns) {
                GameObject man = Instantiate(spawn);
                man.transform.SetParent(managerParent.transform);
            }
        }
    }
}
