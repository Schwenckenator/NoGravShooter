using UnityEngine;
using System.Collections;

public interface IDamageable {

    int Health { get; }
    int MaxHealth { get; }
    bool IsFullHealth {get;}

    void TakeDamage(int damage, LobbyPlayer fromPlayer = null, int weaponId = -1);
    void RestoreHealth(int restore);

}
