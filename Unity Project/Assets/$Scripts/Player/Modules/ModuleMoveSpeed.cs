using UnityEngine;
using System.Collections;

public class ModuleMoveSpeed : Module {

    private float speedPassive = 1.25f; // Increase by 25%
    private float speedActive = 5f;

    // Dash?
    public override void ModifyActive(IActorStats stats) {
        throw new System.NotImplementedException();
    }

    public override void ModifyPassive(IActorStats stats) {
        stats.speed *= speedPassive;
    }
}
