﻿using UnityEngine;
using System.Collections;

public class ActorStats : MonoBehaviour, IActorStats {


    public float speed {
        get;
        set;
    }

    public int maxHealth {
        get;
        set;
    }

    public float fuelSpend {
        get;
        set;
    }

    void Awake() {
        // Default values
        maxHealth = 100;
        fuelSpend = 0.5f;
    }
}
