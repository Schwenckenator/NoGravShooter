using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The currently held weapons
/// </summary>
public class WeaponInventory : MonoBehaviour {
    public int maxHeldWeapons = 2;
    public Weapon currentWeapon { get; set; }
    public int currentInventorySlot {get; private set;}
    public AudioClip soundChangeWeapon;

    public static bool isChanging = false;
    
    private bool initialised = false;
    private List<Weapon> heldWeapons;
    private WeaponReloadRotation weaponReloadRotation;

    private AudioSource audioSource;

    //NetworkView //NetworkView;
	// Use this for initialization
	void Awake () {
        //NetworkView = GetComponent<//NetworkView>();
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
        //if (NetworkView.isMine) {
            ChangeWeapon(0);
        //}
    }
    void Start() {
        //if (NetworkView.isMine) {
            ChangeWeapon(0);
        //} else {
        //    this.enabled = false;
        //    return;
        //}
        initialised = true;
    }

    private void SetWeaponLoadout() {

        currentWeapon = null;
        heldWeapons.Clear();

        if (GameManager.IsSceneTutorial()) return; // No weapons for tutorial
        int[] temp = GameManager.instance.GetStartingWeapons();

        foreach (int id in temp) {
            //Debug.Log(id.ToString());
            if (id >= 0 && id < GameManager.weapon.Count) {
                AddWeapon(id);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (GameManager.IsPlayerMenu() || HasNoWeapons()) return;
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
        if (DebugManager.IsAllWeapon()) {
            return GameManager.weapon.Count;
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
        return heldWeapons.Contains(GameManager.weapon[weaponId]);
    }

    public bool IsCurrentWeapon(int weaponId) {
        return currentWeapon == GameManager.weapon[weaponId];
    }

    public void ChangeWeapon(int weaponId, bool force = false) {
        if (currentInventorySlot == weaponId && !force) { return; } // If you're already here, do nothing

        StopCoroutine("WeaponChange");
        if (DebugManager.IsAllWeapon()) {
            if (weaponId < GameManager.weapon.Count) {
                currentInventorySlot = weaponId;
                currentWeapon = GameManager.weapon[weaponId];
                StartCoroutine("WeaponChange");
            }
        } else {
            if (weaponId < heldWeapons.Count) {
                currentInventorySlot = weaponId;
                currentWeapon = heldWeapons[weaponId];
                StartCoroutine("WeaponChange");
            }
        }
    }

    public void AddWeapon(int weaponID, int index) {
        bool hadNoWeapons = HasNoWeapons();
        heldWeapons.Insert(index, GameManager.weapon[weaponID]);

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
        weaponReloadRotation.ReloadRotation(waitTime, GameManager.weapon[currentWeaponID]);
    }
}
