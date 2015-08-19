using UnityEngine;
using System.Collections;

public class PlasmaBlaster : Weapon {
    public static Weapon instance;
    public override void Init() {
        instance = this;
    }
}
