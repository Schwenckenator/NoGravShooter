using UnityEngine;
using System.Collections;

public class ForceCannon : Weapon {
    public static Weapon instance;
    public override void Init() {
        instance = this;
    }
}
