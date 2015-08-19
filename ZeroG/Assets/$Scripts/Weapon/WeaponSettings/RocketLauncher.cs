using UnityEngine;
using System.Collections;

public class RocketLauncher : Weapon {
    public static Weapon instance;
    public override void Init() {
        instance = this;
        update = PlayWithBalls;
    }
}
