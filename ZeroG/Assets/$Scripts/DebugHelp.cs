﻿using UnityEngine;
using System.Collections;

public class DebugHelp : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.F11)) {
            UIGraphicsSettings.instance.SaveGraphicsSettings();
        }
	}
}
