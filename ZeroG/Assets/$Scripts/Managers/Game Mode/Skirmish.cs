using UnityEngine;
using System.Collections;

namespace GameMode {
    public class Skirmish : MonoBehaviour, IGameMode {

        public void Kill(LobbyPlayer killer, LobbyPlayer corpse) {
            ScoreVictoryManager.singleton.PointScored(killer, 2);
        }

        public void Suicide(LobbyPlayer player) {
            ScoreVictoryManager.singleton.PointLost(player);
        }

        public void PlayerDied(LobbyPlayer player) {
            ScoreVictoryManager.singleton.PointLost(player);
        }

        public void ObjectiveScored(LobbyPlayer player) {
            // Do Nothing
        }
    }
}

