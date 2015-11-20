using UnityEngine;
using System.Collections;

namespace GameMode {
    public class TeamDeathmatch : MonoBehaviour, IGameMode {

        public void Kill(Player killer, Player corpse) {
            // Check if legit kill or friendly fire
            if (killer.IsOnTeam(corpse.Team)) {
                FriendlyKill(killer);
            } else {
                LegitKill(killer);
            }
        }

        private void LegitKill(Player killer) {
            ScoreVictoryManager.singleton.PointScored(killer.info.id);
        }
        private void FriendlyKill(Player killer) {
            ScoreVictoryManager.singleton.PointLost(killer.info.id);
        }

        public void Suicide(Player player) {
            ScoreVictoryManager.singleton.PointLost(player.info.id);
        }

        public void PlayerDied(Player player) {
            // Do nothing
        }

        public void ObjectiveScored(Player player) {
            // Do nothing
        }
    }
}

