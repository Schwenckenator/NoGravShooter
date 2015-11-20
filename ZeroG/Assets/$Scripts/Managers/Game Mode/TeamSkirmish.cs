using UnityEngine;
using System.Collections;

namespace GameMode {
    public class TeamSkirmish : MonoBehaviour, IGameMode {

        public void Kill(Player killer, Player corpse) {
            // Check if legit kill or friendly fire
            if (killer.IsOnTeam(corpse.Team)) {
                FriendlyKill(killer);
            } else {
                LegitKill(killer);
            }
        }

        private void LegitKill(Player killer) {
            ScoreVictoryManager.singleton.PointScored(killer.ID);
            ScoreVictoryManager.singleton.PointScored(killer.ID);
        }
        private void FriendlyKill(Player killer) {
            ScoreVictoryManager.singleton.PointLost(killer.ID);
        }

        public void Suicide(Player player) {
            ScoreVictoryManager.singleton.PointLost(player.ID);
        }

        public void PlayerDied(Player player) {
            ScoreVictoryManager.singleton.PointLost(player.ID);
        }

        public void ObjectiveScored(Player player) {
            // Do nothing
        }
    }
}

