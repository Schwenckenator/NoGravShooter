using UnityEngine;
using System.Collections;

public class ThrowGrenade : MonoBehaviour {
	public GameObject grenade;
	private Transform grenadeSpawn;

	// Use this for initialization
	void Start () {
		grenadeSpawn = transform.FindChild("CameraPos").FindChild("GrenadeSpawn");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if((Input.GetKeyDown(GameManagerScript.keyBindings[(int)GameManagerScript.KeyBind.Grenade]))){

			GameObject newGrenade = Instantiate(grenade, grenadeSpawn.position, grenadeSpawn.rotation) as GameObject;
			newGrenade.rigidbody.AddRelativeForce(0, 0, 15, ForceMode.VelocityChange);
		}
	}
}
