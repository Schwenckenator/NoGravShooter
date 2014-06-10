using UnityEngine;
using System.Collections;

public class OnGameOpen : MonoBehaviour {

	public GameObject manager;

	void Awake(){
		if(GameObject.FindGameObjectsWithTag("GameController").Length == 0){
			Instantiate(manager);
		}
		Destroy(gameObject);
	}
	
}
