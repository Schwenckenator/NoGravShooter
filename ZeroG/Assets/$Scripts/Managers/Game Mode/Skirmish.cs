using UnityEngine;
using System.Collections;

namespace GameMode {
    public class Skirmish : MonoBehaviour, IGameMode {

        public void Kill(Player killer, Player corpse) {
            ScoreVictoryManager.singleton.PointScored(killer, 2);
        }

        public void Suicide(Player player) {
            ScoreVictoryManager.singleton.PointLost(player);
        }

        public void PlayerDied(Player player) {
            ScoreVictoryManager.singleton.PointLost(player);
        }

        public void ObjectiveScored(Player player) {
            // Do Nothing
        }
    }
}

