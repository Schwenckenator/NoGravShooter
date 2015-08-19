using UnityEngine;
using System.Collections;

public class LaserRifle : Weapon {
    public static Weapon instance;
    public override void Init() {
        instance = this;
    }
}
