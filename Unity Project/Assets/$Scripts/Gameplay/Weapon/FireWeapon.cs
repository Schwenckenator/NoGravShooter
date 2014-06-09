using UnityEngine;
using System.Collections;

public class FireWeapon : MonoBehaviour {
	Transform gun;
	Transform cameraPos;

	public GameObject particle;

	PlayerResources resource;
	public AudioClip soundLaserShot;
	public GameObject laserShot;
	public float fireDelay;
	public int weaponDamage;
	public float weaponHeat;

	GameObject shot;

	float nextFire = 0;

	// Use this for initialization
	void Start () {
		gun = transform.FindChild("CameraPos").FindChild("Weapon");
		cameraPos = transform.FindChild("CameraPos");
		resource = GetComponent<PlayerResources>();
	}
	
	// Update is called once per frame
	void Update () {
		if((Input.GetAxisRaw("Fire1") > 0) && (Time.time > nextFire) && resource.WeaponCanFire()){
			audio.PlayOneShot(soundLaserShot);
			resource.WeaponFired(weaponHeat);
			nextFire = Time.time + fireDelay;
			RaycastHit hit;

			Physics.Raycast(cameraPos.position, cameraPos.forward, out hit, Mathf.Infinity);
			//Deal with the shot
			if(hit.collider.CompareTag("Player")){
				if(!hit.collider.networkView.isMine){
					hit.collider.GetComponent<PlayerResources>().TakeDamage(weaponDamage);
				}
			}
			Network.Instantiate(particle, hit.point, Quaternion.identity, 0);

			shot = Instantiate(laserShot, gun.position, cameraPos.rotation) as GameObject;
			shot.transform.parent = cameraPos;
			LineRenderer render = shot.GetComponent<LineRenderer>();


			render.SetPosition(0, gun.InverseTransformPoint(gun.position));
			render.SetPosition(1, cameraPos.InverseTransformPoint(hit.point));



			networkView.RPC("MultiplayerLaserRender", RPCMode.Others, gun.position, hit.point);

		}
	}

	[RPC]
	void MultiplayerLaserRender(Vector3 start, Vector3 end){
		shot = Instantiate(laserShot, start, Quaternion.identity) as GameObject;
		LineRenderer render = shot.GetComponent<LineRenderer>();
		render.useWorldSpace = true;
		render.SetPosition(0, start);
		render.SetPosition(1, end);
	}
}
