using UnityEngine;
using System.Collections;

public interface IGameMode{
    void Kill(LobbyPlayer killer, LobbyPlayer corpse);
    void Suicide(LobbyPlayer player);
    void PlayerDied(LobbyPlayer player);
    void ObjectiveScored(LobbyPlayer player);
}
