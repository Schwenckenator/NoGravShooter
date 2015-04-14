using UnityEngine;
using System.Collections;

public class OnGameOpen : MonoBehaviour {

	public GameObject gameManager;
	public GameObject weaponManager;
    public GameObject uiManager;

	void Awake(){
		if(GameObject.FindGameObjectsWithTag("GameController").Length == 0){
			Instantiate(gameManager);
			Instantiate(weaponManager);
            Instantiate(uiManager);
		}
		Destroy(gameObject);
	}
	
}
