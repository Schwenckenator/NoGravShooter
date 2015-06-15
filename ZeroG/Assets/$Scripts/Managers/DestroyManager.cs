using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DestroyManager : MonoBehaviour {

    #region Instance
    //Here is a private reference only this class can access
    private static DestroyManager _instance;
    //This is the public reference that other classes will use
    public static DestroyManager instance {
        get {
            //If _instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<DestroyManager>();
            }
            return _instance;
        }
    }
    #endregion

    private List<NetworkViewID> DestroyBuffer;
    private List<NetworkViewID> DestroyedObjects;
    private bool willDestroy = false;

    void Start() {
        DestroyBuffer = new List<NetworkViewID>();
        DestroyedObjects = new List<NetworkViewID>();
    }

    /// <summary>
    /// Will destroy the object everywhere.
    /// </summary>
    /// <param name="obj"></param>
    public void CleanUp(GameObject obj) {
        AddToDestroyList(obj.GetComponent<NetworkView>().viewID);
    }
    [RPC]
    void AddToDestroyList(NetworkViewID viewID) {
        if (Network.isServer) {
            AddToDestroyBufferIfUnique(viewID);
        } else {
            ChatManager.DebugMessage("I am a client.");
            GetComponent<NetworkView>().RPC("AddToDestroyList", RPCMode.Server, viewID);
        }

    }
    void AddToDestroyBufferIfUnique(NetworkViewID viewID) {
        if (Network.isClient) {
            throw new ClientRunningServerCodeException("Thrown in DestroyManager.");
        }
        // Check for uniqueness
        if (DestroyBuffer.Contains(viewID) || DestroyedObjects.Contains(viewID)) {
            ChatManager.DebugMessage("viewID not unique. Not destroying.");
            return;
        }
            

        DestroyBuffer.Add(viewID);
        if (!willDestroy) {
            willDestroy = true;
            StartCoroutine(DestroyBufferObjects());
        }
    }
    /// <summary>
    /// Use with caution, direct call to destroy.
    /// </summary>
    /// <param name="viewID"></param>
    public void Destroy(NetworkViewID viewID) {
        Network.RemoveRPCs(viewID);
        Network.Destroy(viewID);
    }

    IEnumerator DestroyBufferObjects() {
        yield return new WaitForEndOfFrame();

        foreach (NetworkViewID viewID in DestroyBuffer) {
            ChatManager.DebugMessage("Destroying viewID: "+viewID.ToString());
            DestroyedObjects.Add(viewID);
            this.Destroy(viewID);
        }
        DestroyBuffer.Clear();
        willDestroy = false;
    }

    public void ClearDestroyLists() {
        DestroyBuffer.Clear();
        DestroyedObjects.Clear();
    }
}
