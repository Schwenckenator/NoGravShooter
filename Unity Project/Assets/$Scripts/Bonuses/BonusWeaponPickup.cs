using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ObjectCleanUp))]
public class BonusWeaponPickup : MonoBehaviour {
	[SerializeField]
    private GameObject[] weaponModelsArray;
	
    private GameObject currentWeaponModel;

    [SerializeField]
	private int id;
	private FireWeapon weapon;
	private int weaponcount;
	private int maxweaponcount;
	private int currentInventorySlot;
	private bool playerColliding = false;

    private GameManager gameManager;
    private SettingsManager settingsManager;

	void Start(){
		if(GameManager.testMode){
			maxweaponcount = 99;
		}
		if(Network.isServer){
			networkView.RPC("ChangeId", RPCMode.AllBuffered, Random.Range(0,7));
			UpdateModel();
		}
		gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        settingsManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<SettingsManager>();
	}

	//
	void UpdateModel(){
		if(currentWeaponModel != null){
			networkView.RPC("DeleteOldModel", RPCMode.AllBuffered);
		}
		networkView.RPC ("CreateNewModel", RPCMode.AllBuffered);
	}

	[RPC]
	void DeleteOldModel(){
		Debug.Log (transform.GetChild(0).ToString());
		Destroy(transform.GetChild(0).gameObject);
		currentWeaponModel = null;
	}
	[RPC]
	void CreateNewModel(){
		currentWeaponModel = Instantiate(weaponModelsArray[id], transform.position, transform.rotation) as GameObject;
		currentWeaponModel.transform.parent = transform;
	}

	[RPC]
	void ChangeId(int newId){
		id = newId;
	}
	
	void OnTriggerEnter(Collider info){
		if(info.CompareTag("Player")){
			if(info.networkView.isMine){
				playerColliding = true;
				weapon = info.GetComponent<FireWeapon>();
				weaponcount = weapon.NumberWeaponsHeld();
				maxweaponcount = GameManager.GetMaxStartingWeapons();
				currentInventorySlot = weapon.CurrentWeaponSlot();
			}
		}
	}
	
	void OnTriggerExit(Collider info){
		if(info.CompareTag("Player")){
			if(info.networkView.isMine){
				playerColliding = false;
			}
		}
	}
	
	public float weaponSwapCooldown = 2f;
	private float swapTimeout = 2f;
	void FixedUpdate(){
		if(playerColliding){
			if(weapon.IsWeaponHeld(id)){
				GameManager.weapon[id].remainingAmmo += (GameManager.weapon[id].clipSize + GameManager.weapon[id].defaultRemainingAmmo);
				Debug.Log ("Already own, adding ammo");
				GetComponent<ObjectCleanUp>().KillMe();
			}else{
				if(weaponcount >= maxweaponcount){
					if(swapTimeout < Time.time){
						swapTimeout = weaponSwapCooldown;
                        gameManager.GetComponent<GuiManager>().ButtonPrompt("Swap Weapons", (int)SettingsManager.KeyBind.Interact);
                        if (Input.GetKey(SettingsManager.keyBindings[(int)SettingsManager.KeyBind.Interact])) {
							for(int i=0; i < 7; i++){
								if(weapon.IsCurrentWeapon(i)){
									weapon.removeWeapon(GameManager.weapon[i]);
									weapon.AddWeapon(id);
									weapon.ChangeWeapon(currentInventorySlot);
									networkView.RPC("ChangeId", RPCMode.AllBuffered, i);
									swapTimeout = Time.time + weaponSwapCooldown;
									i = 99;//lol

									//Change the weapon box model to the new weapon
									UpdateModel();
								}
							}
						}
					}
				}else{
					weapon.AddWeapon(id);
					if(settingsManager.AutoPickup == 1){
						weapon.ChangeWeapon(weaponcount);
					}
					Debug.Log ("Not at maximum weapons, auto picking up");
					GetComponent<ObjectCleanUp>().KillMe();
				}
			}
		}
	}
}
