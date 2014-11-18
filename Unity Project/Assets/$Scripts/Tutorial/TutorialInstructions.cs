using UnityEngine;
using System.Collections;
     
public class TutorialInstructions : MonoBehaviour {
	private GameManager manager;

	void Start(){
		manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
		StartCoroutine(RunTutorial());
	}
	
	IEnumerator RunTutorial(){
		manager.GetComponent<GUIScript>().TutorialPrompt("Welcome to the SC1830 Utility Suit.\n\nCalibrating.", 6000);
		yield return new WaitForSeconds(1);
		manager.GetComponent<GUIScript>().TutorialPrompt("Welcome to the SC1830 Utility Suit.\n\nCalibrating..", 6000);
		yield return new WaitForSeconds(1);
		manager.GetComponent<GUIScript>().TutorialPrompt("Welcome to the SC1830 Utility Suit.\n\nCalibrating...", 6000);
		yield return new WaitForSeconds(1);
		manager.GetComponent<GUIScript>().TutorialPrompt("Welcome to the SC1830 Utility Suit.\n\nCalibrating....", 6000);
		yield return new WaitForSeconds(1);
		manager.GetComponent<GUIScript>().TutorialPrompt("Welcome to the SC1830 Utility Suit.\n\nSuit Calibrated.", 6000);
		yield return new WaitForSeconds(2);
		manager.GetComponent<GUIScript>().TutorialPrompt("Welcome to the SC1830 Utility Suit.\n\nSuit Calibrated.\n\nRunning Tutorial Simulation.", 6000);
		yield return new WaitForSeconds(4);
		manager.GetComponent<GUIScript>().TutorialPrompt("Move your Mouse to look around.\n\nUse "+GameManager.keyBindings[(int)GameManager.KeyBind.MoveForward].ToString()+", "+GameManager.keyBindings[(int)GameManager.KeyBind.MoveLeft].ToString()+", "+GameManager.keyBindings[(int)GameManager.KeyBind.MoveBack].ToString()+" and "+GameManager.keyBindings[(int)GameManager.KeyBind.MoveRight].ToString()+" to move around.", 6000);
		yield return new WaitForSeconds(15);
		manager.GetComponent<GUIScript>().TutorialPrompt("Use "+GameManager.keyBindings[(int)GameManager.KeyBind.JetUp].ToString()+" to boost upwards and "+GameManager.keyBindings[(int)GameManager.KeyBind.JetDown].ToString()+" to boost downwards.\n\nUse "+GameManager.keyBindings[(int)GameManager.KeyBind.RollLeft].ToString()+" to roll to the left and "+GameManager.keyBindings[(int)GameManager.KeyBind.RollRight].ToString()+" to roll to the right.\n\nYou can also rotate by moving the mouse while floating.", 6000);
		yield return new WaitForSeconds(25);
		manager.GetComponent<GUIScript>().TutorialPrompt("Click the left Mouse Button to shoot and use the right Mouse Button to aim.\n\nUse the Mouse Wheel or Numbers to change weapons.\n\nPress "+GameManager.keyBindings[(int)GameManager.KeyBind.Reload].ToString()+" to reload your weapon and press "+GameManager.keyBindings[(int)GameManager.KeyBind.Grenade].ToString()+" to throw a Proximity Mine.\nKeep in mind that without gravity, the mines will fly in a straight line.", 6000);
		yield return new WaitForSeconds(35);
		manager.GetComponent<GUIScript>().TutorialPrompt("This suit comes equipped with Electro-Gravitational Boots.\n\nTo land on a surface you must rotate yourself so you hit the surface feet first.\n\nThis suit uses air as fuel, if you run low on air the suit will\nautomatically disable boosting temporarily while it generates more.", 5000);
		yield return new WaitForSeconds(35);
		manager.GetComponent<GUIScript>().TutorialPrompt("At the right of your HUD the suit displays important information including:\n\nmine count, ammo count, remaining air and suit structural integrity.\n\nIf the structural integrity of the suit is compromised you will lose\nboth pressurization and air supply, killing you.", 6000);
		yield return new WaitForSeconds(35);
		manager.GetComponent<GUIScript>().TutorialPrompt("Try exploring the virtual space.", 99999);
	}
}