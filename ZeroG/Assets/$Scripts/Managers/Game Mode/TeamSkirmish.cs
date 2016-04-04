using UnityEngine;
using System.Collections;

namespace GameMode {
    public class TeamSkirmish : MonoBehaviour, IGameMode {

        public void Kill(LobbyPlayer killer, LobbyPlayer corpse) {
            // Check if legit kill or friendly fire
            if (killer.IsOnTeam(corpse.Team)) {
                FriendlyKill(killer);
            } else {
                LegitKill(killer);
            }
        }

        private void LegitKill(LobbyPlayer killer) {
            ScoreVictoryManager.singleton.PointScored(killer, 2);
        }
        private void FriendlyKill(LobbyPlayer killer) {
            ScoreVictoryManager.singleton.PointLost(killer);
        }

        public void Suicide(LobbyPlayer player) {
            ScoreVictoryManager.singleton.PointLost(player);
        }

        public void PlayerDied(LobbyPlayer player) {
            ScoreVictoryManager.singleton.PointLost(player);
        }

        public void ObjectiveScored(LobbyPlayer player) {
            // Do nothing
        }
    }
}

