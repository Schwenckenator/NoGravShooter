using UnityEngine;
using System.Collections;

public class FireWeapon : MonoBehaviour {
	Transform gun;
	Transform cameraPos;



	private LaserWeaponValues laser;
	private SlugRifleWeaponValues slug;
	private IWeaponValues[] weapons;
	private IWeaponValues currentWeapon;

	PlayerResources resource;

	GameObject shot;

	float nextFire = 0;

	// Use this for initialization
	void Start () {
		laser = ScriptableObject.CreateInstance<LaserWeaponValues>();
		slug = ScriptableObject.CreateInstance<SlugRifleWeaponValues>();

		weapons = new IWeaponValues[]{laser, slug};
		currentWeapon = laser;

		gun = transform.FindChild("CameraPos").FindChild("Weapon");
		cameraPos = transform.FindChild("CameraPos");
		resource = GetComponent<PlayerResources>();
	}
	
	// Update is called once per frame
	void Update () {
		if((Input.GetAxisRaw("Fire1") > 0) && (Time.time > nextFire) && resource.WeaponCanFire()){
			audio.PlayOneShot(currentWeapon.FireSound);
			resource.WeaponFired(currentWeapon.HeatPerShot);
			nextFire = Time.time + currentWeapon.FireDelay;
			RaycastHit hit;

			Physics.Raycast(cameraPos.position, cameraPos.forward, out hit, Mathf.Infinity);
			//Deal with the shot
			if(hit.collider.CompareTag("Player")){
				if(!hit.collider.networkView.isMine){
					hit.collider.GetComponent<PlayerResources>().TakeDamage(laser.DamagePerShot);
				}
			}
			Instantiate(currentWeapon.HitParticle, hit.point, Quaternion.identity);

			shot = Instantiate(currentWeapon.Projectile, gun.position, cameraPos.rotation) as GameObject;
			shot.transform.parent = cameraPos;
			LineRenderer render = shot.GetComponent<LineRenderer>();


			render.SetPosition(0, gun.InverseTransformPoint(gun.position));
			render.SetPosition(1, cameraPos.InverseTransformPoint(hit.point));



			networkView.RPC("MultiplayerLaserRender", RPCMode.Others, gun.position, hit.point);

		}
	}

	[RPC]
	void MultiplayerLaserRender(Vector3 start, Vector3 end){
		Instantiate(laser.HitParticle, end, Quaternion.identity);
		audio.PlayOneShot(laser.FireSound);

		shot = Instantiate(laser.Projectile, start, Quaternion.identity) as GameObject;
		LineRenderer render = shot.GetComponent<LineRenderer>();
		render.useWorldSpace = true;
		render.SetPosition(0, start);
		render.SetPosition(1, end);
	}

	public void ChangeWeapon(int weaponId){
		currentWeapon = weapons[weaponId];
	}
}
