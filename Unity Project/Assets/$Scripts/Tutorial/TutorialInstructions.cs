using UnityEngine;
using System.Collections;

public class TutorialInstructions : MonoBehaviour {
    private PlayerResources playerRes;
    private GameObject player;

    private int dotLooks = 0;
    private GameObject lookingDot;
    private bool step0 = false;

    private bool check1 = false;
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

    private bool check3 = false;
    private bool shot = false;
    private bool aimed = false;
    private bool changedgun = false;
    private bool step3 = false;

    private bool checkingtargets = false;
    private int remainingtargets;
    private bool shotAllTargets = false;
    private bool step4 = false;

    private bool check5 = false;
    private bool U = false;
    private bool D = false;
    private bool X = false;
    private bool step5 = false;

    private bool check6 = false;
    private bool RL = false;
    private bool RR = false;
    private bool step6 = false;

    public int landedPlatforms = 0;
    private bool platforms = false;
    private GameObject platform;
    private bool step7 = false;

    private bool check8 = false;
    private bool grenade = false;
    private bool changedgrenade = false;
    private bool step8 = false;

    private GameObject healthpack;

    private GameObject litfloorpanel;

    private GameObject platform1;
    private GameObject platform2;
    private GameObject platform3;

    private GameObject room1exit;
    private GameObject room2exit;
    private GameObject room3exit;

    private GameObject[] bonusItems;


    void Start() {
        GameManager.instance.SpawnActor();
        lookingDot = GameObject.Find("LookAtDot");
        platform = GameObject.Find("PlatformFront1");
        platform.transform.renderer.material.color = new Color(0, 1, 0);
        platform = GameObject.Find("PlatformFront2");
        platform.transform.renderer.material.color = new Color(0, 1, 0);
        platform = GameObject.Find("PlatformFront3");
        platform.transform.renderer.material.color = new Color(0, 1, 0);
        UIManager.instance.GoPlayerHUD();
        StartCoroutine(SearchForPlayer());
    }

