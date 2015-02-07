using UnityEngine;
using System.Collections;

public class ObjectSpawnManager : MonoBehaviour {

    [RPC]
    public void Spawn(GameObject original, Vector3 position, Quaternion rotation) {
        if (Network.isServer) {
            Network.Instantiate(original, position, rotation, 0);
        } else {
            networkView.RPC("Spawn", RPCMode.Server, original, position, rotation);
        }
    }
}
