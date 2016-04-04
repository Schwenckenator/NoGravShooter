using UnityEngine;
using System.Collections;

public class UISpawner : MonoBehaviour {

    public GameObject menuWindows;
    public GameObject gameMenu;

	// Use this for initialization
	void Start () {
        //Always start on menu
        Instantiate(menuWindows);
	}

    void OnLevelWasLoaded() {
        if (GameManager.IsSceneMenu()) {
            Instantiate(menuWindows);
        } else {
            Instantiate(gameMenu);
        }
    }
	
}
