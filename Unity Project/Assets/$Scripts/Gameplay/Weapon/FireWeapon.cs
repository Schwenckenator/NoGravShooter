using UnityEngine;
using System.Collections;

public class FireWeapon : MonoBehaviour {
	Transform gun;
	public GameObject laserShot;
	public float fireDelay;
	public int weaponDamage;

	GameObject shot;

	float nextFire = 0;

	// Use this for initialization
	void Start () {
		gun = transform.FindChild("CameraPos").FindChild("Weapon");
	}
	
	// Update is called once per frame
	void Update () {
		if((Input.GetAxisRaw("Fire1") > 0) && (Time.time > nextFire)){
			nextFire = Time.time + fireDelay;
			RaycastHit hit;

			Physics.Raycast(gun.position, gun.forward, out hit, Mathf.Infinity);
			//Deal with the shot
			if(hit.collider.CompareTag("Player")){
				if(!hit.collider.networkView.isMine){
					hit.collider.GetComponent<PlayerResources>().TakeDamage(weaponDamage);
				}
			}


			shot = Instantiate(laserShot, gun.position, gun.rotation) as GameObject;
			shot.transform.parent = gun;
			LineRenderer render = shot.GetComponent<LineRenderer>();

			render.SetPosition(0, Vector3.zero);
			render.SetPosition(1, gun.InverseTransformPoint(hit.point));

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
