using UnityEngine;
using System.Collections;

/// <summary>
/// This window is controlled by a different script
/// This will just disable the object on start
/// </summary>
public class UIHideWindow : MonoBehaviour {

	// Use this for initialization
	void Start () {
        // Turn self off after initialsation
        gameObject.SetActive(false);
	}
	
}
