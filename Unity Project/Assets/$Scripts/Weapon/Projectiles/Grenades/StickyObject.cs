using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(RocketBurn))]
public class StickyObject : MonoBehaviour {

	void OnCollisionEnter(Collision info){
        if(info.collider.CompareTag("Player") || info.collider.CompareTag("GrenadeMine")) return;

        ChatManager.DebugMessage(gameObject.ToString() + " freezing all rigidbody axes");
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        GetComponent<RocketBurn>().Disable();
	}
}
