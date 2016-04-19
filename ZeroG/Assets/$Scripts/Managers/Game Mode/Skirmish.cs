using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace GameMode {
    public class Skirmish : MonoBehaviour, IGameMode {

        public void Kill(NetworkIdentity killer, NetworkIdentity corpse) {
            //ScoreVictoryManager.singleton.PointScored(killer, 2);
        }

        public void Suicide(NetworkIdentity player) {
            //ScoreVictoryManager.singleton.PointLost(player);
        }

        public void PlayerDied(NetworkIdentity player) {
            //ScoreVictoryManager.singleton.PointLost(player);
        }

        public void ObjectiveScored(NetworkIdentity player) {
            // Do Nothing
        }
    }
}

