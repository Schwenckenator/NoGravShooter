using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(RocketBurn))]
public class StickyObject : MonoBehaviour {

	void OnCollisionEnter(){
        ChatManager.PrintMessageIfDebug(gameObject.ToString() + " freezing all rigidbody axes");
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        GetComponent<RocketBurn>().Disable();
	}
}
