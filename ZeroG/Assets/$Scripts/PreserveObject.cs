using UnityEngine;
using System.Collections;

public class PreserveObject : MonoBehaviour {

    public bool dontDestroyOnLoad = true;

    void Awake() {
        if (dontDestroyOnLoad) {
            DontDestroyOnLoad(gameObject);
        }
    }
}
