using UnityEngine;
using System.Collections;

public class InitWindows : MonoBehaviour {

    public GameObject[] toInitialise;
    public GameObject[] toOpen;

    void Start() {
        Open(true, toOpen);
        StartCoroutine(CoInitialise());
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
