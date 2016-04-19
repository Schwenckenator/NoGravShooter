using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public interface IGameMode{
    void Kill(NetworkIdentity killer, NetworkIdentity corpse);
    void Suicide(NetworkIdentity player);
    void PlayerDied(NetworkIdentity player);
    void ObjectiveScored(NetworkIdentity player);
}
