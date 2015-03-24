using UnityEngine;
using System.Collections;
     
public class TutorialPlatform : MonoBehaviour {

	private bool landed = false;
	private GameObject tuteManager;
	
	void Start(){
		tuteManager = GameObject.Find("TutorialChecker");
	}
	
	void OnTriggerEnter(Collider other) {
		if(other.tag == "Player") {
			if(landed == false){
				tuteManager.GetComponent<TutorialInstructions>().landedPlatforms ++;
				landed = true;
			}
		}
	}
	
}