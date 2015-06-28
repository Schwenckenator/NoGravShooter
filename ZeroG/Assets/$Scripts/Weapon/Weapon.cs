using UnityEngine;
using System.Collections;

public abstract class Weapon {
    /// 
    /// Weapon Stats
    /// 
    public bool useRay { get; private set; }
    public bool hasRecoil { get; private set; }
    public bool isEnergy { get; private set; }

    public int rayNum { get; private set; }
    public int damage { get; private set; }
    public int heatPerShot { get; private set; }
    public int clipSize { get; private set; }
    
    public int remainingAmmo { get; private set; }
    public int defaultRemainingAmmo { get; private set; }

    public float fireDelay { get; private set; }
    public float recoil { get; private set; }
    public float reloadTime { get; private set; }

    public float heat { get; private set; }
    public int currentClip { get; private set; }

    public GameObject projectile { get; private set; }
    public GameObject hitParticle { get; private set; }
    public AudioClip fireSound { get; private set; }
    public AudioClip reloadSound { get; private set; }

    /// <summary>
    /// Attempts to fire the weapon
    /// </summary>
    /// <returns>True if successful</returns>
    public virtual bool FireWeapon() {
        return false;
    }
    /// <summary>
    /// Attempts to reload
    /// </summary>
    /// <returns>True if successful</returns>
    public virtual bool Reload() {
        return false;
    }
}
