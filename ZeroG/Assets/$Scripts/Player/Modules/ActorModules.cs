using UnityEngine;
using System.Collections;
/// <summary>
/// Container for an actors modules
/// </summary>
public class ActorModules : MonoBehaviour {

    public Module ActiveMod { get; set; }
    public Module PassiveMod { get; set; }

    private IActorStats stats;

    void Start() {
        stats = gameObject.GetInterface<IActorStats>();
    }
    /// <summary>
    /// Should be set after spawning
    /// </summary>
    /// <param name="active"></param>
    /// <param name="passive"></param>
    public void SetModules(Module active, Module passive) {
        this.ActiveMod = active;
        this.PassiveMod = passive;
    }

    private void ApplyModules() {
        ActiveMod.ModifyActive(stats);
        ActiveMod.ModifyPassive(stats);
    }
}
