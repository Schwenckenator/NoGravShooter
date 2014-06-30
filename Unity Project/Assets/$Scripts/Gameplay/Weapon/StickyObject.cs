using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class StickyObject : MonoBehaviour {

	void OnCollisionEnter(){
		rigidbody.isKinematic = true;
	}
}
