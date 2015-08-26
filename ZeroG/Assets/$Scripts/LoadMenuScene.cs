using UnityEngine;
using System.Collections;

public class LoadMenuScene : MonoBehaviour {

    void Update() {
        if (Input.GetKeyDown(KeyCode.F11)) {
            Application.LoadLevel(0); // Load main menu
        }
    }
}
