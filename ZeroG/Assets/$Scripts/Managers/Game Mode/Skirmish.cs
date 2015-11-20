using UnityEngine;
using System.Collections;

namespace GameMode {
    public class Skirmish : MonoBehaviour, IGameMode {

        public void Kill(Player killer, Player corpse) {
            ScoreVictoryManager.singleton.PointScored(killer.ID);
            ScoreVictoryManager.singleton.PointScored(killer.ID);
        }

        public void Suicide(Player player) {
            ScoreVictoryManager.singleton.PointLost(player.ID);
        }

        public void PlayerDied(Player player) {
            ScoreVictoryManager.singleton.PointLost(player.ID);
        }

        public void ObjectiveScored(Player player) {
            // Do Nothing
        }
    }
}

