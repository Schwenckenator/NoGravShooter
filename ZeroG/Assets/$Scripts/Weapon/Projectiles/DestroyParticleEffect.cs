using UnityEngine;
using System.Collections;

public class DestroyParticleEffect : MonoBehaviour {

    public float delay;

    public void DestroyAfterDelay() {
        GetComponent<ParticleSystem>().Stop();
        transform.SetParent(null);
        Invoke("DestroyMe", delay);
    }

    private void DestroyMe() {
        Destroy(gameObject);
    }
}
