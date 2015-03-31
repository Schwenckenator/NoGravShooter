using UnityEngine;
using System.Collections;
     
public class TutorialInstructions : MonoBehaviour {
	private GameManager manager;
	private PlayerResources playerRes;
	private GameObject player;
	
	private bool F = false;
	private bool B = false;
	private bool L = false;
	private bool R = false;
	private bool step1 = false;
	
	public bool checkingfloortiles = false;
	public bool Floor1 = false;
	public bool Floor2 = false;
	public bool Floor3 = false;
	private bool openroom2 = false;
	private bool step2 = false;
	
	private bool shot = false;
	private bool aimed = false;
	private bool changedgun = false;
	private bool reloaded = false;
	private bool step3 = false;
	
	private bool checkingtargets = false;
	private int remainingtargets;
	private bool shotAllTargets = false;
	private bool step4 = false;
	
	private bool U = false;
	private bool D = false;
	private bool X = false;
	private bool step5 = false;
	
	private bool RL = false;
	private bool RR = false;
	private bool step6 = false;
	
	public int landedPlatforms = 0;
	private bool platforms = false;
	private bool step7 = false;
	
	private bool grenade = false;
	private bool step8 = false;
	
	private bool checkingitems = false;
	private bool items = false;
	private bool step9 = false;
	
	private GameObject initialGrenades;
	
	private GameObject bonus1;
	private GameObject bonus2;
	private GameObject bonus3;
	
	private GameObject litfloorpanel;
	
	private GameObject platform1;
	private GameObject platform2;
	private GameObject platform3;
	
	private GameObject room1exit;
	private GameObject room2exit;
	private GameObject room3exit;
	
	private GameObject[] bonusItems;
	

	void Start(){
		manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
		StartCoroutine(MovementTutorial());
	}
	
