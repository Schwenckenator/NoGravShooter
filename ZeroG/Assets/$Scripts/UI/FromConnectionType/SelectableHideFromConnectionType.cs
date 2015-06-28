using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SelectableHideFromConnectionType : MonoBehaviour {

    public bool hideFromClient;
    public bool hideFromServer;

    private Selectable mySelectable;

    void Awake() {
        mySelectable = GetComponent<Selectable>();
    }

    public void UpdateVisibility() {
        bool hide = Network.isServer ? hideFromServer : hideFromClient;
        SetVisibility(hide);
    }

    private void SetVisibility(bool hidden) {
        mySelectable.gameObject.SetActive(!hidden);
    }

}
