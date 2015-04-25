using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SelectableHideFromConnectionType : MonoBehaviour {

    public bool hideFromClient;
    public bool hideFromServer;

    private Selectable myButton;

    void Awake() {
        myButton = GetComponent<Selectable>();
    }

    public void UpdateVisibility() {
        bool hide = Network.isServer ? hideFromServer : hideFromClient;
        SetVisibility(hide);
    }

    private void SetVisibility(bool hidden) {
        myButton.enabled = !hidden;
    }

}
