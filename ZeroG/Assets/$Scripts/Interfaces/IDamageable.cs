using UnityEngine;
using System.Collections;

public interface IDamageable {

    int Health { get; }
    int MaxHealth { get; }
    bool IsFullHealth {get;}

    void TakeDamage(int damage, int weaponId = -1);
    void RestoreHealth(int restore);

}
