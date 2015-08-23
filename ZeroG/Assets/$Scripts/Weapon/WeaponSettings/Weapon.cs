using UnityEngine;
using System.Collections;

public enum WeaponType { Ray, Projectile};
public abstract class Weapon: MonoBehaviour {
    private static float coolRate = 30;

    /// 
    /// Weapon Stats
    /// 
    public string name;
    public WeaponType type;
    public bool hasRecoil;
    public bool isEnergy;
    public bool isScoped;

    public int rayNum;
    public int damage;
    public int heatPerShot;
    public int clipSize;
    public int defaultRemainingAmmo;

    public float fireDelay;
    public float recoil;
    public float minSpread;
    public float maxSpread;
    public float spreadPerShot;
    public float spreadChangeRate;
    public float reloadTime;
    public float zoomFactor;

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

    public abstract void Init();

    public bool isFull() {
        return currentClip == clipSize;
    }
    public bool isEmpty() {
        return currentClip <= 0;
    }

    public virtual void Fire() {
        currentClip--;
        
        heat += heatPerShot;

        // Add the fire delay as a cooldown time
        // Sustained fire will quickly heat up your weapon, but taking small breaks will help it cool
        coolTime = Time.time + fireDelay + 0.1f; // Just a little bit extra

        shotSpread += spreadPerShot;
        if (shotSpread > maxSpread) {
            shotSpread = maxSpread;
        }

    }
    public virtual void Aim(bool aiming) {

    }
    //***********************
    //*** Delegate events ***
    //***********************
    protected delegate void OnUpdate();
    protected OnUpdate update;
    void Update() {
        update();
    }

    protected void ReduceSpread() {
        // Reduce if above min
        if (shotSpread > minSpread) {
            shotSpread -= spreadChangeRate * Time.deltaTime;

            // Set to min if overshot
            if (shotSpread < minSpread) {
                shotSpread = minSpread;
            }
        }
    }
    protected void IncreaseSpread() {
        if (shotSpread < maxSpread) {
            shotSpread += spreadChangeRate * Time.deltaTime;

            // Set to min if overshot
            if (shotSpread > maxSpread) {
                shotSpread = maxSpread;
            }
        }
    }
    protected void Cool() {
        if (heat <= 0 || Time.time < coolTime) return;

        heat -= coolRate * Time.deltaTime;

        if (heat < 0) heat = 0;
    }
    protected void PlayWithBalls() {
        // Do nothing
    }
}
