using UnityEngine;
using System.Collections;

public class ProjectileCollision : MonoBehaviour {

    public GameObject rocketBlast;
    public DestroyParticleEffect particles;

    void OnCollisionEnter() {

        particles.DestroyAfterDelay();

        if (LobbyManager.isServer) {

            //GameObject explosion = Network.Instantiate(rocketBlast, transform.position, Quaternion.identity, 0) as GameObject;
            //explosion.GetComponent<Owner>().ID = GetComponent<Owner>().ID;

            //GetComponent<ObjectCleanUp>().KillMe();
        }
    }
}
