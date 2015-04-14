using UnityEngine;
using System.Collections;
     
public class TutorialPlatform : MonoBehaviour {

	private bool landed = false;
	public int platformID;
	private GameObject tuteManager;
	private GameObject platform;
	
	void Start(){
		tuteManager = GameObject.Find("TutorialChecker");
	}
	
	void OnTriggerEnter(Collider other) {
		if(other.tag == "Player") {
			if(landed == false){
				tuteManager.GetComponent<TutorialInstructions>().landedPlatforms ++;
				landed = true;
				platform = GameObject.Find("PlatformFront"+platformID);
				platform.transform.renderer.material.color = new Color(0, 0, 1);
			}
		}
	}
	
}