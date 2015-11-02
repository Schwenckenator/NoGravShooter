using UnityEngine;
using System.Collections;

public class Owner : MonoBehaviour {

    private NetworkPlayer id;

    public NetworkPlayer ID {
        get { return id; }
        set {
            id = value;
            //NetworkView.RPC("SetProjectileOwner", RPCMode.OthersBuffered, value);
        }
    }

    //NetworkView //NetworkView;
    void Awake() {
        //NetworkView = GetComponent<//NetworkView>();
    }
    //[RPC]
    private void SetProjectileOwner(NetworkPlayer value) {
        id = value;
    }
}
