using UnityEngine;

using System.Collections;
using UnityEngine.Networking;

[RequireComponent(typeof(ObjectCleanUp))]
public class BonusWeaponPickup : NetworkBehaviour {
    [SyncVar]
    public int id;

    public bool randomid;

    private GameObject currentWeaponModel;
    private WeaponInventory inventory;
    private int weaponcount;
    private int maxweaponcount;
    private int currentInventorySlot;
    private bool playerColliding = false;
    private bool hasAmmo = true;

    public override void OnStartServer() {
        base.OnStartServer();
        if (DebugManager.allWeapon) {
			maxweaponcount = 99;
		}
		if(randomid){
            id = Random.Range(0, WeaponManager.weapon.Count);
		}
        UpdateModel();
	}


	void UpdateModel(){
		if(currentWeaponModel != null){
            RpcDeleteOldModel();
        }
        RpcCreateNewModel();
    }

    [ClientRpc]
    void RpcDeleteOldModel(){
		Destroy(transform.GetChild(0).gameObject);
		currentWeaponModel = null;
	}
    [ClientRpc]
    void RpcCreateNewModel(){
		currentWeaponModel = Instantiate(WeaponManager.weapon[id].model, transform.position, transform.rotation) as GameObject;
		currentWeaponModel.transform.parent = transform;
	}
	
	void OnTriggerEnter(Collider info){
		if(info.CompareTag("Player")){
            if (info.GetComponent<LobbyPlayer>().isLocalPlayer) {
                playerColliding = true;
                inventory = info.GetComponent<WeaponInventory>();
                weaponcount = inventory.NumWeaponsHeld();
                maxweaponcount = WeaponManager.GetMaxHeldWeapons();
                currentInventorySlot = inventory.currentInventorySlot;
            }
        }
    }
	
	void OnTriggerExit(Collider info){
		if(info.CompareTag("Player")){
            if (info.GetComponent<LobbyPlayer>().isLocalPlayer) {
                playerColliding = false;
            }
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
        UIPlayerHUD.Prompt(InputKey.GetKeyName(KeyBind.Interact) + " - Swap Weapons");

        if (InputKey.GetKeyDown(KeyBind.Interact)) {
            SwapWeapon();
        }
    }

    private void SwapWeapon() { // This will break
        for (int i = 0; i < 7; i++) {
            if (inventory.IsCurrentWeapon(i)) {
                
                inventory.RemoveWeapon(WeaponManager.weapon[i]);
                inventory.AddWeapon(id, inventory.currentInventorySlot);
                inventory.ChangeWeapon(currentInventorySlot, true);

                id = i;
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
        hasAmmo = false;
        NetworkServer.Destroy(gameObject);
    }

    private void AddAmmo() {
        if (!hasAmmo) return;
		if(WeaponManager.weapon[id].isEnergy){
			WeaponManager.weapon[id].currentClip += (WeaponManager.weapon[id].defaultRemainingAmmo);
		} else {
			WeaponManager.weapon[id].remainingAmmo += (WeaponManager.weapon[id].clipSize + WeaponManager.weapon[id].defaultRemainingAmmo);
		}
        Debug.Log("Already own, adding ammo");
        hasAmmo = false;
        NetworkServer.Destroy(gameObject);
    }
}
