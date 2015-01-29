using UnityEngine;
using System.Collections;

public class ProjectileOwnerName : MonoBehaviour {

    private NetworkPlayer projectileOwner;

    public NetworkPlayer ProjectileOwner {
        get { return projectileOwner; }
        set {
            networkView.RPC("SetProjectileOwner", RPCMode.AllBuffered, value);
        }
    }
    [RPC]
    private void SetProjectileOwner(NetworkPlayer owner) {
        projectileOwner = owner;
    }
}
