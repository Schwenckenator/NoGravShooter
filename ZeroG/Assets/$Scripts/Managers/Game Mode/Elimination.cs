using UnityEngine;
using System.Collections;

namespace GameMode {
    public class Elimination : MonoBehaviour, IGameMode {

        public void Kill(Player killer, Player corpse) {
			foreach (Player player in NetworkManager.connectedPlayers) {
				ScoreVictoryManager.instance.PointScored(player.ID);
			}
			ScoreVictoryManager.instance.PointScored(killer.ID);
        }

        public void Suicide(Player dumbass) {
			foreach (Player player in NetworkManager.connectedPlayers) {
				ScoreVictoryManager.instance.PointScored(player.ID);
			}
			while (dumbass.IsScoreEqualOrOverAmount(1)) {
				ScoreVictoryManager.instance.PointLost(dumbass.ID);
			}
        }

        public void PlayerDied(Player player) {
			while (player.IsScoreEqualOrOverAmount(1)) {
				ScoreVictoryManager.instance.PointLost(player.ID);
			}
        }

        public void ObjectiveScored(Player player) {
            // Do Nothing
        }
    }
}

