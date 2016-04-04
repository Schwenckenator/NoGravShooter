using UnityEngine;
using System.Collections;

namespace GameMode {
    public class TeamDeathmatch : MonoBehaviour, IGameMode {

        public void Kill(LobbyPlayer killer, LobbyPlayer corpse) {
            // Check if legit kill or friendly fire
            if (killer.IsOnTeam(corpse.Team)) {
                FriendlyKill(killer);
            } else {
                LegitKill(killer);
            }
        }

        private void LegitKill(LobbyPlayer killer) {
            ScoreVictoryManager.singleton.PointScored(killer);
        }
        private void FriendlyKill(LobbyPlayer killer) {
            ScoreVictoryManager.singleton.PointLost(killer);
        }

        public void Suicide(LobbyPlayer player) {
            ScoreVictoryManager.singleton.PointLost(player);
        }

        public void PlayerDied(LobbyPlayer player) {
            // Do nothing
        }

        public void ObjectiveScored(LobbyPlayer player) {
            // Do nothing
        }
    }
}

