using UnityEngine;
using System.Collections;

namespace GameMode {
    public class TeamDeathmatch : MonoBehaviour, IGameMode {
        // Use this for initialization
        void Start() {
        }

        // Update is called once per frame
        void Update() {

        }

        public void KillScored(Player killer) {
            throw new System.NotImplementedException();
        }

        public void PlayerDied(Player player) {
            throw new System.NotImplementedException();
        }

        public void ObjectiveScored(Player player) {
            throw new System.NotImplementedException();
        }


        public void FriendlyKill(Player killer) {
            throw new System.NotImplementedException();
        }
    }
}

