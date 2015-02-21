using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ObjectCleanUp))]
public class BonusWeaponPickup : MonoBehaviour {
	[SerializeField]
    private GameObject[] weaponModelsArray;
	
    private GameObject currentWeaponModel;

    [SerializeField]
	private int id;
	private FireWeapon fireWeapon;
	private int weaponcount;
	private int maxweaponcount;
	private int currentInventorySlot;
	private bool playerColliding = false;

    private SettingsManager settingsManager;
    private GuiManager guiManager;

	void Start(){
		if(GameManager.IsAllWeapon()){
			maxweaponcount = 99;
		}
		if(Network.isServer){
			networkView.RPC("ChangeId", RPCMode.AllBuffered, Random.Range(0,7));
			UpdateModel();
		}
        GameObject manager = GameObject.FindGameObjectWithTag("GameController");
        settingsManager = manager.GetComponent<SettingsManager>();
        guiManager = manager.GetComponent<GuiManager>();

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
				fireWeapon = info.GetComponent<FireWeapon>();
				weaponcount = fireWeapon.NumberWeaponsHeld();
				maxweaponcount = GameManager.GetMaxStartingWeapons();
				currentInventorySlot = fireWeapon.CurrentWeaponSlot();
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
	
    void Update(){
		if(!playerColliding) return;

		if(fireWeapon.IsWeaponHeld(id)){
            AddAmmo();
		}else{
			if(weaponcount >= maxweaponcount){
                WeaponSwapCheck();
			}else{
                AddWeapon();
			}
		}
	}

    private void WeaponSwapCheck() {
        if (swapTimeout > Time.time) return;

        swapTimeout = weaponSwapCooldown;
        guiManager.ButtonPrompt("Swap Weapons", (int)SettingsManager.KeyBind.Interact);

        if (InputConverter.GetKeyDown(SettingsManager.KeyBind.Interact)) {
            SwapWeapon();
        }
    }

    private void SwapWeapon() {
        for (int i = 0; i < 7; i++) {
            if (fireWeapon.IsCurrentWeapon(i)) {
                
                fireWeapon.removeWeapon(GameManager.weapon[i]);
                fireWeapon.AddWeapon(id, fireWeapon.CurrentWeaponSlot());
                fireWeapon.ChangeWeapon(currentInventorySlot);
                
                networkView.RPC("ChangeId", RPCMode.AllBuffered, i);
                swapTimeout = Time.time + weaponSwapCooldown;

                //Change the weapon box model to the new weapon
                UpdateModel();

                break; // Bail out of loop
            }
        }
    }

    private void AddWeapon() {
        fireWeapon.AddWeapon(id);
        if (settingsManager.AutoPickup) {
            fireWeapon.ChangeWeapon(weaponcount);
        }
        Debug.Log("Not at maximum weapons, auto picking up");
        GetComponent<ObjectCleanUp>().KillMe();
    }

    private void AddAmmo() {
        GameManager.weapon[id].remainingAmmo += (GameManager.weapon[id].clipSize + GameManager.weapon[id].defaultRemainingAmmo);
        Debug.Log("Already own, adding ammo");
        GetComponent<ObjectCleanUp>().KillMe();
    }
}
