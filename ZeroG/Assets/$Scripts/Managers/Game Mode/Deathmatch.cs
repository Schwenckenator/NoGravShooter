using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace GameMode {
    public class Deathmatch : MonoBehaviour, IGameMode {

        public void Kill(NetworkIdentity killer, NetworkIdentity corpse) {
            Debug.Log("Kill scored.");
            //ScoreVictoryManager.singleton.PointScored(killer);
        }

        public void Suicide(NetworkIdentity player) {
            Debug.Log("Suicide.");
            //ScoreVictoryManager.singleton.PointLost(player);
        }

        public void PlayerDied(NetworkIdentity player) {
            // Do Nothing
        }

        public void ObjectiveScored(NetworkIdentity player) {
            // Do Nothing
        }
    }
}

