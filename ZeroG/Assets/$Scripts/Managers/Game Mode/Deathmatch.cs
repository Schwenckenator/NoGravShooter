using UnityEngine;
using System.Collections;

namespace GameMode {
    public class Deathmatch : MonoBehaviour, IGameMode {

        public void Kill(Player killer, Player corpse) {
            ScoreVictoryManager.singleton.PointScored(killer);
        }

        public void Suicide(Player player) {
            ScoreVictoryManager.singleton.PointLost(player);
        }

        public void PlayerDied(Player player) {
            // Do Nothing
        }

        public void ObjectiveScored(Player player) {
            // Do Nothing
        }
    }
}

