using UnityEngine;
using System.Collections;
     
public class TutorialInstructions : MonoBehaviour {
	private PlayerResources playerRes;
	private GameObject player;
    private IControllerInput playerInput;
	
	private int dotLooks = 0;
	private GameObject lookingDot;
	private bool LookingExplained = false;
	
	private bool check1 = false;
	private bool F = false;
	private bool B = false;
	private bool L = false;
	private bool R = false;
	private bool MovementKeyPressExplained = false;
	
	public bool checkingfloortiles = false;
	public bool Floor1 = false;
	public bool Floor2 = false;
	public bool Floor3 = false;
	private bool openroom2 = false;
	private bool MovementOnTilesExplained = false;
	
	private bool check3 = false;
	private bool shot = false;
	private bool aimed = false;
	private bool changedgun = false;
	private bool GunsExplained = false;
	
	private bool checkingtargets = false;
	private int remainingtargets;
	private bool shotAllTargets = false;
	private bool GunShootTargetsExplained = false;
	
	private bool check5 = false;
	private bool U = false;
	private bool D = false;
	private bool X = false;
	private bool FlightExplained = false;
	
	private bool check6 = false;
	private bool RL = false;
	private bool RR = false;
	private bool FlightRollingExplained = false;
	
	public int landedPlatforms = 0;
	private bool platforms = false;
	private GameObject platform;
	private bool NavigationExplained = false;
	
	private bool check8 = false;
	private bool grenade = false;
	private bool changedgrenade = false;
	private bool GrenadeExplained = false;
	
	private GameObject healthpack;
	
	private GameObject litfloorpanel;
	
	private GameObject platform1;
	private GameObject platform2;
	private GameObject platform3;
	
	private GameObject room1exit;
	private GameObject room2exit;
	private GameObject room3exit;
	
	private GameObject[] bonusItems;

    delegate void TutorialStep();
    TutorialStep tutorialStep;

	void Start(){
        GameManager.instance.SpawnActor();
		lookingDot = GameObject.Find("LookAtDot");
		platform = GameObject.Find("PlatformFront1");
		platform.transform.renderer.material.color = new Color(0, 1, 0);
		platform = GameObject.Find("PlatformFront2");
		platform.transform.renderer.material.color = new Color(0, 1, 0);
		platform = GameObject.Find("PlatformFront3");
		platform.transform.renderer.material.color = new Color(0, 1, 0);
		StartCoroutine(LookTutorial());

        // Assign delegate
        tutorialStep = SearchForPlayer;
	}
	
	void Update(){
        tutorialStep();

        //-- After this, nothing
		
		if(check5){
            FlightCheck();
		}
		
		if(check6){
            RollCheck();
		}
		
		if(check8){
            GrenadeCheck();
		}
		
		if(checkingtargets){
            TargetCheck();
		}

        PlatformLandingCheck();



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
		if (Floor1 && Floor2 && Floor3 && MovementOnTilesExplained){
			MovementOnTilesExplained = false;
			openroom2 = true;
			StartCoroutine(GunTutorial());
		}
		if (openroom2){
			room1exit = GameObject.Find("WalkRoomExit");
			if(room1exit.transform.position.y > -30){
				room1exit.transform.position = new Vector3(room1exit.transform.position.x,room1exit.transform.position.y-0.07f,room1exit.transform.position.z); 
			}
		}
		
		if (shot && aimed && changedgun && GunsExplained){
			GunsExplained = false;
			StartCoroutine(GunTutorial2());
		}
		
		if (shotAllTargets && GunShootTargetsExplained){
			GunShootTargetsExplained = false;
			checkingtargets = false;
			StartCoroutine(FlightTutorial());
		}
		if (shotAllTargets){
			room2exit = GameObject.Find("GunRoomExit");
			if(room2exit.transform.position.y > -30){
				room2exit.transform.position = new Vector3(room2exit.transform.position.x,room2exit.transform.position.y-0.07f,room2exit.transform.position.z); 
			}
		}
		
		if (U && D && X && FlightExplained){
			FlightExplained = false;
			StartCoroutine(FlightTutorial2());
		}
		if (RL && RR && FlightRollingExplained){
			FlightRollingExplained = false;
			StartCoroutine(NavigationTutorial());
		}
		
		if (platforms && NavigationExplained){
			NavigationExplained = false;
			StartCoroutine(GrenadeTutorial());
		}
		if (platforms){
			room3exit = GameObject.Find("FlightRoomExit");
			if(room3exit.transform.position.y > -30){
				room3exit.transform.position = new Vector3(room3exit.transform.position.x,room3exit.transform.position.y-0.07f,room3exit.transform.position.z); 
			}
		}
		
		if (grenade && changedgrenade && GrenadeExplained){
			GrenadeExplained = false;
			StartCoroutine(ItemTutorial());
		}
	}

