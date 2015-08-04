using UnityEngine;
using System.Collections;

public class ActorOnInstantiate : MonoBehaviour {

    void OnNetworkInstantiate(NetworkMessageInfo info) {
        if (!NetworkManager.IsReadyToSpawn()) {
            NetworkManager.ReserveObject(gameObject);
        }
    }
}
