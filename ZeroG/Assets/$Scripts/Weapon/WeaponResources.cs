using UnityEngine;
using System.Collections;

public class WeaponResources : MonoBehaviour {
    
    public static bool isReloading = false;

    public AudioClip soundOverheat;
    AudioSource audioSource;

    // Set in editor
    public int maxHeat;
    public float coolRate;
    public float overheatWaitTime;

    //private float heat;
    //private float coolTime;

    private ParticleSystem smoke;

    private WeaponReloadRotation weaponReloadRotation;
    private WeaponInventory inventory;

	// Use this for initialization
	void Awake () {
        weaponReloadRotation = GetComponentInChildren<WeaponReloadRotation>();
        inventory = GetComponent<WeaponInventory>();
        audioSource = GetComponent<AudioSource>();
        smoke = GetComponentInChildren<ParticleSystem>();
        smoke.emissionRate = 0;
        smoke.Play();

        if (!GetComponent<NetworkView>().isMine) {
            this.enabled = false;
        }
	}

	// Update is called once per frame
	void Update () {
        if (InputConverter.GetKeyDown(KeyBind.Reload) && !GameManager.IsPlayerMenu() && !WeaponInventory.isChanging && !isReloading && !inventory.currentWeapon.isFull()) {
            StartCoroutine("WeaponReload");
        }
        CoolWeapon();
        WeaponSmokeCheck();
	}
    void OnGUI() {
        if (DebugManager.IsDebugMode()) {
            GUI.Label(new Rect(Screen.width - 200, 150, 180, 20), "Weapon Heat: " + inventory.currentWeapon.heat.ToString());
            GUI.Label(new Rect(Screen.width - 200, 170, 180, 20), "Cool Time: " + inventory.currentWeapon.coolTime.ToString());
            GUI.Label(new Rect(Screen.width - 200, 190, 180, 20), "Time: " + Time.time.ToString());
        }
    }

    private void CoolWeapon() {
        foreach (Weapon weapon in GameManager.weapon) {
            weapon.Cool();
        }
    }
    public bool WeaponCanFire() {
        return !( inventory.currentWeapon.isEmpty() || isWeaponOverheated() || isReloading);
    }

    public bool isWeaponOverheated() {
        return inventory.currentWeapon.heat >= maxHeat;
    }

    public void WeaponFired(float addedHeat) {
        StopCoroutine("WeaponReload");
        audioSource.Stop();
        inventory.currentWeapon.Fire();
        if (isWeaponOverheated()) {
            //Gun overheated
            audioSource.PlayOneShot(soundOverheat);
            inventory.currentWeapon.heat = maxHeat;

            //When overheating, delay cooling
            inventory.currentWeapon.coolTime += overheatWaitTime;
        }
        if (inventory.currentWeapon.currentClip <= 0) {
            StartCoroutine("WeaponReload");
        }
    }
    private void WeaponSmokeCheck() {
        if (isWeaponOverheated()) {
            smoke.emissionRate = 15;
            smoke.startColor = Color.black;
        } else if (inventory.currentWeapon.heat > maxHeat * 2 / 3) {
            smoke.emissionRate = 15;
            smoke.startColor = Color.grey;
        } else if (inventory.currentWeapon.heat > maxHeat * 1 / 3) {
            smoke.emissionRate = 10;
            smoke.startColor = Color.white;
        } else {
            smoke.emissionRate = 0;
        }
    }
    IEnumerator WeaponReload() {

        // If no remaining ammo and NOT testmode, don't attempt to reload
        if (inventory.currentWeapon.remainingAmmo <= 0 && !DebugManager.IsAllAmmo()) {
            if (inventory.currentWeapon.currentClip <= 0) {
                // Remove weapon?
            }

            yield break;
        }

        isReloading = true;
        if (inventory.currentWeapon.reloadTime > 0.5f) {
            weaponReloadRotation.ReloadRotation(inventory.currentWeapon.reloadTime);
        }
        audioSource.clip = inventory.currentWeapon.reloadSound;
        audioSource.Play();
        float wait = 0;
        while (wait < inventory.currentWeapon.reloadTime) {
            wait += Time.deltaTime;
            yield return null;
        }
        int newBullets = inventory.currentWeapon.clipSize - inventory.currentWeapon.currentClip;
        if (DebugManager.IsAllAmmo()) {
            inventory.currentWeapon.currentClip = inventory.currentWeapon.clipSize;
        } else {
            inventory.currentWeapon.currentClip = Mathf.Min(inventory.currentWeapon.clipSize, inventory.currentWeapon.remainingAmmo + inventory.currentWeapon.currentClip);
        }

        inventory.currentWeapon.remainingAmmo -= newBullets;
        inventory.currentWeapon.remainingAmmo = Mathf.Max(inventory.currentWeapon.remainingAmmo, 0);
        isReloading = false;
    }
    public void SafeStartReload() {
        if (!WeaponInventory.isChanging && !isReloading && !inventory.currentWeapon.isFull()) {
            StartCoroutine("WeaponReload");
        }
    }

    void WeaponChanged() {
        // Clear heat and stop reload
        StopCoroutine("WeaponReload");
        isReloading = false;
    }
}
