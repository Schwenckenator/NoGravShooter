﻿using UnityEngine;
using System.Collections;

public class AssaultRifle : Weapon {
    public static Weapon instance;
    public override void Init() {
        instance = this;
    }
}
