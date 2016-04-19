using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public interface IDamageable {

    int Health { get; }
    int MaxHealth { get; }
    bool IsFullHealth {get;}

    void TakeDamage(int damage, NetworkIdentity fromPlayer = null, int weaponId = -1);
    void RestoreHealth(int restore);

}
