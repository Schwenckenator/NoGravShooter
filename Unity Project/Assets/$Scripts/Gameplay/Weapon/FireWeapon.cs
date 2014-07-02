using UnityEngine;
using System.Collections;

public class FireWeapon : MonoBehaviour {
	Transform gun;
	Transform cameraPos;
	NoGravCharacterMotor motor;


	private LaserWeaponValues laser;
	private SlugRifleWeaponValues slug;
	private IWeaponValues[] weapons;
	private IWeaponValues currentWeapon;

	PlayerResources resource;

	GameObject shot;

	float nextFire = 0;

	// Use this for initialization
	void Awake () {
		laser = ScriptableObject.CreateInstance<LaserWeaponValues>();
		slug = ScriptableObject.CreateInstance<SlugRifleWeaponValues>();

		weapons = new IWeaponValues[]{laser, slug};
		currentWeapon = laser;

		gun = transform.FindChild("CameraPos").FindChild("Weapon");
		cameraPos = transform.FindChild("CameraPos");

		motor = GetComponent<NoGravCharacterMotor>();

		resource = GetComponent<PlayerResources>();

		ChangeWeapon(0);
	}
	void FixedUpdate(){
		if((Input.GetAxisRaw("Fire1") > 0) && (Time.time > nextFire) && resource.WeaponCanFire()){
			resource.WeaponFired(currentWeapon.heatPerShot);
			audio.PlayOneShot(currentWeapon.fireSound);
			nextFire = Time.time + currentWeapon.fireDelay;
			RaycastHit hit;
			
			Physics.Raycast(cameraPos.position, cameraPos.forward, out hit, Mathf.Infinity);
			//Deal with the shot
			if(hit.collider.CompareTag("Player")){
				if(!hit.collider.networkView.isMine){
					hit.collider.GetComponent<PlayerResources>().TakeDamage(currentWeapon.damagePerShot);
				}
			}else if(hit.collider.CompareTag("BonusPickup")){
				Network.Destroy(hit.collider.gameObject);
			}
			Instantiate(currentWeapon.hitParticle, hit.point, Quaternion.identity);
			
			shot = Instantiate(currentWeapon.projectile, gun.position, cameraPos.rotation) as GameObject;
			shot.transform.parent = cameraPos;
			LineRenderer render = shot.GetComponent<LineRenderer>();
			
			
			render.SetPosition(0, gun.InverseTransformPoint(gun.position));
			render.SetPosition(1, cameraPos.InverseTransformPoint(hit.point));
			
			
			if(currentWeapon == laser){
				networkView.RPC("MultiplayerLaserRender", RPCMode.Others, gun.position, hit.point);
			}else if(currentWeapon == slug){
				networkView.RPC("MultiplayerSlugRender", RPCMode.Others, gun.position, hit.point);
			}

			if(currentWeapon == slug){
				motor.Recoil();
			}
			
		}
	}

	[RPC]
	void MultiplayerLaserRender(Vector3 start, Vector3 end){
		Instantiate(laser.hitParticle, end, Quaternion.identity);
		audio.PlayOneShot(laser.fireSound);

		shot = Instantiate(laser.projectile, start, Quaternion.identity) as GameObject;
		LineRenderer render = shot.GetComponent<LineRenderer>();
		render.useWorldSpace = true;
		render.SetPosition(0, start);
		render.SetPosition(1, end);
	}

	[RPC]
	void MultiplayerSlugRender(Vector3 start, Vector3 end){
		Instantiate(slug.hitParticle, end, Quaternion.identity);
		audio.PlayOneShot(slug.fireSound);
		
		shot = Instantiate(slug.projectile, start, Quaternion.identity) as GameObject;
		LineRenderer render = shot.GetComponent<LineRenderer>();
		render.useWorldSpace = true;
		render.SetPosition(0, start);
		render.SetPosition(1, end);
	}

	public void ChangeWeapon(int weaponId){
		currentWeapon = weapons[weaponId];
		resource.ChangeWeapon(currentWeapon);
	}


}
