using UnityEngine;
using System.Collections;

public class SlugPistol : Weapon {
    public static Weapon instance;
    public override void Init() {
        instance = this;
    }
}