    private void PlatformLandingCheck() {
        if (landedPlatforms > 2) {
            platforms = true;
            Debug.Log("Player landed on all platforms");
        }
    }

    private void TargetCheck() {
        bonusItems = GameObject.FindGameObjectsWithTag("BonusPickup");
        remainingtargets = 0;
        foreach (GameObject objecttest in bonusItems) {
            if (objecttest.name == "Target" || objecttest.name == "movingTarget1" || objecttest.name == "movingTarget2") {
                remainingtargets++;
            }
        }
        if (remainingtargets > 0) {
            shotAllTargets = false;
        } else {
            shotAllTargets = true;
            Debug.Log("Player shot all targets");
        }
    }

    private void GrenadeCheck() {
        if (InputConverter.GetKeyDown(KeyBind.Grenade)) {
            grenade = true;
            Debug.Log("Player Threw Grenade");
        }
        if (InputConverter.GetKeyDown(KeyBind.GrenadeSwitch)) {
            changedgrenade = true;
            Debug.Log("Player Changed Grenade-type");
        }
    }

    private void RollCheck() {
        if (InputConverter.GetKeyDown(KeyBind.RollLeft)) {
            RL = true;
            Debug.Log("Player Rolled Left");
        }
        if (InputConverter.GetKeyDown(KeyBind.RollRight)) {
            RR = true;
            Debug.Log("Player Rolled Right");
        }
    }

    private void FlightCheck() {
        if (InputConverter.GetKeyDown(KeyBind.JetUp)) {
            U = true;
            Debug.Log("Player Flew Up");
        }
        if (InputConverter.GetKeyDown(KeyBind.JetDown)) {
            D = true;
            Debug.Log("Player Flew Down");
        }
        if (InputConverter.GetKeyDown(KeyBind.StopMovement)) {
            X = true;
            Debug.Log("Player Braked");
        }
    }

    private void GunControls() {
        if (Input.GetMouseButtonDown(0)) {
            shot = true;
            Debug.Log("Player Shot");
        }
        if (Input.GetMouseButtonDown(1)) {
            aimed = true;
            Debug.Log("Player Aimed");
        }
        if (Input.GetKeyDown("2") || Input.GetAxis("Mouse ScrollWheel") < 0 || Input.GetAxis("Mouse ScrollWheel") > 0) {
            changedgun = true;
            Debug.Log("Player Changed Weapons");
        }
    }

    private void CheckPlayerMovement() {
        if (InputConverter.GetKeyDown(KeyBind.MoveForward)) {
            F = true;
            Debug.Log("Player Moved Forwards");
        }
        if (InputConverter.GetKeyDown(KeyBind.MoveRight)) {
            R = true;
            Debug.Log("Player Moved Right");
        }
        if (InputConverter.GetKeyDown(KeyBind.MoveBack)) {
            B = true;
            Debug.Log("Player Moved Backwards");
        }
        if (InputConverter.GetKeyDown(KeyBind.MoveLeft)) {
            L = true;
            Debug.Log("Player Moved Left");
        }

        if (F && B && L && R && MovementKeyPressExplained) {
            MovementKeyPressExplained = false;
            StartCoroutine(MovementTutorialTiles());
        }
    }

