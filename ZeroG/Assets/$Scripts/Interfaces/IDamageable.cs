using UnityEngine;
using System.Collections;

public interface IDamageable {

    int GetHealth();
    int GetMaxHealth();
    bool IsFullHealth();

    void TakeDamage(int damage, NetworkPlayer from, int weaponId = -1);
    void RestoreHealth(int restore);

}
