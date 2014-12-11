using UnityEngine;
using System.Collections;

public class DEBUGSummonBoxes : MonoBehaviour {

	public GameObject box;
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.F2)){
			Network.Instantiate(box, Vector3.forward * 2, Quaternion.identity, 0);
		}
	}
}
