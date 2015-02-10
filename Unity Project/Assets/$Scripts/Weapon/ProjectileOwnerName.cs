using UnityEngine;
using System.Collections;

public class ProjectileOwnerName : MonoBehaviour {

    private NetworkPlayer projectileOwner;

    public NetworkPlayer ProjectileOwner {
        get { return projectileOwner; }
        set {
            ChatManager.PrintMessageIfDebug("I am: " + gameObject.ToString());
            projectileOwner = value;
            networkView.RPC("SetProjectileOwner", RPCMode.OthersBuffered, value);
        }
    }
    [RPC]
    private void SetProjectileOwner(NetworkPlayer owner) {
        ChatManager.PrintMessageIfDebug("In RPC. I am: " + gameObject.ToString());
        projectileOwner = owner;
    }
}
