using UnityEngine;
using System.Collections;

public class UIPlayerSettings : MonoBehaviour {
    static GameObject instance;
	// Use this for initialization
	void Awake () {
        instance = gameObject;
        instance.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
