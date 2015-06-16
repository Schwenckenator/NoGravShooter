using UnityEngine;
using System.Collections;

/// <summary>
/// Stats that can change via modules go here.
/// </summary>
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
        speed = 10f;
    }
}
