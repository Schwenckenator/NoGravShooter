using UnityEngine;
using System.Collections;

public class LaserSniper : Weapon {
    public static Weapon instance;

    private static bool isAiming;
    public override void Init() {
        instance = this;
        isAiming = false;
        update += Cool;
        update += IncreaseSpread;
        shotSpread = maxSpread;
    }

    public override void Aim(bool aiming) {
        if (aiming == isAiming) return;
        // Only change delegates if change is needed
        if (aiming) {
            update -= IncreaseSpread;
            update += ReduceSpread;
        } else {
            update += IncreaseSpread;
            update -= ReduceSpread;
        }
        isAiming = aiming;
    }
}
