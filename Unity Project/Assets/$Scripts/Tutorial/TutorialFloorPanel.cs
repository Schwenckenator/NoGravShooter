using UnityEngine;
using System.Collections;
     
public class TutorialFloorPanel : MonoBehaviour {

	private bool landed = false;
	private GameObject tuteManager;
	public int tileID;
	
	void Start(){
		tuteManager = GameObject.Find("TutorialChecker");
	}
	
	void OnTriggerEnter(Collider other) {
		if(other.tag == "Player") {
			if(landed == false && tuteManager.GetComponent<TutorialInstructions>().checkingfloortiles){
				if(tileID == 1){
					tuteManager.GetComponent<TutorialInstructions>().Floor1 = true;
					landed = true;
				} else if(tileID == 2){
					if(tuteManager.GetComponent<TutorialInstructions>().Floor1){
						tuteManager.GetComponent<TutorialInstructions>().Floor2 = true;
						landed = true;
					}
				} else {
					if(tuteManager.GetComponent<TutorialInstructions>().Floor2){
						tuteManager.GetComponent<TutorialInstructions>().Floor3 = true;
						landed = true;
					}
				}
			}
		}
	}
	
}