using UnityEngine;
using System.Collections;

namespace GameMode {
    public class Skirmish : MonoBehaviour, IGameMode {

        public void Kill(Player killer, Player corpse) {
            ScoreVictoryManager.instance.PointScored(killer.ID);
            ScoreVictoryManager.instance.PointScored(killer.ID);
        }

        public void Suicide(Player player) {
            ScoreVictoryManager.instance.PointLost(player.ID);
        }

        public void PlayerDied(Player player) {
            ScoreVictoryManager.instance.PointLost(player.ID);
        }

        public void ObjectiveScored(Player player) {
            // Do Nothing
        }
    }
}

