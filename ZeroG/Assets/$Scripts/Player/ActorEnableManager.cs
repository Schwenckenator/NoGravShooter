using UnityEngine;
using System.Collections;

public class ActorEnableManager : MonoBehaviour {

    new NetworkView networkView;
    private GameObject myActor;

    void Awake() {
        networkView = GetComponent<NetworkView>();
        DontDestroyOnLoad(gameObject);
    }

    public void SetActor(GameObject actor) {
        myActor = actor;
        DontDestroyOnLoad(myActor);
        networkView.RPC("SetActorByViewID", RPCMode.OthersBuffered, actor.GetComponent<NetworkView>().viewID);
    }

    [RPC]
    void SetActorByViewID(NetworkViewID viewID) {
        myActor = NetworkView.Find(viewID).gameObject;
        DontDestroyOnLoad(myActor);
    }

    [RPC]
    public void DisableActor(bool sendRPC = true) {
        ChatManager.DebugMessage("Disabling actor." + networkView.viewID);
        if (sendRPC) {
            networkView.RPC("DisableActor", RPCMode.Others, false);
        }

        myActor.SetActive(false);
    }

    [RPC]
    public void EnableActor(bool sendRPC = true) {
        ChatManager.DebugMessage("Enabling actor." + networkView.viewID);
        myActor.SetActive(true);
        if (sendRPC) {
            networkView.RPC("EnableActor", RPCMode.Others, false);
        }
    }

    // Kill self on disconnect
    void OnDisconnectedFromServer() {
        Debug.Log("Destroyed Actor Enable Manager.");
        Destroy(myActor);
        Destroy(gameObject);
    }
}
