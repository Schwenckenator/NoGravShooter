using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

/// <summary>
/// The currently held weapons
/// </summary>
public class WeaponInventory : NetworkBehaviour {
    public int maxHeldWeapons = 2;
    public Weapon currentWeapon { get; set; }
    public int currentInventorySlot {get; private set;}
    public AudioClip soundChangeWeapon;

    public static bool isChanging = false;
    
    private bool initialised = false;
    private List<Weapon> heldWeapons;
    private WeaponReloadRotation weaponReloadRotation;

    private AudioSource audioSource;

	// Use this for initialization
	void Awake () {
        heldWeapons = new List<Weapon>();
        weaponReloadRotation = GetComponentInChildren<WeaponReloadRotation>();
        audioSource = GetComponent<AudioSource>();

        SetWeaponLoadout();
        currentInventorySlot = -1; // Bad value, will change
        currentWeapon = null;

    }
    public void Reset() {
        SetWeaponLoadout();
        currentInventorySlot = -1; // Bad value, will change
        currentWeapon = null;
        if (isLocalPlayer) {
            ChangeWeapon(0, true);
        }
    }
    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();
        ChangeWeapon(0, true);
        initialised = true;
    }

    private void SetWeaponLoadout() {

        currentWeapon = null;
        heldWeapons.Clear();

        if (GameManager.IsTutorial()) return; // No weapons for tutorial
        SyncListInt IDs = WeaponManager.singleton.GetStartingWeapons();

        for(int i=0; i<IDs.Count; i++) { 
            //Debug.Log(id.ToString());
            if (IDs[i] >= 0 && IDs[i] < WeaponManager.weapon.Count) {
                AddWeapon(IDs[i]);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (UIPauseSpawn.IsShown || HasNoWeapons()) return;
        GetKeyStrokes();
        MouseWheelWeaponChange();

	}
    private void GetKeyStrokes() {
		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			ChangeWeapon(0);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2)) {
			ChangeWeapon(1);
		}
		if (Input.GetKeyDown(KeyCode.Alpha3)) {
			ChangeWeapon(2);
		}
		if (Input.GetKeyDown(KeyCode.Alpha4)) {
			ChangeWeapon(3);
		}
		if (Input.GetKeyDown(KeyCode.Alpha5)) {
			ChangeWeapon(4);
		}
		if (Input.GetKeyDown(KeyCode.Alpha6)) {
			ChangeWeapon(5);
		}
		if (Input.GetKeyDown(KeyCode.Alpha7)) {
			ChangeWeapon(6);
		}
		if (Input.GetKeyDown(KeyCode.Alpha8)) {
			ChangeWeapon(7);
		}
    }
    private void MouseWheelWeaponChange() {
        //change weapons by mouse wheel
        //checks if player has max number of weapons
        int newSlot = currentInventorySlot;
        if (Input.GetAxis("Mouse ScrollWheel") < 0) {
            newSlot++;
            newSlot %= GetMaxWeapon();
            ChangeWeapon(newSlot);

        } else if (Input.GetAxis("Mouse ScrollWheel") > 0) {
            newSlot--;
            if (newSlot < 0) newSlot += GetMaxWeapon();
            ChangeWeapon(newSlot);
        }
    }
    int GetMaxWeapon() {
        if (DebugManager.allWeapon) {
            return WeaponManager.weapon.Count;
        } else {
            return heldWeapons.Count;
        }
    }

    public bool HasNoWeapons() {
        return heldWeapons.Count == 0 && initialised;
    }
    public int NumWeaponsHeld() {
        return heldWeapons.Count;
    }
    public bool IsWeaponHeld(int weaponId) {
        return heldWeapons.Contains(WeaponManager.weapon[weaponId]);
    }

    public bool IsCurrentWeapon(int weaponId) {
        return currentWeapon == WeaponManager.weapon[weaponId];
    }

    public void ChangeWeapon(int weaponId, bool force = false) {
        if (currentInventorySlot == weaponId && !force) { return; } // If you're already here, do nothing

        StopCoroutine("WeaponChange");
        int max = DebugManager.allWeapon ? WeaponManager.weapon.Count : heldWeapons.Count;
        List<Weapon> list = DebugManager.allWeapon ? WeaponManager.weapon : heldWeapons;

        if (weaponId < max) {
            currentInventorySlot = weaponId;
            currentWeapon = list[weaponId];
            StartCoroutine("WeaponChange");
        }
    }

    public void AddWeapon(int weaponID, int index) {
        bool hadNoWeapons = HasNoWeapons();
        heldWeapons.Insert(index, WeaponManager.weapon[weaponID]);

        if (hadNoWeapons) {
            ChangeWeapon(0, true);
        }
    }
    public void AddWeapon(int weaponID) {
        AddWeapon(weaponID, heldWeapons.Count);
    }
    public void RemoveWeapon(Weapon item) {
        heldWeapons.Remove(item);
    }

    IEnumerator WeaponChange() {
        isChanging = true;
        gameObject.SendMessage("WeaponChanged");
        audioSource.Stop();
        audioSource.clip = soundChangeWeapon;
        audioSource.Play();
        float waitTime = 1.0f;
        weaponReloadRotation.ReloadRotation(waitTime, currentWeapon);
        //NetworkView.RPC("NetworkWeaponChange", RPCMode.Others, waitTime, currentWeapon.id);
        yield return new WaitForSeconds(waitTime);
        isChanging = false;
    }
    //[RPC]
    void NetworkWeaponChange(float waitTime, int currentWeaponID) {
        weaponReloadRotation.ReloadRotation(waitTime, WeaponManager.weapon[currentWeaponID]);
    }
}
