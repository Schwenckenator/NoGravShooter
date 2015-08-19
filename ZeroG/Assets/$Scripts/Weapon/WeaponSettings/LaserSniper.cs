using UnityEngine;
using System.Collections;

public class LaserSniper : Weapon {
    public static Weapon instance;
    public override void Init() {
        instance = this;
        update = Cool;
    }
}
