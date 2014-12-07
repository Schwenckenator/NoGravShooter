using UnityEngine;
using System.Collections;
     
public class TutorialInstructions : MonoBehaviour {
	private GameManager manager;
	private PlayerResources playerRes;
	
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
	
	private bool items = false;
	private bool step4 = false;
	
	public GameObject[] bonuses;
	private GameObject[] bonusSpawnPoints;
	private GameObject[] bonusItems;
	

	void Start(){
		manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
		bonusSpawnPoints = GameObject.FindGameObjectsWithTag("BonusSpawnPoint");
		StartCoroutine(MovementTutorial());
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
		if(step4){
			bonusItems = GameObject.FindGameObjectsWithTag("BonusPickup");
			if(bonusItems.Length == 0){
				items = true;
			}
		}


		if (moved && step1){
			step1 = false;
			StartCoroutine(FlightTutorial());
		}
		if (jumped && rolled && step2){
			step2 = false;
			StartCoroutine(GunTutorial());
		}
		if (shot && aimed && changedgun && reloaded && grenade && step3){
			step3 = false;
			StartCoroutine(ItemTutorial());
		}
		if (items && step4){
			step4 = false;
			StartCoroutine(FinalTutorial());
		}
	}
	
	IEnumerator MovementTutorial(){
		//give player mines
		Network.Instantiate(bonuses[2], bonusSpawnPoints[2].transform.position, bonusSpawnPoints[2].transform.rotation, 0);
		manager.GetComponent<GUIScript>().TutorialPrompt("Welcome to the SC1830 Utility Suit.\n\nCalibrating.", 6000);
		yield return new WaitForSeconds(1);
		//damage player so they can pick up Medikit
		GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerResources>().TakeDamage(10);
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
	
	IEnumerator FlightTutorial(){
		manager.GetComponent<GUIScript>().TutorialPrompt("Use "+GameManager.keyBindings[(int)GameManager.KeyBind.JetUp].ToString()+" to boost upwards and "+GameManager.keyBindings[(int)GameManager.KeyBind.JetDown].ToString()+" to boost downwards.\n\nUse "+GameManager.keyBindings[(int)GameManager.KeyBind.RollLeft].ToString()+" to roll to the left and "+GameManager.keyBindings[(int)GameManager.KeyBind.RollRight].ToString()+" to roll to the right.\n\nYou can also rotate by moving the mouse while floating.", 99999);
		yield return new WaitForSeconds(5);
		step2 = true;		
	}
	
	IEnumerator GunTutorial(){
		manager.GetComponent<GUIScript>().TutorialPrompt("Click the left Mouse Button to shoot and use the right Mouse Button to aim.\n\nUse the Mouse Wheel or Numbers to change weapons.\n\nPress "+GameManager.keyBindings[(int)GameManager.KeyBind.Reload].ToString()+" to reload your weapon and press "+GameManager.keyBindings[(int)GameManager.KeyBind.Grenade].ToString()+" to throw a Proximity Mine.\nKeep in mind that without gravity, the mines will fly in a straight line.", 99999);
		yield return new WaitForSeconds(5);
		step3 = true;
	}
	
	IEnumerator ItemTutorial(){
		manager.GetComponent<GUIScript>().TutorialPrompt("Some items have been spawned on one of the platforms.\n\nThese are a Weapon pickup, a Medikit and a Proximity Mine box.\n\nThey can be picked up by touching them.\nYou can also shoot items to stop others from getting them.", 9999);
		Network.Instantiate(bonuses[0], bonusSpawnPoints[2].transform.position, bonusSpawnPoints[2].transform.rotation, 0);
		Network.Instantiate(bonuses[1], bonusSpawnPoints[1].transform.position, bonusSpawnPoints[1].transform.rotation, 0);
		Network.Instantiate(bonuses[2], bonusSpawnPoints[0].transform.position, bonusSpawnPoints[0].transform.rotation, 0);
		yield return new WaitForSeconds(5);
		step4 = true;
	}
	
	IEnumerator FinalTutorial(){
		manager.GetComponent<GUIScript>().TutorialPrompt("This suit comes equipped with Electro-Gravitational Boots.\n\nTo land on a surface you must rotate yourself so you hit the surface feet first.\n\nThis suit uses air as fuel, if you run low on air the suit will\nautomatically disable boosting temporarily while it generates more.", 5000);
		yield return new WaitForSeconds(40);
		manager.GetComponent<GUIScript>().TutorialPrompt("At the right of your HUD the suit displays important information including:\n\nmine count, ammo count, remaining air and suit structural integrity.\n\nIf the structural integrity of the suit is compromised you will lose\nboth pressurization and air supply, resulting in death.", 6000);
		yield return new WaitForSeconds(50);
		manager.GetComponent<GUIScript>().TutorialPrompt("Try exploring the virtual space.", 99999);
	}
}