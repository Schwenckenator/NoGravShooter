using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ObjectCleanUp))]
public class BonusWeaponPickup : MonoBehaviour {
	
    private GameObject currentWeaponModel;

    [SerializeField]
	private int id;
    [SerializeField]
	private bool randomid;
	private WeaponInventory inventory;
	private int weaponcount;
	private int maxweaponcount;
	private int currentInventorySlot;
	private bool playerColliding = false;

    private bool hasAmmo = true;

    ////NetworkView //NetworkView;
	void Start(){
        ////NetworkView = GetComponent<//NetworkView>();
        if (DebugManager.IsAllWeapon()) {
			maxweaponcount = 99;
		}
		if(Network.isServer){
			if(randomid){
				////NetworkView.RPC("ChangeId", RPCMode.AllBuffered, Random.Range(0,8));
			} else {
				////NetworkView.RPC("ChangeId", RPCMode.AllBuffered, id);
			}
			UpdateModel();
		}

	}

	//
	void UpdateModel(){
		if(currentWeaponModel != null){
			////NetworkView.RPC("DeleteOldModel", RPCMode.AllBuffered);
		}
		////NetworkView.RPC ("CreateNewModel", RPCMode.AllBuffered);
	}

	////[RPC]
	void DeleteOldModel(){
		Debug.Log (transform.GetChild(0).ToString());
		Destroy(transform.GetChild(0).gameObject);
		currentWeaponModel = null;
	}
	////[RPC]
	void CreateNewModel(){
		currentWeaponModel = Instantiate(GameManager.weapon[id].model, transform.position, transform.rotation) as GameObject;
		currentWeaponModel.transform.parent = transform;
	}

	////[RPC]
	void ChangeId(int newId){
		id = newId;
	}
	
	void OnTriggerEnter(Collider info){
		if(info.CompareTag("Player")){
   //         if (info.GetComponent<//NetworkView>().isMine) {
			//	playerColliding = true;
   //             inventory = info.GetComponent<WeaponInventory>();
   //             weaponcount = inventory.NumWeaponsHeld();
			//	maxweaponcount = GameManager.GetMaxStartingWeapons();
			//	currentInventorySlot = inventory.currentInventorySlot;
			//}
		}
	}
	
	void OnTriggerExit(Collider info){
		if(info.CompareTag("Player")){
   //         if (info.GetComponent<//NetworkView>().isMine) {
			//	playerColliding = false;
			//}
		}
	}
	
	public float weaponSwapCooldown = 2f;
	private float swapTimeout = 2f;
	
    void Update(){
		if(!playerColliding) return;

		if(inventory.IsWeaponHeld(id)){
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
        UIPlayerHUD.Prompt(InputConverter.GetKeyName(KeyBind.Interact) + " - Swap Weapons");

        if (InputConverter.GetKeyDown(KeyBind.Interact)) {
            SwapWeapon();
        }
    }

    private void SwapWeapon() {
        for (int i = 0; i < 7; i++) {
            if (inventory.IsCurrentWeapon(i)) {
                
                inventory.RemoveWeapon(GameManager.weapon[i]);
                inventory.AddWeapon(id, inventory.currentInventorySlot);
                inventory.ChangeWeapon(currentInventorySlot, true);
                
                ////NetworkView.RPC("ChangeId", RPCMode.AllBuffered, i);
                swapTimeout = Time.time + weaponSwapCooldown;

                //Change the weapon box model to the new weapon
                UpdateModel();

                break; // Bail out of loop
            }
        }
    }

    private void AddWeapon() {
        if (!hasAmmo) return;
        inventory.AddWeapon(id);
        if (SettingsManager.singleton.AutoPickup) {
            inventory.ChangeWeapon(weaponcount);
        }
        Debug.Log("Not at maximum weapons, auto picking up");
        ////GetComponent<ObjectCleanUp>().KillMe();
        hasAmmo = false;
    }

    private void AddAmmo() {
        if (!hasAmmo) return;
		if(GameManager.weapon[id].isEnergy){
			GameManager.weapon[id].currentClip += (GameManager.weapon[id].defaultRemainingAmmo);
		} else {
			GameManager.weapon[id].remainingAmmo += (GameManager.weapon[id].clipSize + GameManager.weapon[id].defaultRemainingAmmo);
		}
        Debug.Log("Already own, adding ammo");
        ////GetComponent<ObjectCleanUp>().KillMe();
        hasAmmo = false;
    }
}
