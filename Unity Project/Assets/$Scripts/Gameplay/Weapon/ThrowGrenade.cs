using UnityEngine;
using System.Collections;

public class ThrowGrenade : MonoBehaviour {
	public GameObject[] grenade;
	private Transform grenadeSpawn;
    [SerializeField]
    private float grenadeForce = 20f;

	PlayerResources resource;

	float nextThrow = 0;
	float throwDelay = 1.5f;

	// Use this for initialization
	void Start () {
		resource = GetComponent<PlayerResources>();
		grenadeSpawn = transform.FindChild("CameraPos").FindChild("GrenadeSpawn");
	}

	// Update is called once per frame
	void Update () {
        if ((Input.GetKeyDown(SettingsManager.keyBindings[(int)SettingsManager.KeyBind.Grenade])) && Time.time > nextThrow && networkView.isMine) {
			if(resource.ThrowGrenade()){
				GameObject newGrenade = Network.Instantiate(grenade[resource.GetCurrentGrenadeType()], grenadeSpawn.position, grenadeSpawn.rotation, 0) as GameObject;
                newGrenade.rigidbody.AddRelativeForce(0, 0, grenadeForce, ForceMode.VelocityChange);
                newGrenade.GetComponent<ProjectileOwnerName>().ProjectileOwner = Network.player;
				nextThrow = Time.time + throwDelay;
			}
		}
	}
}