    private void SearchForPlayer() {
        player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null) { // Did it work?
            tutorialStep = LookAtDots;
        }
    }

    private void LookAtDots() {
        float minangle = .75f;
        if (Vector3.Angle(player.transform.FindChild("CameraPos").forward, lookingDot.transform.position - player.transform.position) < minangle) {
            Debug.Log("Player Looked at Dot");
            dotLooks++;
        }
        if (dotLooks == 1) {
            lookingDot.transform.position = new Vector3(-358f, -10.6f, -3f);
        }
        if (dotLooks == 2) {
            lookingDot.transform.position = new Vector3(-358f, -7.6f, 0f);
        }
        if (dotLooks == 3) {
            lookingDot.transform.position = new Vector3(-358f, -10.6f, 3f);
        }
        if (dotLooks == 4) {
            lookingDot.transform.position = new Vector3(-358f, -13.6f, 0f);
        }

        // Done
        if (dotLooks > 4) {
            lookingDot.transform.position = new Vector3(-358f, -120f, 0f);
            StartCoroutine(MovementTutorialKeyPress());
        }
    }
	
	IEnumerator LookTutorial(){
		//black out screen
        tutorialStep = LookAtDots;
		GuiManager.instance.blackOutScreen = true;
		ChatManager.TutorialChat("Welcome to the SC1830 Utility Suit.\n\nCalibrating.");
        while (player == null) {
            yield return new WaitForSeconds(.1f);
        }
        playerInput = player.GetComponent(typeof(IControllerInput)) as IControllerInput;
        playerInput.canWalk = false;
        playerInput.canJump = false;
            
        ChatManager.TutorialChat("Welcome to the SC1830 Utility Suit.\n\nCalibrating..");
		yield return new WaitForSeconds(1);
        ChatManager.TutorialChat("Welcome to the SC1830 Utility Suit.\n\nCalibrating...");
		yield return new WaitForSeconds(1);
		ChatManager.TutorialChat("Welcome to the SC1830 Utility Suit.\n\nCalibrating....");
		yield return new WaitForSeconds(1);
		ChatManager.TutorialChat("Welcome to the SC1830 Utility Suit.\n\nSuit Calibrated.");
		yield return new WaitForSeconds(2);
		GuiManager.instance.blackOutScreen = false;
		ChatManager.TutorialChat("Welcome to the SC1830 Utility Suit.\n\nSuit Calibrated.\n\nRunning Tutorial Simulation.");
		yield return new WaitForSeconds(4);
		lookingDot.transform.FindChild("DotMesh").renderer.material.color = new Color(0, 0, 200, 200);
		lookingDot.transform.position = new Vector3(-358f,-10.6f,0f);
		ChatManager.TutorialChat("\nMove your Mouse to look around.\n\nPlease look at the blue dot.");
		yield return new WaitForSeconds(5);
		LookingExplained = true;
	}
	IEnumerator MovementTutorialKeyPress(){
        tutorialStep = CheckPlayerMovement;
		playerInput.canWalk = true;

		ChatManager.TutorialChat("\nPress "
            +InputConverter.GeyKeyName(KeyBind.MoveForward)+" to move forwards.");
		yield return new WaitForSeconds(1);
		ChatManager.TutorialChat("\nPress "
            + InputConverter.GeyKeyName(KeyBind.MoveForward) + " to move forwards.\nPress "
            + InputConverter.GeyKeyName(KeyBind.MoveLeft) + " to move Left.");
		yield return new WaitForSeconds(1);
		ChatManager.TutorialChat("\nPress "
            + InputConverter.GeyKeyName(KeyBind.MoveForward) + " to move forwards.\nPress "
            + InputConverter.GeyKeyName(KeyBind.MoveLeft) + " to move Left.\nPress "
            + InputConverter.GeyKeyName(KeyBind.MoveRight) + " to move Right.");
		yield return new WaitForSeconds(1);
		ChatManager.TutorialChat("\nPress "
            + InputConverter.GeyKeyName(KeyBind.MoveForward) + " to move forwards.\nPress "
            +InputConverter.GeyKeyName(KeyBind.MoveLeft)+" to move Left.\nPress "
            +InputConverter.GeyKeyName(KeyBind.MoveRight)+" to move Right.\nPress "
            +InputConverter.GeyKeyName(KeyBind.MoveBack)+" to move Backwards.");
		yield return new WaitForSeconds(1);
		MovementKeyPressExplained = true;
	}
	IEnumerator MovementTutorialTiles(){
		checkingfloortiles = true;
		litfloorpanel = GameObject.Find("walkhere1");
		litfloorpanel.transform.renderer.material.color = new Color(0, 1, 0);
		ChatManager.TutorialChat("\nWalk onto the green squares.");
		yield return new WaitForSeconds(5);
		MovementOnTilesExplained = true;
	}
	
	IEnumerator GunTutorial(){
		ChatManager.TutorialChat("\nWell done!\n\nPlease proceed to the next room.");
		yield return new WaitForSeconds(7);
        tutorialStep = GunControls;
		ChatManager.TutorialChat("\nTo pick up a gun simply walk over it.\n\nIf you already have 2 guns you can swap out the gun you are currently holding.");
		yield return new WaitForSeconds(10);
		ChatManager.TutorialChat("Click the left Mouse Button to shoot and use the right Mouse Button to aim.\n\nUse the Mouse Wheel or Numbers to change weapons.\n\nPress "
		+SettingsManager.keyBindings[(int)KeyBind.Reload].ToString()+" to reload.");
		yield return new WaitForSeconds(5);
		GunsExplained = true;
	}
	
	IEnumerator GunTutorial2(){
		checkingtargets = true;
		ChatManager.TutorialChat("\nTry to shoot all the targets.");
		yield return new WaitForSeconds(5);
		GunShootTargetsExplained = true;
	}
	
	IEnumerator FlightTutorial(){
		ChatManager.TutorialChat("\nWell done!\n\nPlease proceed to the next room.");
		yield return new WaitForSeconds(7);
		check5 = true;
		player.GetComponent<KeyboardInput>().canJump = true;
		ChatManager.TutorialChat("This suit comes equipped with Electro-Gravitational Boots.\n\nTo land on a surface you must rotate yourself so you hit the surface feet first.\n\nPress "+SettingsManager.keyBindings[(int)KeyBind.JetUp].ToString()+" to jump and boost upwards.");
		yield return new WaitForSeconds(10);
		ChatManager.TutorialChat("\nPress "
            +SettingsManager.keyBindings[(int)KeyBind.JetDown].ToString()+" to boost downwards.\n\nPress "
			+SettingsManager.keyBindings[(int)KeyBind.StopMovement].ToString()+" to brake.");
		yield return new WaitForSeconds(8);
		FlightExplained = true;		
	}
	IEnumerator FlightTutorial2(){
		check6 = true;
		ChatManager.TutorialChat("\nPress "
            +SettingsManager.keyBindings[(int)KeyBind.RollLeft].ToString()+" to roll to the left and "
            +SettingsManager.keyBindings[(int)KeyBind.RollRight].ToString()+" to roll to the right.\n\nYou can also rotate by moving the mouse while floating.");
		yield return new WaitForSeconds(5);
		FlightRollingExplained = true;		
	}
	
	IEnumerator NavigationTutorial(){
		platform1 = GameObject.Find("LandingPlatform1");
		platform2 = GameObject.Find("LandingPlatform2");
		platform3 = GameObject.Find("LandingPlatform3");
		platform1.transform.position = new Vector3(platform1.transform.position.x,0,platform1.transform.position.z); 
		platform2.transform.position = new Vector3(platform2.transform.position.x,0,platform2.transform.position.z); 
		platform3.transform.position = new Vector3(platform3.transform.position.x,0,platform3.transform.position.z); 
		ChatManager.TutorialChat("\nTry to land on these platforms.\n\nRemember to 'look up' before you crash so you land feet first.");
		yield return new WaitForSeconds(5);
		NavigationExplained = true;
	}
	
	IEnumerator GrenadeTutorial(){
		ChatManager.TutorialChat("\nWell done!\n\nPlease proceed to the next room.");
		yield return new WaitForSeconds(7);
		check8 = true;
		ChatManager.TutorialChat("The purple boxes contain grenades.\nThere are 3 types of grenades: Black Hole, EMP and Frag.\n\nPress "
			+SettingsManager.keyBindings[(int)KeyBind.Grenade].ToString()+" to throw a Proximity Grenade.\nPress "+SettingsManager.keyBindings[(int)KeyBind.GrenadeSwitch].ToString()+" to change grenade type.\n\nKeep in mind that without gravity, the grenades will fly in a straight line.");
		yield return new WaitForSeconds(8);
		GrenadeExplained = true;
	}
	
	IEnumerator ItemTutorial(){
		ChatManager.TutorialChat(
            "\nIf a grenade hits a person it will detonate instantly.\n\nYou can also set off grenades by shooting them.");
		yield return new WaitForSeconds(8);
		healthpack = GameObject.Find("TutorialBonusHealthPack");
		healthpack.transform.position = new Vector3(142.22f,13.45f,-0.17f);
		ChatManager.TutorialChat(
            "\nThis green box is a Medkit, it will restore damage to your suit.\n\nThey can be picked up by touching them.\nYou can also shoot them to destroy them and stop others from using them.");
		yield return new WaitForSeconds(10);
		StartCoroutine(FinalTutorial());
	}
	
	IEnumerator FinalTutorial(){
		ChatManager.TutorialChat("\nKeep in mind that this suit uses air as fuel.\n\nIf you run low on air the suit will automatically\ndisable boosting temporarily while it generates more.");
		yield return new WaitForSeconds(30);
		ChatManager.TutorialChat("At the bottom left of your HUD the suit displays a radar.\nThis shows you the locations of items and other players.\n\nThe green dot represents you and the triangle at the top is your field of view.\nPlayers or items within the triange are in front of you, when items or players are\nabove you their dot is larger than yours, when they are below you their dot is smaller.");
		yield return new WaitForSeconds(30);
		ChatManager.TutorialChat("At the bottom right of your HUD the suit displays important information including:\n\nmine count, ammo count, remaining air and suit structural integrity.\n\nIf the structural integrity of the suit is compromised you will lose\nboth pressurization and air supply, resulting in death.");
		yield return new WaitForSeconds(30);
		ChatManager.TutorialChat("\nWhen you're ready to end the simulation, press Escape.");
	}
}