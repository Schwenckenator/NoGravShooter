using UnityEngine;
using System.Collections;

public class ActorOnInstantiate : MonoBehaviour {

    void OnNetworkInstantiate(NetworkMessageInfo info) {
        //NetworkView nView = GetComponent<//NetworkView>();
        //if (!NetworkManager.IsReadyToSpawn()) {
        //    NetworkManager.ReserveObject(info, nView, gameObject);
        //}

        //ChatManager.DebugMessage("NetworkViewID is: " + nView.viewID.ToString());
        //ChatManager.DebugMessage("//NetworkView owner is: " + nView.owner.ToString());
    }
}
