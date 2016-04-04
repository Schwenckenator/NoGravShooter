using UnityEngine;
using System.Collections;

namespace GameMode {
    public class Deathmatch : MonoBehaviour, IGameMode {

        public void Kill(LobbyPlayer killer, LobbyPlayer corpse) {
            Debug.Log("Kill scored.");
            ScoreVictoryManager.singleton.PointScored(killer);
        }

        public void Suicide(LobbyPlayer player) {
            Debug.Log("Suicide.");
            ScoreVictoryManager.singleton.PointLost(player);
        }

        public void PlayerDied(LobbyPlayer player) {
            // Do Nothing
        }

        public void ObjectiveScored(LobbyPlayer player) {
            // Do Nothing
        }
    }
}

