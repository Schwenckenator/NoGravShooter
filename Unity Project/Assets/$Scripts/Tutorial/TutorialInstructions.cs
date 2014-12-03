using UnityEngine;
using System.Collections;
     
public class TutorialInstructions : MonoBehaviour {
	private GameManager manager;
	
	private bool moved = false;
	private bool step1 = false;
	
	private bool jumped = false;
	private bool rolled = false;
	private bool step2 = false;
	
	private bool shot = false;
	private bool aimed = false;
	private bool changedgun = false;
	private bool reloaded = false;
	private bool grenade = false;
	private bool step3 = false;

	void Start(){
		manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
		StartCoroutine(StartTutorial());
	}
	
	void Update(){
		if (Input.GetKeyDown(GameManager.keyBindings[(int)GameManager.KeyBind.MoveForward]) || Input.GetKeyDown(GameManager.keyBindings[(int)GameManager.KeyBind.MoveRight])){
			moved = true;
			Debug.Log("Player Moved");
		}
		if (Input.GetKeyDown(GameManager.keyBindings[(int)GameManager.KeyBind.JetUp])){
			jumped = true;
			Debug.Log("Player Jumped");
		}
		if (Input.GetKeyDown(GameManager.keyBindings[(int)GameManager.KeyBind.RollRight]) || Input.GetKeyDown(GameManager.keyBindings[(int)GameManager.KeyBind.RollLeft])){
			rolled = true;
			Debug.Log("Player Rolled");
		}
		if (Input.GetMouseButtonDown(0)){
			shot = true;
			Debug.Log("Player Shot");
		}
		if (Input.GetMouseButtonDown(1)){
			aimed = true;
			Debug.Log("Player Aimed");
		}
		if (Input.GetKeyDown("2") || Input.GetKeyDown("3") || Input.GetKeyDown("4") || Input.GetKeyDown("5") || Input.GetKeyDown("6") || Input.GetKeyDown("7") || Input.GetAxis("Mouse ScrollWheel") < 0 || Input.GetAxis("Mouse ScrollWheel") > 0){
			changedgun = true;
			Debug.Log("Player Changed Weapons");
		}		
		if (Input.GetKeyDown(GameManager.keyBindings[(int)GameManager.KeyBind.Reload])){
			reloaded = true;
			Debug.Log("Player Reloaded");
		}
		if (Input.GetKeyDown(GameManager.keyBindings[(int)GameManager.KeyBind.Grenade])){
			grenade = true;
			Debug.Log("Player Threw Mine");
		}
		
		if (moved && step1){
			StartCoroutine(MovementTutorial());
		}
		if (jumped && rolled && step2){
			StartCoroutine(FlightTutorial());
		}
		if (shot && aimed && changedgun && reloaded && grenade && step3){
			StartCoroutine(GunTutorial());
		}
	}
	
	IEnumerator StartTutorial(){
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
		manager.GetComponent<GUIScript>().TutorialPrompt("Move your Mouse to look around.\n\nUse "+GameManager.keyBindings[(int)GameManager.KeyBind.MoveForward].ToString()+", "+GameManager.keyBindings[(int)GameManager.KeyBind.MoveLeft].ToString()+", "+GameManager.keyBindings[(int)GameManager.KeyBind.MoveBack].ToString()+" and "+GameManager.keyBindings[(int)GameManager.KeyBind.MoveRight].ToString()+" to move around.", 99999);
		yield return new WaitForSeconds(5);
		step1 = true;
	}
	
	IEnumerator MovementTutorial(){
		manager.GetComponent<GUIScript>().TutorialPrompt("Use "+GameManager.keyBindings[(int)GameManager.KeyBind.JetUp].ToString()+" to boost upwards and "+GameManager.keyBindings[(int)GameManager.KeyBind.JetDown].ToString()+" to boost downwards.\n\nUse "+GameManager.keyBindings[(int)GameManager.KeyBind.RollLeft].ToString()+" to roll to the left and "+GameManager.keyBindings[(int)GameManager.KeyBind.RollRight].ToString()+" to roll to the right.\n\nYou can also rotate by moving the mouse while floating.", 99999);
		yield return new WaitForSeconds(5);
		step2 = true;		
	}
	
	IEnumerator FlightTutorial(){
		manager.GetComponent<GUIScript>().TutorialPrompt("Click the left Mouse Button to shoot and use the right Mouse Button to aim.\n\nUse the Mouse Wheel or Numbers to change weapons.\n\nPress "+GameManager.keyBindings[(int)GameManager.KeyBind.Reload].ToString()+" to reload your weapon and press "+GameManager.keyBindings[(int)GameManager.KeyBind.Grenade].ToString()+" to throw a Proximity Mine.\nKeep in mind that without gravity, the mines will fly in a straight line.", 99999);
		yield return new WaitForSeconds(5);
		step3 = true;
	}
	
	IEnumerator GunTutorial(){
		manager.GetComponent<GUIScript>().TutorialPrompt("This suit comes equipped with Electro-Gravitational Boots.\n\nTo land on a surface you must rotate yourself so you hit the surface feet first.\n\nThis suit uses air as fuel, if you run low on air the suit will\nautomatically disable boosting temporarily while it generates more.", 5000);
		yield return new WaitForSeconds(40);
		manager.GetComponent<GUIScript>().TutorialPrompt("At the right of your HUD the suit displays important information including:\n\nmine count, ammo count, remaining air and suit structural integrity.\n\nIf the structural integrity of the suit is compromised you will lose\nboth pressurization and air supply, resulting in death.", 6000);
		yield return new WaitForSeconds(50);
		manager.GetComponent<GUIScript>().TutorialPrompt("Try exploring the virtual space.", 99999);
	}
}