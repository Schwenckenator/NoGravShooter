using UnityEngine;
using System.Collections;

public interface IGameMode{
    void KillScored(Player killer);
    void FriendlyKill(Player killer);
    void PlayerDied(Player player);
    void ObjectiveScored(Player player);
}
