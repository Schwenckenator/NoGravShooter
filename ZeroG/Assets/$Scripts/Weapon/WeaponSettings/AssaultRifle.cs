using UnityEngine;
using System.Collections;

public class AssaultRifle : Weapon {
    public float spreadPerShot;
    public float spreadReduceRate;

    protected override void AfterFire() {
        shotSpread += spreadPerShot;
        if (shotSpread > maxSpread) {
            shotSpread = maxSpread;
        }
    }

    void Update() {
        // Reduce if above min
        if (shotSpread > minSpread) {
            shotSpread -= spreadReduceRate * Time.deltaTime;
            
            // Set to min if overshot
            if (shotSpread < minSpread) {
                shotSpread = minSpread;
            }
        }
    }
}
