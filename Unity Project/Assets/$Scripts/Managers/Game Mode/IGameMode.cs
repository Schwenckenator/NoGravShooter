using UnityEngine;
using System.Collections;

public interface IGameMode{
    void Kill(Player killer, Player corpse);
    void Suicide(Player player);
    void PlayerDied(Player player);
    void ObjectiveScored(Player player);
}
