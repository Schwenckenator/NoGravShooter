using UnityEngine;
using System.Collections;

public class Shotgun : Weapon {
    public static Weapon instance;
    public override void Init() {
        instance = this;
        update = ReduceSpread;
    }
}
