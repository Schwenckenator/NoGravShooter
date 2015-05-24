using UnityEngine;
using System.Collections;

public class WeaponResources : MonoBehaviour {
    public static bool isWeaponBusy = false;
    public AudioClip soundOverheat;

    public int maxHeat = 100;
    public float coolRate = 30;
    public float heatCooldownWaitTime = 3.0f;

    private float heat;

    private ParticleSystem smoke;

    private bool isReloading;


    private WeaponReloadRotation weaponReloadRotation;
    private WeaponInventory inventory;

	// Use this for initialization
	void Awake () {
        weaponReloadRotation = GetComponentInChildren<WeaponReloadRotation>();
        inventory = GetComponent<WeaponInventory>();
        smoke = GetComponentInChildren<ParticleSystem>();
        smoke.emissionRate = 0;
        smoke.Play();
	}

    public float GetWeaponHeat() {
        return heat;
    }

	// Update is called once per frame
	void Update () {
        if (InputConverter.GetKeyDown(KeyBind.Reload) && !isReloading && !isWeaponFull()) {
            StartCoroutine("WeaponReload");
        }
        CoolWeapon(coolRate);
        WeaponSmokeCheck();
	}

    private void CoolWeapon(float coolRate) {
        heat -= coolRate * Time.deltaTime;
        if (heat < 0) {
            heat = 0;
        }
    }
    public bool WeaponCanFire() {
        return !( isWeaponEmpty() || isWeaponOverheated() );
    }

    public bool isWeaponFull() {
        return inventory.currentWeapon.clipSize == inventory.currentWeapon.currentClip;
    }
    public bool isWeaponEmpty() {
        return inventory.currentWeapon.currentClip == 0;
    }
    public bool isWeaponOverheated() {
        return heat > maxHeat;
    }

    public void WeaponFired(float addedHeat) {
        StopCoroutine("WeaponReload");
        audio.Stop();
        heat += addedHeat;
        inventory.currentWeapon.currentClip--;
        if (heat > maxHeat) {
            //Gun overheated
            audio.PlayOneShot(soundOverheat);

            heat = maxHeat + (coolRate * heatCooldownWaitTime);
        }
        if (inventory.currentWeapon.currentClip <= 0) {
            StartCoroutine("WeaponReload");
        }
    }
    private void WeaponSmokeCheck() {
        if (heat > maxHeat) {
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

                //GetComponent<FireWeapon>().removeWeapon(currentWeapon);
                //GetComponent<FireWeapon>().ChangeWeapon(0);
            }

            yield break;
        }

        isReloading = true;
        if (inventory.currentWeapon.reloadTime > 1.0f) {
            weaponReloadRotation.ReloadRotation(inventory.currentWeapon.reloadTime);
        }
        isWeaponBusy = true;
        audio.clip = inventory.currentWeapon.reloadSound;
        audio.Play();
        float wait = 0;
        while (wait < inventory.currentWeapon.reloadTime) {
            wait += Time.deltaTime;
            yield return null;
        }
        isWeaponBusy = false;
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
        if (!isWeaponBusy && !isReloading && !isWeaponFull()) {
            StartCoroutine("WeaponReload");
        }
    }
}
