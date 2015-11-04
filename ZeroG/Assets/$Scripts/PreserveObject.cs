using UnityEngine;
using System.Collections;

public class PreserveObject : MonoBehaviour {

    public bool dontDestroyOnLoad = true;

    void Start() {
        if (dontDestroyOnLoad) {
            DontDestroyOnLoad(gameObject);
        }
    }
}
