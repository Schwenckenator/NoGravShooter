using UnityEngine;
using System.Collections;

public class OnGameOpen : MonoBehaviour {

	public GameObject manager;
	public GameObject weaponManager;

	void Awake(){
		if(GameObject.FindGameObjectsWithTag("GameController").Length == 0){
			Instantiate(manager);
			Instantiate(weaponManager);
		}
		Destroy(gameObject);
	}
	
}
