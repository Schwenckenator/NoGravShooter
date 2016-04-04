using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadMenuScene : MonoBehaviour {

    void Update() {
        if (Input.GetKeyDown(KeyCode.F11)) {
            SceneManager.LoadScene(0); // Load main menu
        }
    }
}
