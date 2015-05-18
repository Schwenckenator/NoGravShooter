using UnityEngine;
using System.Collections;

/// <summary>
/// A Module changes stats or gives an ability to an actor
/// </summary>
public abstract class Module {

    public Module() {

    }

    public abstract void ModifyActive(IActorStats stats);
    public abstract void ModifyPassive(IActorStats stats);
}
