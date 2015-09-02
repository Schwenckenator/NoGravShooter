using UnityEngine;
using System.Collections;

public class DestroyParticleEffect : MonoBehaviour {


    public void DestroyAfterDelay(float delay) {
        GetComponent<ParticleSystem>().Stop();
        transform.SetParent(null);
        Invoke("DestroyMe", delay);
    }

    private void DestroyMe() {
        Destroy(gameObject);
    }
}
