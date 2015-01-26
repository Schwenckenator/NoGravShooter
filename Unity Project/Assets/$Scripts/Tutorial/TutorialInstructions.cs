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
		if (Input.GetKeyDown(SettingsManager.keyBindings[(int)SettingsManager.KeyBind.MoveForward]) || Input.GetKeyDown(SettingsManager.keyBindings[(int)SettingsManager.KeyBind.MoveRight])){
			moved = true;
			Debug.Log("Player Moved");
		}
		if (Input.GetKeyDown(SettingsManager.keyBindings[(int)SettingsManager.KeyBind.JetUp])){
			jumped = true;
			Debug.Log("Player Jumped");
		}
		if (Input.GetKeyDown(SettingsManager.keyBindings[(int)SettingsManager.KeyBind.RollRight]) || Input.GetKeyDown(SettingsManager.keyBindings[(int)SettingsManager.KeyBind.RollLeft])){
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
		if (Input.GetKeyDown(SettingsManager.keyBindings[(int)SettingsManager.KeyBind.Reload])){
			reloaded = true;
			Debug.Log("Player Reloaded");
		}
		if (Input.GetKeyDown(SettingsManager.keyBindings[(int)SettingsManager.KeyBind.Grenade])){
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
		manager.GetComponent<GuiManager>().TutorialPrompt("Welcome to the SC1830 Utility Suit.\n\nCalibrating.", 6000);
		yield return new WaitForSeconds(1);
		//damage player so they can pick up Medikit
		GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerResources>().TakeDamage(10, Network.player);
		manager.GetComponent<GuiManager>().TutorialPrompt("Welcome to the SC1830 Utility Suit.\n\nCalibrating..", 6000);
		yield return new WaitForSeconds(1);
		manager.GetComponent<GuiManager>().TutorialPrompt("Welcome to the SC1830 Utility Suit.\n\nCalibrating...", 6000);
		yield return new WaitForSeconds(1);
		manager.GetComponent<GuiManager>().TutorialPrompt("Welcome to the SC1830 Utility Suit.\n\nCalibrating....", 6000);
		yield return new WaitForSeconds(1);
		manager.GetComponent<GuiManager>().TutorialPrompt("Welcome to the SC1830 Utility Suit.\n\nSuit Calibrated.", 6000);
		yield return new WaitForSeconds(2);
		manager.GetComponent<GuiManager>().TutorialPrompt("Welcome to the SC1830 Utility Suit.\n\nSuit Calibrated.\n\nRunning Tutorial Simulation.", 6000);
		yield return new WaitForSeconds(4);
		manager.GetComponent<GuiManager>().TutorialPrompt("Move your Mouse to look around.\n\nUse "+SettingsManager.keyBindings[(int)SettingsManager.KeyBind.MoveForward].ToString()+", "+SettingsManager.keyBindings[(int)SettingsManager.KeyBind.MoveLeft].ToString()+", "+SettingsManager.keyBindings[(int)SettingsManager.KeyBind.MoveBack].ToString()+" and "+SettingsManager.keyBindings[(int)SettingsManager.KeyBind.MoveRight].ToString()+" to move around.", 99999);
		yield return new WaitForSeconds(5);
		step1 = true;
	}
	
	IEnumerator FlightTutorial(){
		manager.GetComponent<GuiManager>().TutorialPrompt("Use "+SettingsManager.keyBindings[(int)SettingsManager.KeyBind.JetUp].ToString()+" to boost upwards and "+SettingsManager.keyBindings[(int)SettingsManager.KeyBind.JetDown].ToString()+" to boost downwards.\n\nUse "+SettingsManager.keyBindings[(int)SettingsManager.KeyBind.RollLeft].ToString()+" to roll to the left and "+SettingsManager.keyBindings[(int)SettingsManager.KeyBind.RollRight].ToString()+" to roll to the right.\n\nYou can also rotate by moving the mouse while floating.", 99999);
		yield return new WaitForSeconds(5);
		step2 = true;		
	}
	
	IEnumerator GunTutorial(){
		manager.GetComponent<GuiManager>().TutorialPrompt("Click the left Mouse Button to shoot and use the right Mouse Button to aim.\n\nUse the Mouse Wheel or Numbers to change weapons.\n\nPress "+SettingsManager.keyBindings[(int)SettingsManager.KeyBind.Reload].ToString()+" to reload your weapon and press "+SettingsManager.keyBindings[(int)SettingsManager.KeyBind.Grenade].ToString()+" to throw a Proximity Mine.\nKeep in mind that without gravity, the mines will fly in a straight line.", 99999);
		yield return new WaitForSeconds(5);
		step3 = true;
	}
	
	IEnumerator ItemTutorial(){
		manager.GetComponent<GuiManager>().TutorialPrompt("Some items have been spawned on one of the platforms.\n\nThese are a Weapon pickup, a Medikit and a Proximity Mine box.\n\nThey can be picked up by touching them.\nYou can also shoot items to stop others from getting them.", 9999);
		Network.Instantiate(bonuses[0], bonusSpawnPoints[2].transform.position, bonusSpawnPoints[2].transform.rotation, 0);
		Network.Instantiate(bonuses[1], bonusSpawnPoints[1].transform.position, bonusSpawnPoints[1].transform.rotation, 0);
		Network.Instantiate(bonuses[2], bonusSpawnPoints[0].transform.position, bonusSpawnPoints[0].transform.rotation, 0);
		yield return new WaitForSeconds(5);
		step4 = true;
	}
	
	IEnumerator FinalTutorial(){
		manager.GetComponent<GuiManager>().TutorialPrompt("This suit comes equipped with Electro-Gravitational Boots.\n\nTo land on a surface you must rotate yourself so you hit the surface feet first.\n\nThis suit uses air as fuel, if you run low on air the suit will\nautomatically disable boosting temporarily while it generates more.", 5000);
		yield return new WaitForSeconds(40);
		manager.GetComponent<GuiManager>().TutorialPrompt("At the right of your HUD the suit displays important information including:\n\nmine count, ammo count, remaining air and suit structural integrity.\n\nIf the structural integrity of the suit is compromised you will lose\nboth pressurization and air supply, resulting in death.", 6000);
		yield return new WaitForSeconds(50);
		manager.GetComponent<GuiManager>().TutorialPrompt("Try exploring the virtual space.", 99999);
	}
}