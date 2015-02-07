using UnityEngine;
using System.Collections;

public class ProjectileCollision : MonoBehaviour {

    public GameObject rocketBlast;

    void OnCollisionEnter() {
        if (Network.isServer) {

            GameObject explosion = Network.Instantiate(rocketBlast, transform.position, Quaternion.identity, 0) as GameObject;
            explosion.GetComponent<ProjectileOwnerName>().ProjectileOwner = GetComponent<ProjectileOwnerName>().ProjectileOwner;

            GetComponent<ObjectCleanUp>().KillMe();
        }
    }
}
