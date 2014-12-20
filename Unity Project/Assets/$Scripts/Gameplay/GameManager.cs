using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	// *****************Test Mode Variable************************
	public static bool testMode = true;
	// ***********************************************************
	public static bool paused;


	private MouseLook cameraLook;
	private CameraMove cameraMove;
	private FireWeapon fireWeapon;
	private PlayerResources playerResources;

	public static int maxStartingWeapons = 2;
	private int[] startingWeapons = new int[maxStartingWeapons];

	private bool myPlayerSpawned = false;

	public GameObject playerPrefab;
	private GameObject[] spawnPoints;

	public enum KeyBind { MoveForward, MoveBack, MoveLeft, MoveRight, RollLeft, RollRight, JetUp, JetDown, Reload, Grenade, Interact, GrenadeSwitch};
	public static KeyCode[] keyBindings;
	
	public static List<WeaponSuperClass> weapon = new List<WeaponSuperClass>();

	private GUIScript gameGUI;

	public string playerCurrentName;



	void Awake(){
		DontDestroyOnLoad(gameObject);

		gameGUI = GetComponent<GUIScript>();

		keyBindings = new KeyCode[System.Enum.GetNames(typeof(GameManager.KeyBind)).Length];

		keyBindings[(int)GameManager.KeyBind.MoveForward]	= (KeyCode)PlayerPrefs.GetInt("bindMoveForward", (int)KeyCode.W);
		keyBindings[(int)GameManager.KeyBind.MoveBack] 		= (KeyCode)PlayerPrefs.GetInt("bindMoveBack", (int)KeyCode.S);
		keyBindings[(int)GameManager.KeyBind.MoveLeft] 		= (KeyCode)PlayerPrefs.GetInt("bindMoveLeft", (int)KeyCode.A);
		keyBindings[(int)GameManager.KeyBind.MoveRight] 	= (KeyCode)PlayerPrefs.GetInt("bindMoveRight", (int)KeyCode.D);
		
		keyBindings[(int)GameManager.KeyBind.RollLeft]		= (KeyCode)PlayerPrefs.GetInt("bindRollLeft", (int)KeyCode.Q);
		keyBindings[(int)GameManager.KeyBind.RollRight] 	= (KeyCode)PlayerPrefs.GetInt("bindRollRight", (int)KeyCode.E);
		keyBindings[(int)GameManager.KeyBind.JetUp]			= (KeyCode)PlayerPrefs.GetInt("bindJetUp", (int)KeyCode.Space);
		keyBindings[(int)GameManager.KeyBind.JetDown] 		= (KeyCode)PlayerPrefs.GetInt("bindJetDown", (int)KeyCode.X);
		
		keyBindings[(int)GameManager.KeyBind.Reload] 		= (KeyCode)PlayerPrefs.GetInt("bindReload", (int)KeyCode.R);
		keyBindings[(int)GameManager.KeyBind.Grenade] 		= (KeyCode)PlayerPrefs.GetInt("bindGrenade", (int)KeyCode.G);
		keyBindings[(int)GameManager.KeyBind.Interact] 		= (KeyCode)PlayerPrefs.GetInt("bindInteract", (int)KeyCode.F);
		keyBindings[(int)GameManager.KeyBind.GrenadeSwitch]	= (KeyCode)PlayerPrefs.GetInt("bindGrenadeSwitch", (int)KeyCode.H);

		// Add weapons to list
		weapon.Add(new LaserRifleValues());
		weapon.Add(new SlugRifleValues());
		weapon.Add(new LaserSniperValues());
		weapon.Add(new ShotgunValues());
		weapon.Add(new ForceShotgunValues());
		weapon.Add(new RocketLauncherValues());
		weapon.Add(new PlasmaBlasterValues());

	}

	public void AddToChat(string input, bool addPlayerPrefix = true){
		gameGUI.SubmitTextToChat(input, addPlayerPrefix);
	}

	public static int WeaponClassToWeaponId(WeaponSuperClass input){
		for(int i=0; i<weapon.Count; i++){
			if(input == weapon[i]){
				return i;
			}
		}
		return -1;
	}

	void OnLevelWasLoaded(int level){
		CursorVisible(true);
		if(!GameManager.SceneIsMenu()){
			spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
			cameraMove = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMove>();

			PlayerDied(); //Died before you begin? Don't worry, it's just cleanup

			Pause (false);

			//
			if(Network.isServer){
				startingWeapons[0] = PlayerPrefs.GetInt("1stWeapon", 0);
				startingWeapons[1] = PlayerPrefs.GetInt("2ndWeapon", 7);
				networkView.RPC ("SetStartingWeapons", RPCMode.OthersBuffered, startingWeapons);
			}
		}
	}

	[RPC]
	private void SetStartingWeapons(int[] selection){
		startingWeapons = selection;
	}
	public int[] GetStartingWeapons(){
		return startingWeapons;
	}


	public static bool SceneIsMenu(){
		return Application.loadedLevelName == "MenuScene";
	}
	public static bool IsPaused(){
		return paused;
	}
	public bool IsPlayerSpawned(){
		return myPlayerSpawned;
	}
	public void Spawn(){
		myPlayerSpawned = true;

		CursorVisible(false);
		int point = Random.Range(0, spawnPoints.Length);
		Network.Instantiate(playerPrefab, spawnPoints[point].transform.position, spawnPoints[point].transform.rotation, 0);
		cameraMove.Spawn();

		GameObject[] list = GameObject.FindGameObjectsWithTag("Player");
		foreach(GameObject player in list){
			if(player.networkView.isMine){
				cameraLook = player.transform.FindChild("CameraPos").GetComponent<MouseLook>();
				cameraLook.SetYDirection(PlayerPrefs.GetInt("mouseYDirection"));

				fireWeapon = player.GetComponent<FireWeapon>();
				playerResources = player.GetComponent<PlayerResources>();
			}
		}

		//Reload all weapons
		for(int i=0; i< weapon.Count; i++){
			weapon[i].currentClip = weapon[i].clipSize;
			weapon[i].remainingAmmo = weapon[i].defaultRemainingAmmo;
		}

	}

	public void PlayerDied(){
		myPlayerSpawned = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.F1)){
			testMode = !testMode;
		}
		if(!GameManager.SceneIsMenu()){
			if(Input.GetKeyDown(KeyCode.Escape)){
				CursorVisible(!paused);
				Pause (!paused); // Toggle Pause
			}
			if(Input.GetKeyDown(KeyCode.Alpha1) && myPlayerSpawned){
				fireWeapon.ChangeWeapon(0);
			}
			if(Input.GetKeyDown(KeyCode.Alpha2) && myPlayerSpawned){
				fireWeapon.ChangeWeapon(1);
			}
			if(Input.GetKeyDown(KeyCode.Alpha3) && myPlayerSpawned){
				fireWeapon.ChangeWeapon(2);
			}
			if(Input.GetKeyDown(KeyCode.Alpha4) && myPlayerSpawned){
				fireWeapon.ChangeWeapon(3);
			}
			if(Input.GetKeyDown(KeyCode.Alpha5) && myPlayerSpawned){
				fireWeapon.ChangeWeapon(4);
			}
			if(Input.GetKeyDown(KeyCode.Alpha6) && myPlayerSpawned){
				fireWeapon.ChangeWeapon(5);
			}
			if(Input.GetKeyDown(KeyCode.Alpha7) && myPlayerSpawned){
				fireWeapon.ChangeWeapon(6);
			}
			if(Input.GetKeyDown(GameManager.keyBindings[(int)GameManager.KeyBind.GrenadeSwitch])){
				Debug.Log ("Grenade Switch!");
				playerResources.ChangeGrenade();
			}
		}
	}
	public void Pause(bool input){
		paused = input;
	}
	public void CursorVisible(bool visible){
		Screen.showCursor = visible;
		Screen.lockCursor = !visible;
	}
	public void ManagerDetachCamera(){
		cameraMove.DetachCamera();
	}

	void OnApplicationQuit(){
		if(Network.isClient || Network.isServer){
			Network.Disconnect();
		}
	}

	void OnPlayerDisconnected(NetworkPlayer player){

		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}
}
