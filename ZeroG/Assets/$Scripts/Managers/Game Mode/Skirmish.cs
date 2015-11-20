using UnityEngine;
using System.Collections;

namespace GameMode {
    public class Skirmish : MonoBehaviour, IGameMode {

        public void Kill(Player killer, Player corpse) {
            ScoreVictoryManager.singleton.PointScored(killer.info.id);
            ScoreVictoryManager.singleton.PointScored(killer.info.id);
        }

        public void Suicide(Player player) {
            ScoreVictoryManager.singleton.PointLost(player.info.id);
        }

        public void PlayerDied(Player player) {
            ScoreVictoryManager.singleton.PointLost(player.info.id);
        }

        public void ObjectiveScored(Player player) {
            // Do Nothing
        }
    }
}

