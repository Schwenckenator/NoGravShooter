using UnityEngine;
using System.Collections;

public enum WeaponType { Ray, Projectile};
public abstract class Weapon: MonoBehaviour {
    /// 
    /// Weapon Stats
    /// 
    public WeaponType type;
    public bool hasRecoil;
    public bool isEnergy;

    public int rayNum;
    public int damage;
    public int heatPerShot;
    public int clipSize;
    public int defaultRemainingAmmo;

    public float fireDelay;
    public float minSpread;
    public float maxSpread;
    public float recoil;
    public float reloadTime;

    public GameObject model;
    public GameObject projectile;
    public GameObject hitParticle;
    public AudioClip fireSound;
    public AudioClip reloadSound;

    // Changeables
    public int id { get; set; }
    public float heat { get; set; }
    public float coolTime { get; set; }
    public int currentClip{ get; set;}
    public int remainingAmmo {get; set;}
    public float shotSpread { get; set; }

    public void ResetVariables() {
        if (isEnergy) {
            currentClip = defaultRemainingAmmo;
            remainingAmmo = 0;
        } else {
            currentClip = clipSize;
            remainingAmmo = defaultRemainingAmmo;
        }
        shotSpread = minSpread;
    }

    public bool isFull() {
        return currentClip == clipSize;
    }
    public bool isEmpty() {
        return currentClip <= 0;
    }

    public void Fire() {
        heat += heatPerShot;

        // Add the fire delay as a cooldown time
        // Sustained fire will quickly heat up your weapon, but taking small breaks will help it cool
        coolTime = Time.time + fireDelay + 0.1f; // Just a little bit extra
        currentClip--;
        AfterFire();
    }
    protected virtual void AfterFire() {

    }
}
