using UnityEngine;
using System.Collections;

public class WeaponResources : MonoBehaviour {
    
    public static bool isReloading = false;

    public AudioClip soundOverheat;

    // Set in editor
    public int maxHeat;
    public float coolRate;
    public float overheatWaitTime;

    private float heat;
    private float coolTime;

    private ParticleSystem smoke;

    private WeaponReloadRotation weaponReloadRotation;
    private WeaponInventory inventory;

	// Use this for initialization
	void Awake () {
        weaponReloadRotation = GetComponentInChildren<WeaponReloadRotation>();
        inventory = GetComponent<WeaponInventory>();
        smoke = GetComponentInChildren<ParticleSystem>();
        smoke.emissionRate = 0;
        smoke.Play();

        if (!GetComponent<NetworkView>().isMine) {
            this.enabled = false;
        }
	}

    public float GetWeaponHeat() {
        return heat;
    }

	// Update is called once per frame
	void Update () {
        if (InputConverter.GetKeyDown(KeyBind.Reload) && !GameManager.IsPlayerMenu() && !WeaponInventory.isChanging && !isReloading && !isWeaponFull()) {
            StartCoroutine("WeaponReload");
        }
        CoolWeapon();
        WeaponSmokeCheck();
	}
    void OnGUI() {
        if (DebugManager.IsDebugMode()) {
            GUI.Label(new Rect(Screen.width - 200, 150, 180, 20), "Weapon Heat: " + heat.ToString());
            GUI.Label(new Rect(Screen.width - 200, 170, 180, 20), "Cool Time: " + coolTime.ToString());
            GUI.Label(new Rect(Screen.width - 200, 190, 180, 20), "Time: " + Time.time.ToString());
        }
    }

    private void CoolWeapon() {
        if (heat <= 0 || Time.time < coolTime) return;
        heat -= coolRate * Time.deltaTime;
        if (heat < 0) {
            heat = 0;
        }
    }
    public bool WeaponCanFire() {
        return !( isWeaponEmpty() || isWeaponOverheated() || isReloading);
    }

    public bool isWeaponFull() {
        return inventory.currentWeapon.clipSize == inventory.currentWeapon.currentClip;
    }
    public bool isWeaponEmpty() {
        return inventory.currentWeapon.currentClip == 0;
    }
    public bool isWeaponOverheated() {
        return heat >= maxHeat;
    }

    public void WeaponFired(float addedHeat) {
        StopCoroutine("WeaponReload");
        GetComponent<AudioSource>().Stop();
        heat += addedHeat;
        // Add the fire delay as a cooldown time
        // Sustained fire will quickly heat up your weapon, but taking small breaks will help it cool
        coolTime = Time.time + inventory.currentWeapon.fireDelay + 0.1f; // Just a little bit extra
        inventory.currentWeapon.currentClip--;
        if (isWeaponOverheated()) {
            //Gun overheated
            GetComponent<AudioSource>().PlayOneShot(soundOverheat);
            heat = maxHeat;

            //When overheating, delay cooling
            coolTime += overheatWaitTime;
        }
        if (inventory.currentWeapon.currentClip <= 0) {
            StartCoroutine("WeaponReload");
        }
    }
    private void WeaponSmokeCheck() {
        if (isWeaponOverheated()) {
            smoke.emissionRate = 10;
            smoke.startColor = Color.black;
        } else if (heat > maxHeat * 2 / 3) {
            smoke.emissionRate = 10;
            smoke.startColor = Color.grey;
        } else if (heat > maxHeat * 1 / 3) {
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
        if (inventory.currentWeapon.reloadTime > 1.0f) {
            weaponReloadRotation.ReloadRotation(inventory.currentWeapon.reloadTime);
        }
        GetComponent<AudioSource>().clip = inventory.currentWeapon.reloadSound;
        GetComponent<AudioSource>().Play();
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
        if (!WeaponInventory.isChanging && !isReloading && !isWeaponFull()) {
            StartCoroutine("WeaponReload");
        }
    }

    void WeaponChanged() {
        // Clear heat and stop reload
        heat = 0;
        StopCoroutine("WeaponReload");
        isReloading = false;
    }
}
