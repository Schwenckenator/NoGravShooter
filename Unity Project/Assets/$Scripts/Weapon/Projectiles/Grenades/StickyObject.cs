using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(RocketBurn))]
public class StickyObject : MonoBehaviour {

	void OnCollisionEnter(Collision info){
        if(info.collider.CompareTag("Player") || info.collider.CompareTag("GrenadeMine")) return;

        Freeze();
	}

    void Freeze() {
        ChatManager.DebugMessage(gameObject.ToString() + " freezing all rigidbody axes");
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        GetComponent<RocketBurn>().Disable();
    }

    public void UnStick() {
        ChatManager.DebugMessage(gameObject.ToString() + " unsticking all rigidbody axes");

        rigidbody.constraints = RigidbodyConstraints.None;
    }

    public bool IsFrozen() {
        return rigidbody.constraints == RigidbodyConstraints.FreezeAll;
    }
}