    void Update() {
        if (player == null) return;
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

        if (check1) {
            if (Input.GetKeyDown(SettingsManager.keyBindings[(int)KeyBind.MoveForward])) {
                F = true;
                Debug.Log("Player Moved Forwards");
            }
            if (Input.GetKeyDown(SettingsManager.keyBindings[(int)KeyBind.MoveRight])) {
                R = true;
                Debug.Log("Player Moved Right");
            }
            if (Input.GetKeyDown(SettingsManager.keyBindings[(int)KeyBind.MoveBack])) {
                B = true;
                Debug.Log("Player Moved Backwards");
            }
            if (Input.GetKeyDown(SettingsManager.keyBindings[(int)KeyBind.MoveLeft])) {
                L = true;
                Debug.Log("Player Moved Left");
            }
        }

        if (check3) {
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

        if (check5) {
            if (Input.GetKeyDown(SettingsManager.keyBindings[(int)KeyBind.JetUp])) {
                U = true;
                Debug.Log("Player Flew Up");
            }
            if (Input.GetKeyDown(SettingsManager.keyBindings[(int)KeyBind.JetDown])) {
                D = true;
                Debug.Log("Player Flew Down");
            }
            if (Input.GetKeyDown(SettingsManager.keyBindings[(int)KeyBind.StopMovement])) {
                X = true;
                Debug.Log("Player Braked");
            }
        }

        if (check6) {
            if (Input.GetKeyDown(SettingsManager.keyBindings[(int)KeyBind.RollLeft])) {
                RL = true;
                Debug.Log("Player Rolled Left");
            }
            if (Input.GetKeyDown(SettingsManager.keyBindings[(int)KeyBind.RollRight])) {
                RR = true;
                Debug.Log("Player Rolled Right");
            }
        }

        if (check8) {
            if (Input.GetKeyDown(SettingsManager.keyBindings[(int)KeyBind.Grenade])) {
                grenade = true;
                Debug.Log("Player Threw Grenade");
            }
            if (Input.GetKeyDown(SettingsManager.keyBindings[(int)KeyBind.GrenadeSwitch])) {
                changedgrenade = true;
                Debug.Log("Player Changed Grenade-type");
            }
        }

        if (checkingtargets) {
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

        if (landedPlatforms > 2) {
            platforms = true;
            Debug.Log("Player landed on all platforms");
            landedPlatforms = 0; // Fix message Spam
        }


        if (dotLooks > 4 && step0) {
            lookingDot.transform.position = new Vector3(-358f, -120f, 0f);
            step0 = false;
            StartCoroutine(MovementTutorial());
        }
        if (F && B && L && R && step1) {
            step1 = false;
            StartCoroutine(MovementTutorial2());
        }
        if (Floor1) {
            litfloorpanel.transform.renderer.material.color = new Color(0, 0, 1);
            litfloorpanel = GameObject.Find("walkhere2");
            litfloorpanel.transform.renderer.material.color = new Color(0, 1, 0);
        }
        if (Floor2) {
            litfloorpanel.transform.renderer.material.color = new Color(0, 0, 1);
            litfloorpanel = GameObject.Find("walkhere3");
            litfloorpanel.transform.renderer.material.color = new Color(0, 1, 0);
        }
        if (Floor3) {
            litfloorpanel.transform.renderer.material.color = new Color(0, 0, 1);
        }
        if (Floor1 && Floor2 && Floor3 && step2) {
            step2 = false;
            openroom2 = true;
            StartCoroutine(GunTutorial());
        }
        if (openroom2) {
            room1exit = GameObject.Find("WalkRoomExit");
            if (room1exit.transform.position.y > -30) {
                room1exit.transform.position = new Vector3(room1exit.transform.position.x, room1exit.transform.position.y - 0.07f, room1exit.transform.position.z);
            }
        }

        if (shot && aimed && changedgun && step3) {
            step3 = false;
            StartCoroutine(GunTutorial2());
        }

        if (shotAllTargets && step4) {
            step4 = false;
            checkingtargets = false;
            StartCoroutine(FlightTutorial());
        }
        if (shotAllTargets) {
            room2exit = GameObject.Find("GunRoomExit");
            if (room2exit.transform.position.y > -30) {
                room2exit.transform.position = new Vector3(room2exit.transform.position.x, room2exit.transform.position.y - 0.07f, room2exit.transform.position.z);
            }
        }

        if (U && D && X && step5) {
            step5 = false;
            StartCoroutine(FlightTutorial2());
        }
        if (RL && RR && step6) {
            step6 = false;
            StartCoroutine(NavigationTutorial());
        }

        if (platforms && step7) {
            step7 = false;
            StartCoroutine(GrenadeTutorial());
        }
        if (platforms) {
            room3exit = GameObject.Find("FlightRoomExit");
            if (room3exit.transform.position.y > -30) {
                room3exit.transform.position = new Vector3(room3exit.transform.position.x, room3exit.transform.position.y - 0.07f, room3exit.transform.position.z);
            }
        }

        if (grenade && changedgrenade && step8) {
            step8 = false;
            StartCoroutine(ItemTutorial());
        }
    }
    void ClearScreen() {
        Debug.Log("Clear screen called.");
        GetComponent<ScreenFadeInOut>().FadeToClear();
    }
    IEnumerator SearchForPlayer() {
        while (player == null) {
            player = GameObject.FindGameObjectWithTag("Player");
            yield return null;
        }
        StartCoroutine(LookTutorial());
    }
    IEnumerator LookTutorial() {
        Invoke("ClearScreen", 6f); // Time it takes to get through starting messages
        ChatManager.TutorialChat("Welcome to the SC1830 Utility Suit.\n\nCalibrating.");
        yield return new WaitForSeconds(.1f);
        player = GameObject.Find("NoGravPlayer(Clone)");
        player.GetComponent<KeyboardInput>().canWalk = false;
        player.GetComponent<KeyboardInput>().canJump = false;
        yield return new WaitForSeconds(1);
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
        lookingDot.transform.position = new Vector3(-358f, -10.6f, 0f);
        ChatManager.TutorialChat("\nPlease look at the blue dot.");
		UIPlayerHUD.TutorialPrompt("\nMove your Mouse to look around.\n\n\n\n\nPress Space to continue.");
        yield return new WaitForSeconds(5);
        step0 = true;
    }
    IEnumerator MovementTutorial() {
        check1 = true;
        player.GetComponent<KeyboardInput>().canWalk = true;
        ChatManager.TutorialChat("Use "+ SettingsManager.keyBindings[(int)KeyBind.MoveForward].ToString() + ", " + SettingsManager.keyBindings[(int)KeyBind.MoveLeft].ToString()
		 + ", " + SettingsManager.keyBindings[(int)KeyBind.MoveBack].ToString()  + " & " + SettingsManager.keyBindings[(int)KeyBind.MoveRight].ToString() + " to move around.");
		UIPlayerHUD.TutorialPrompt("\nPress "
            + SettingsManager.keyBindings[(int)KeyBind.MoveForward].ToString() + " to move forwards.\n\nPress "
            + SettingsManager.keyBindings[(int)KeyBind.MoveLeft].ToString() + " to move Left.\n\nPress "
            + SettingsManager.keyBindings[(int)KeyBind.MoveRight].ToString() + " to move Right.\n\nPress "
            + SettingsManager.keyBindings[(int)KeyBind.MoveBack].ToString() + " to move Backwards.\n\n\n\n\nPress Space to continue.");
        yield return new WaitForSeconds(4);
        step1 = true;
    }
    IEnumerator MovementTutorial2() {
        checkingfloortiles = true;
        litfloorpanel = GameObject.Find("walkhere1");
        litfloorpanel.transform.renderer.material.color = new Color(0, 1, 0);
        ChatManager.TutorialChat("\nWalk onto the green squares.");
        yield return new WaitForSeconds(5);
        step2 = true;
    }

    IEnumerator GunTutorial() {
        ChatManager.TutorialChat("\nWell done!\n\nPlease proceed to the next room.");
        yield return new WaitForSeconds(7);
        check3 = true;
        ChatManager.TutorialChat("\nTo pick up a gun simply walk over it.\n\nIf you already have 2 guns you can swap out the gun you are currently holding.");
        yield return new WaitForSeconds(10);
        UIPlayerHUD.TutorialPrompt("\nClick the left Mouse Button to shoot and use the right Mouse Button to aim.\n\nUse the Mouse Wheel or Numbers to change weapons.\n\nPress "
        + SettingsManager.keyBindings[(int)KeyBind.Reload].ToString() + " to reload.\n\n\n\n\nPress Space to continue.");
        yield return new WaitForSeconds(5);
        step3 = true;
    }

    IEnumerator GunTutorial2() {
        checkingtargets = true;
        ChatManager.TutorialChat("\nTry to shoot all the targets.");
        yield return new WaitForSeconds(5);
        step4 = true;
    }

    IEnumerator FlightTutorial() {
        ChatManager.TutorialChat("\nWell done!\n\nPlease proceed to the next room.");
        yield return new WaitForSeconds(7);
        check5 = true;
        player.GetComponent<KeyboardInput>().canJump = true;
        ChatManager.TutorialChat("This suit comes equipped with Electro-Gravitational Boots.\nTo land on a surface you must rotate yourself so you hit the surface feet first.\nPress " +
            SettingsManager.keyBindings[(int)KeyBind.JetUp].ToString() + " & " + SettingsManager.keyBindings[(int)KeyBind.JetDown].ToString() + " to boost up & down.\nPress "
			+SettingsManager.keyBindings[(int)KeyBind.StopMovement].ToString()+" to brake.");
        UIPlayerHUD.TutorialPrompt("\nPress "+SettingsManager.keyBindings[(int)KeyBind.JetUp].ToString()+" to boost up.\n\nPress "+SettingsManager.keyBindings[(int)KeyBind.JetDown].ToString()
		+" to boost down.\n\nPress "+SettingsManager.keyBindings[(int)KeyBind.StopMovement].ToString()+" to brake.\n\n\n\n\nPress Space to continue.");
        yield return new WaitForSeconds(10);
        step5 = true;
    }
    IEnumerator FlightTutorial2() {
        check6 = true;
        ChatManager.TutorialChat("\nPress "
            + SettingsManager.keyBindings[(int)KeyBind.RollLeft].ToString() + " & "
            + SettingsManager.keyBindings[(int)KeyBind.RollRight].ToString() + " to roll to the left & right.\n\nYou can also rotate by moving the mouse while floating.");
		UIPlayerHUD.TutorialPrompt("\nPress "
            + SettingsManager.keyBindings[(int)KeyBind.RollLeft].ToString() + " to roll to the left and "
            + SettingsManager.keyBindings[(int)KeyBind.RollRight].ToString() + " to roll to the right.\n\n"
			+ "You can also rotate by moving the mouse while floating.\n\n\n\n\nPress Space to continue.");
        yield return new WaitForSeconds(5);
        step6 = true;
    }

    IEnumerator NavigationTutorial() {
        platform1 = GameObject.Find("LandingPlatform1");
        platform2 = GameObject.Find("LandingPlatform2");
        platform3 = GameObject.Find("LandingPlatform3");
        platform1.transform.position = new Vector3(platform1.transform.position.x, 0, platform1.transform.position.z);
        platform2.transform.position = new Vector3(platform2.transform.position.x, 0, platform2.transform.position.z);
        platform3.transform.position = new Vector3(platform3.transform.position.x, 0, platform3.transform.position.z);
        ChatManager.TutorialChat("\nTry to land on the green platforms.\n\nRemember to 'look up' before you crash so you land feet first.");
        UIPlayerHUD.TutorialPrompt("\nTry to land on the green platforms.\n\nRemember to 'look up' before you crash so you land feet first.\n\n\n\n\nPress Space to continue.");
        yield return new WaitForSeconds(5);
        step7 = true;
    }

    IEnumerator GrenadeTutorial() {
        ChatManager.TutorialChat("\nWell done!\n\nPlease proceed to the next room.");
        yield return new WaitForSeconds(7);
        check8 = true;
        ChatManager.TutorialChat("Purple boxes contain grenades.\nThere are 3 types of grenades: Black Hole, EMP and Frag.\n\nPress "
            + SettingsManager.keyBindings[(int)KeyBind.Grenade].ToString() + " to throw a Proximity Grenade.\nPress " 
			+ SettingsManager.keyBindings[(int)KeyBind.GrenadeSwitch].ToString() 
			+ " to change grenade type.\n\nKeep in mind that without gravity, the grenades will fly in a straight line.");
        UIPlayerHUD.TutorialPrompt("Purple boxes contain grenades.\nThere are 3 types of grenades: Black Hole, EMP and Frag.\n\nPress "
            + SettingsManager.keyBindings[(int)KeyBind.Grenade].ToString() + " to throw a Proximity Grenade.\n\nPress " 
			+ SettingsManager.keyBindings[(int)KeyBind.GrenadeSwitch].ToString() 
			+ " to change grenade type.\n\nKeep in mind that without gravity, the grenades will fly in a straight line.\n\n\n\n\nPress Space to continue.");
        yield return new WaitForSeconds(8);
        step8 = true;
    }

    IEnumerator ItemTutorial() {
        ChatManager.TutorialChat(
            "\nIf a grenade hits a person it will detonate instantly.\n\nYou can also set off grenades by shooting them.");
        yield return new WaitForSeconds(8);
        healthpack = GameObject.Find("TutorialBonusHealthPack");
        healthpack.transform.position = new Vector3(142.22f, 13.45f, -0.17f);
        ChatManager.TutorialChat("\nThis green box is a Medkit, it will restore damage to your suit.\n\nThey can be picked up by touching them."
			+ "\nYou can also shoot them to destroy them and stop others from using them.");
        UIPlayerHUD.TutorialPrompt("\nThis green box is a Medkit, it will restore damage to your suit.\n\nThey can be picked up by touching them."
			+ "\n\nYou can also shoot them to destroy them and stop others from using them.\n\n\n\n\nPress Space to continue.");
        yield return new WaitForSeconds(10);
        StartCoroutine(FinalTutorial());
    }

    IEnumerator FinalTutorial() {
        ChatManager.TutorialChat("\nKeep in mind that this suit uses air as fuel.\n\nIf you run low on air the suit will automatically disable boosting temporarily while it generates more.");
        yield return new WaitForSeconds(30);
        ChatManager.TutorialChat("\nAt the bottom left of your HUD the suit displays a radar.\n\nThis shows you the locations of items and other players.");
        yield return new WaitForSeconds(30);
        ChatManager.TutorialChat("The green dot represents you and the triangle at the top is your field of view.\nPlayers or items within the triange are in front of you, when items or players are above you their dot is larger than yours, when they are below you their dot is smaller.");
        yield return new WaitForSeconds(30);
        ChatManager.TutorialChat("At the bottom right of your HUD the suit displays important information including:\nmine count, ammo count, remaining air and suit structural integrity.\nIf the structural integrity of the suit is compromised you will lose both pressurization and air supply, resulting in death.");
        yield return new WaitForSeconds(60);
        ChatManager.TutorialChat("\nWhen you're ready to end the simulation, press Escape.");
    }
}