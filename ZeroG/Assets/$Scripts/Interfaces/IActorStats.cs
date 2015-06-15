using UnityEngine;
using System.Collections;

public interface IActorStats {

    float speed { get; set; }
    int maxHealth { get; set; }
    float fuelSpend { get; set; }
}
