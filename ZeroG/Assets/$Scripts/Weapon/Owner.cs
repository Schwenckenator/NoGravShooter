using UnityEngine;
using System.Collections;

public class Owner : MonoBehaviour {

    private NetworkPlayer id;

    public NetworkPlayer ID {
        get { return id; }
        set {
            id = value;
            networkView.RPC("SetProjectileOwner", RPCMode.OthersBuffered, value);
        }
    }

    NetworkView networkView;
    void Start() {
        networkView = GetComponent<NetworkView>();
    }
    [RPC]
    private void SetProjectileOwner(NetworkPlayer value) {
        id = value;
    }
}
