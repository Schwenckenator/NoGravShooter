//using UnityEngine;
//using System.Collections;
     
//public class TutorialFloorPanel : MonoBehaviour {

//	private bool landed = false;
//	private TutorialInstructions tuteManager;
//	public int tileID;
	
//	void Start(){
//        tuteManager = GameObject.Find("TutorialChecker").GetComponent<TutorialInstructions>();
//	}
	
//	void OnTriggerEnter(Collider other) {
//		if(other.tag == "Player") {
//			if(landed == false && tuteManager.checkingfloortiles){
//				if(tileID == 1){
//					tuteManager.Floor1 = true;
//					landed = true;
//				} else if(tileID == 2){
//					if(tuteManager.Floor1){
//						tuteManager.Floor2 = true;
//						landed = true;
//					}
//				} else {
//					if(tuteManager.Floor2){
//						tuteManager.Floor3 = true;
//						landed = true;
//					}
//				}
//			}
//		}
//	}
	
//}