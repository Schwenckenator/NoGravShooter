using UnityEngine;
using System.Collections;

public class OnLevelOpen : MonoBehaviour {

    public GameObject[] toInitialise;
    public GameObject[] toOpen;

    void Awake() {
        Open(true, toOpen);
        StartCoroutine(CoInitialise());
        Destroy(gameObject, 1f);
    }

    IEnumerator CoInitialise() {
        Open(true, toInitialise);
        yield return null;
        Open(false, toInitialise);
    }

    void Open(bool open, GameObject[] list) {
        foreach (GameObject init in list) {
            init.SetActive(open);
        }
    }
}
