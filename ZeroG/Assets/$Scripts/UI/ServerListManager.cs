using UnityEngine;
using System.Collections;
using UnityEngine.Networking.Match;

public class ServerListManager : MonoBehaviour {

    public GameObject serverListObj;

    public ServerListEntry AddServer(MatchDesc match) {

        GameObject newServerObj = Instantiate(serverListObj) as GameObject;
        newServerObj.transform.SetParent(this.transform); // Become child of this
        
        ServerListEntry newServer = newServerObj.GetComponent<ServerListEntry>();
        newServer.match = match;
        newServer.Name.text = match.name;
        newServer.PlayerCount.text = match.currentSize + "/" + match.maxSize;
        
        return newServer;
    }
}
