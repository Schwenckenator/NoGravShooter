using UnityEngine;
using System.Collections;

public class PingDisplay : MonoBehaviour {

    public float updateRate = 1.0f;
    public bool isShowPing = false;

    private string serverIP;
    private Ping ping;
    int time; 

    void OnConnectedToServer() {
        Debug.Log("Connected to server!");
        isShowPing = true;
        StartCoroutine(PingServerLoop());
    }
    void OnDisconnectedFromServer() {
        isShowPing = false;
        StopAllCoroutines();
    }

    void Update() {
        if (!isShowPing) return;

        if (ping.isDone) {
            time = ping.time;
        }
    }

    void OnGUI() {
        if (!isShowPing) return;

        if (time < 50) {
            GUI.contentColor = Color.white;
        } else if (time < 150) {
            GUI.contentColor = Color.yellow;
        } else {
            GUI.contentColor = Color.red;
        }

        GUI.Label(new Rect(Screen.width - 100, 40, 100, 20), "Ping: " + time.ToString());
    }

    private IEnumerator PingServerLoop() {
        serverIP = Network.connections[0].ipAddress;
        while (Network.isClient) {
            ping = new Ping(serverIP);
            yield return new WaitForSeconds(1f / updateRate);
        }
    }
}
