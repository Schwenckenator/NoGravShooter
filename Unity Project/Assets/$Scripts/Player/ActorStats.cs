using UnityEngine;
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

    void Awake() {
        // Default values
        maxHealth = 100;
    }
}
