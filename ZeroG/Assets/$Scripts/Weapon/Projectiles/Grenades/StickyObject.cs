using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Projectile))]
public class StickyObject : MonoBehaviour {

	void OnCollisionEnter(Collision info){
        if(info.collider.CompareTag("Player") || info.collider.CompareTag("Grenade")) return;

        Freeze();
	}

    void Freeze() {
        //ChatManager.DebugMessage(gameObject.ToString() + " freezing all rigidbody axes");
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        GetComponent<Projectile>().Disable();
    }

    public void UnStick() {
        //ChatManager.DebugMessage(gameObject.ToString() + " unsticking all rigidbody axes");

        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    }

    public bool IsFrozen() {
        return GetComponent<Rigidbody>().constraints == RigidbodyConstraints.FreezeAll;
    }
}
