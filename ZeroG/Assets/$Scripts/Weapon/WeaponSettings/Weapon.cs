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
    public float shotSpread;
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

    public bool isFull() {
        return currentClip == clipSize;
    }
    public bool isEmpty() {
        return currentClip <= 0;
    }

}