	void Update(){
		if (Input.GetKeyDown(SettingsManager.keyBindings[(int)KeyBind.MoveForward])){
			F = true;
			Debug.Log("Player Moved Forwards");
		}
		if (Input.GetKeyDown(SettingsManager.keyBindings[(int)KeyBind.MoveRight])){
			R = true;
			Debug.Log("Player Moved Right");
		}
		if (Input.GetKeyDown(SettingsManager.keyBindings[(int)KeyBind.MoveBack])){
			B = true;
			Debug.Log("Player Moved Backwards");
		}
		if (Input.GetKeyDown(SettingsManager.keyBindings[(int)KeyBind.MoveLeft])){
			L = true;
			Debug.Log("Player Moved Left");
		}
		
		if (Input.GetKeyDown(SettingsManager.keyBindings[(int)KeyBind.JetUp])){
			U = true;
			Debug.Log("Player Flew Up");
		}
		if (Input.GetKeyDown(SettingsManager.keyBindings[(int)KeyBind.JetDown])){
			D = true;
			Debug.Log("Player Flew Down");
		}
		if (Input.GetKeyDown(SettingsManager.keyBindings[(int)KeyBind.StopMovement])){
			X = true;
			Debug.Log("Player Braked");
		}
		
		if (Input.GetKeyDown(SettingsManager.keyBindings[(int)KeyBind.RollLeft])){
			RL = true;
			Debug.Log("Player Rolled Left");
		}
		if (Input.GetKeyDown(SettingsManager.keyBindings[(int)KeyBind.RollRight])){
			RR = true;
			Debug.Log("Player Rolled Right");
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
		
		if (Input.GetKeyDown(SettingsManager.keyBindings[(int)KeyBind.Reload])){
			reloaded = true;
			Debug.Log("Player Reloaded");
		}
		if (Input.GetKeyDown(SettingsManager.keyBindings[(int)KeyBind.Grenade])){
			grenade = true;
			Debug.Log("Player Threw Grenade");
		}
		
		if(checkingitems){
			bonusItems = GameObject.FindGameObjectsWithTag("BonusPickup");
			if(bonusItems.Length > 3){
				items = false;
			} else {
				items = true;
			}
		}
		
		if(checkingtargets){
			bonusItems = GameObject.FindGameObjectsWithTag("BonusPickup");
			remainingtargets = 0;
			foreach(GameObject objecttest in bonusItems){
				if(objecttest.name == "Target"){
					remainingtargets ++;
				}
			}
			if(remainingtargets > 0){
				shotAllTargets = false;
			} else {
				shotAllTargets = true;
				Debug.Log("Player shot all targets");
			}
		}
		
		if (landedPlatforms > 2){
			platforms = true;
			Debug.Log("Player landed on all platforms");
		}


		if (F && B && L && R && step1){
			step1 = false;
			StartCoroutine(MovementTutorial2());
		}
		if (Floor1){
			litfloorpanel.transform.renderer.material.color = new Color(0, 0, 1);
			litfloorpanel = GameObject.Find("walkhere2");
			litfloorpanel.transform.renderer.material.color = new Color(0, 1, 0);
		}
		if (Floor2){
			litfloorpanel.transform.renderer.material.color = new Color(0, 0, 1);
			litfloorpanel = GameObject.Find("walkhere3");
			litfloorpanel.transform.renderer.material.color = new Color(0, 1, 0);
		}
		if (Floor3){
			litfloorpanel.transform.renderer.material.color = new Color(0, 0, 1);
		}
		if (Floor1 && Floor2 && Floor3 && step2){
			step2 = false;
			openroom2 = true;
			StartCoroutine(GunTutorial());
		}
		if (openroom2){
			room1exit = GameObject.Find("WalkRoomExit");
			if(room1exit.transform.position.y > -30){
				room1exit.transform.position = new Vector3(room1exit.transform.position.x,room1exit.transform.position.y-0.05f,room1exit.transform.position.z); 
			}
		}
		
		if (shot && aimed && reloaded && changedgun && step3){
			step3 = false;
			StartCoroutine(GunTutorial2());
		}
		
		if (shotAllTargets && step4){
			step4 = false;
			checkingtargets = false;
			StartCoroutine(FlightTutorial());
		}
		if (shotAllTargets){
			room2exit = GameObject.Find("GunRoomExit");
			if(room2exit.transform.position.y > -30){
				room2exit.transform.position = new Vector3(room2exit.transform.position.x,room2exit.transform.position.y-0.05f,room2exit.transform.position.z); 
			}
		}
		
		if (U && D && X && step5){
			step5 = false;
			StartCoroutine(FlightTutorial2());
		}
		if (RL && RR && step6){
			step6 = false;
			StartCoroutine(NavigationTutorial());
		}
		
		if (platforms && step7){
			step7 = false;
			StartCoroutine(GrenadeTutorial());
		}
		if (platforms){
			room3exit = GameObject.Find("FlightRoomExit");
			if(room3exit.transform.position.y > -30){
				room3exit.transform.position = new Vector3(room3exit.transform.position.x,room3exit.transform.position.y-0.05f,room3exit.transform.position.z); 
			}
		}
		
		if (grenade && step8){
			step8 = false;
			StartCoroutine(ItemTutorial());
		}
		
		if (items && step9){
			checkingitems = false;
			step9 = false;
			StartCoroutine(FinalTutorial());
		}
	}
	
	IEnumerator MovementTutorial(){
		//black out screen
		GuiManager.instance.blackOutScreen = true;
		manager.GetComponent<GuiManager>().TutorialPrompt("Welcome to the SC1830 Utility Suit.\n\nCalibrating.", 6000);
		yield return new WaitForSeconds(.1f);
		player = GameObject.Find("NoGravPlayer(Clone)");
		player.GetComponent<KeyboardInput>().canWalk = false;
		player.GetComponent<KeyboardInput>().canJump = false;
		yield return new WaitForSeconds(1);
		//damage player so they can pick up Medikit
		//GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerResources>().TakeDamage(10, Network.player);
		//give player mines
		initialGrenades = GameObject.Find("InitialGrenadePack");
		//initialGrenades.transform.position = new Vector3(0,-32,0);
		manager.GetComponent<GuiManager>().TutorialPrompt("Welcome to the SC1830 Utility Suit.\n\nCalibrating..", 6000);
		yield return new WaitForSeconds(1);
		Destroy(initialGrenades);
		manager.GetComponent<GuiManager>().TutorialPrompt("Welcome to the SC1830 Utility Suit.\n\nCalibrating...", 6000);
		yield return new WaitForSeconds(1);
		manager.GetComponent<GuiManager>().TutorialPrompt("Welcome to the SC1830 Utility Suit.\n\nCalibrating....", 6000);
		yield return new WaitForSeconds(1);
		manager.GetComponent<GuiManager>().TutorialPrompt("Welcome to the SC1830 Utility Suit.\n\nSuit Calibrated.", 6000);
		yield return new WaitForSeconds(2);
		GuiManager.instance.blackOutScreen = false;
		manager.GetComponent<GuiManager>().TutorialPrompt("Welcome to the SC1830 Utility Suit.\n\nSuit Calibrated.\n\nRunning Tutorial Simulation.", 6000);
		yield return new WaitForSeconds(4);
		manager.GetComponent<GuiManager>().TutorialPrompt("\nMove your Mouse to look around.", 99999);
		yield return new WaitForSeconds(5);
		player.GetComponent<KeyboardInput>().canWalk = true;
		manager.GetComponent<GuiManager>().TutorialPrompt("\nUse "
            +SettingsManager.keyBindings[(int)KeyBind.MoveForward].ToString()+", "
            +SettingsManager.keyBindings[(int)KeyBind.MoveLeft].ToString()+", "
            +SettingsManager.keyBindings[(int)KeyBind.MoveBack].ToString()+" and "
            +SettingsManager.keyBindings[(int)KeyBind.MoveRight].ToString()+" to move around.", 99999);
		yield return new WaitForSeconds(5);
		step1 = true;
	}
	IEnumerator MovementTutorial2(){
		checkingfloortiles = true;
		litfloorpanel = GameObject.Find("walkhere1");
		litfloorpanel.transform.renderer.material.color = new Color(0, 1, 0);
		manager.GetComponent<GuiManager>().TutorialPrompt("\nWalk onto the green square.", 99999);
		yield return new WaitForSeconds(5);
		step2 = true;
	}
	
	IEnumerator GunTutorial(){
		manager.GetComponent<GuiManager>().TutorialPrompt("\nWell done!\n\nPlease proceed to the next room.", 99999);
		yield return new WaitForSeconds(7);
		bonus2 = GameObject.Find("TutorialBonusWeaponPickup1");
		bonus2.transform.position = new Vector3(-168,-12,12);
		bonus2 = GameObject.Find("TutorialBonusWeaponPickup2");
		bonus2.transform.position = new Vector3(-160,-12,12);
		bonus2 = GameObject.Find("TutorialBonusWeaponPickup3");
		bonus2.transform.position = new Vector3(-152,-12,12);
		manager.GetComponent<GuiManager>().TutorialPrompt("Some guns have been spawned, to pick up a gun simply walk over it.\n\nIf you already have 2 guns you can opt to swap the gun you are currently holding.", 99999);
		yield return new WaitForSeconds(7);
		manager.GetComponent<GuiManager>().TutorialPrompt("Click the left Mouse Button to shoot and use the right Mouse Button to aim.\n\nUse the Mouse Wheel or Numbers to change weapons.\n\nPress "
		+SettingsManager.keyBindings[(int)KeyBind.Reload].ToString()+" to reload.", 99999);
		yield return new WaitForSeconds(5);
		step3 = true;
	}
	
	IEnumerator GunTutorial2(){
		checkingtargets = true;
		manager.GetComponent<GuiManager>().TutorialPrompt("\nTry to shoot all the targets.", 99999);
		yield return new WaitForSeconds(5);
		step4 = true;
	}
	
	IEnumerator FlightTutorial(){
		manager.GetComponent<GuiManager>().TutorialPrompt("\nWell done!\n\nPlease proceed to the next room.", 99999);
		yield return new WaitForSeconds(7);
		player.GetComponent<KeyboardInput>().canJump = true;
		manager.GetComponent<GuiManager>().TutorialPrompt("This suit comes equipped with Electro-Gravitational Boots.\n\nTo land on a surface you must rotate yourself so you hit the surface feet first.", 99999);
		yield return new WaitForSeconds(5);
		manager.GetComponent<GuiManager>().TutorialPrompt("Use "+SettingsManager.keyBindings[(int)KeyBind.JetUp].ToString()+" to boost upwards.\n\nUse "
            +SettingsManager.keyBindings[(int)KeyBind.JetDown].ToString()+" to boost downwards.\n\nUse "
			+SettingsManager.keyBindings[(int)KeyBind.StopMovement].ToString()+" to brake.", 99999);
		yield return new WaitForSeconds(5);
		step5 = true;		
	}
	IEnumerator FlightTutorial2(){
		manager.GetComponent<GuiManager>().TutorialPrompt("Use "
            +SettingsManager.keyBindings[(int)KeyBind.RollLeft].ToString()+" to roll to the left and "
            +SettingsManager.keyBindings[(int)KeyBind.RollRight].ToString()+" to roll to the right.\n\nYou can also rotate by moving the mouse while floating.", 99999);
		yield return new WaitForSeconds(5);
		step6 = true;		
	}
	
	IEnumerator NavigationTutorial(){
		platform1 = GameObject.Find("LandingPlatform1");
		platform2 = GameObject.Find("LandingPlatform2");
		platform3 = GameObject.Find("LandingPlatform3");
		platform1.transform.position = new Vector3(platform1.transform.position.x,0,platform1.transform.position.z); 
		platform2.transform.position = new Vector3(platform2.transform.position.x,0,platform2.transform.position.z); 
		platform3.transform.position = new Vector3(platform3.transform.position.x,0,platform3.transform.position.z); 
		manager.GetComponent<GuiManager>().TutorialPrompt("\nTry to land on these platforms.", 99999);
		yield return new WaitForSeconds(5);
		step7 = true;
	}
	
	IEnumerator GrenadeTutorial(){
		manager.GetComponent<GuiManager>().TutorialPrompt("\nWell done!\n\nPlease proceed to the next room.", 99999);
		yield return new WaitForSeconds(7);
		bonus1 = GameObject.Find("TutorialBonusGrenadePack");
		bonus1.transform.position = new Vector3(0,-32,0); 
		manager.GetComponent<GuiManager>().TutorialPrompt("Some grenade boxes have been spawned.\n\nThere are 3 types of grenades: Black Hole, EMP and Frag.\n\nPress "
			+SettingsManager.keyBindings[(int)KeyBind.Grenade].ToString()+" to throw a Proximity Grenade.\n\nKeep in mind that without gravity, the grenades will fly in a straight line.", 99999);
		yield return new WaitForSeconds(5);
		step8 = true;
	}
	
	IEnumerator ItemTutorial(){
		checkingitems = true;
		manager.GetComponent<GuiManager>().TutorialPrompt(
            "This green box is a Medikit, it will restore any damage to your suit.\n\nThey can be picked up by touching them.\nYou can also shoot items to stop others from getting them.", 9999);
		bonus3 = GameObject.Find("TutorialBonusHealthPack");
		bonus3.transform.position = new Vector3(0,-32,0);
		yield return new WaitForSeconds(5);
		step9 = true;
	}
	
	IEnumerator FinalTutorial(){
		manager.GetComponent<GuiManager>().TutorialPrompt("Keep in mind that this suit uses air as fuel.\n\nIf you run low on air the suit will automatically\ndisable boosting temporarily while it generates more.", 99999);
		yield return new WaitForSeconds(60);
		manager.GetComponent<GuiManager>().TutorialPrompt("At the right of your HUD the suit displays important information including:\n\nmine count, ammo count, remaining air and suit structural integrity.\n\nIf the structural integrity of the suit is compromised you will lose\nboth pressurization and air supply, resulting in death.", 99999);
		yield return new WaitForSeconds(60);
		manager.GetComponent<GuiManager>().TutorialPrompt("When you're ready to end the simulation, press Escape.", 99999);
	}
}