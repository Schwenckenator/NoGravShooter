using UnityEngine;
using System.Collections;

public class ServerListManager : MonoBehaviour {

    public GameObject serverListObj;

    public ServerListEntry AddServer(string name, string status, string player) {
        GameObject newServerObj = Instantiate(serverListObj) as GameObject;
        newServerObj.transform.parent = this.transform; // Become child of this
        
        ServerListEntry newServer = newServerObj.GetComponent<ServerListEntry>();
        newServer.Name.text = name;
        newServer.Status.text = status;
        newServer.PlayerCount.text = player;
        
        return newServer;
    }
}
