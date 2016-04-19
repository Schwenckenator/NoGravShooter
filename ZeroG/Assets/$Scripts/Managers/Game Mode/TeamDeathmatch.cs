using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace GameMode {
    public class TeamDeathmatch : MonoBehaviour, IGameMode {

        public void Kill(NetworkIdentity killer, NetworkIdentity corpse) {
            // Check if legit kill or friendly fire
            //if (killer.IsOnTeam(corpse.Team)) {
            //    FriendlyKill(killer);
            //} else {
            //    LegitKill(killer);
            //}
        }

        private void LegitKill(NetworkIdentity killer) {
            //ScoreVictoryManager.singleton.PointScored(killer);
        }
        private void FriendlyKill(NetworkIdentity killer) {
            //ScoreVictoryManager.singleton.PointLost(killer);
        }

        public void Suicide(NetworkIdentity player) {
            //ScoreVictoryManager.singleton.PointLost(player);
        }

        public void PlayerDied(NetworkIdentity player) {
            // Do nothing
        }

        public void ObjectiveScored(NetworkIdentity player) {
            // Do nothing
        }
    }
}

