using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class StickyObject : MonoBehaviour {

	void OnCollisionEnter(){
        if (DebugManager.IsDebugMode()) ChatManager.DebugMessagePrint(gameObject.ToString() + " freezing all rigidbody axes");
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
	}
}
