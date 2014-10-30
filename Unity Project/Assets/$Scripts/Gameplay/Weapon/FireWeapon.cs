using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireWeapon : MonoBehaviour {
	Transform gun;
	Transform cameraPos;
	NoGravCharacterMotor motor;
	
	private WeaponSuperClass currentWeapon;
	
	private int currentInventorySlot;

	PlayerResources resource;

	GameObject shot;

	float nextFire = 0;

	private List<WeaponSuperClass> heldWeapons;

	// Use this for initialization
	void Awake () {

		heldWeapons = new List<WeaponSuperClass>();
		
		currentInventorySlot = 0;
		
		heldWeapons.Add(GameManager.weapon[0]);
		currentWeapon = heldWeapons[0];

		gun = transform.FindChild("CameraPos").FindChild("Weapon").FindChild("GunSmokeParticle");
		cameraPos = transform.FindChild("CameraPos");

		motor = GetComponent<NoGravCharacterMotor>();

		resource = GetComponent<PlayerResources>();

		ChangeWeapon(0);
	}
	void FixedUpdate(){
		//change weapons by mouse wheel
		if (Input.GetAxis("Mouse ScrollWheel") < 0){
			currentInventorySlot++;
			if(currentInventorySlot > NumberWeaponsHeld()){
				currentInventorySlot = 0;
			}
			ChangeWeapon(currentInventorySlot);
		} else if (Input.GetAxis("Mouse ScrollWheel") > 0){
			currentInventorySlot--;
			if(currentInventorySlot < 0){
				currentInventorySlot = NumberWeaponsHeld() - 1;
			}
			ChangeWeapon(currentInventorySlot);
		}
		
		if((Input.GetAxisRaw("Fire1") > 0) && (Time.time > nextFire) && resource.WeaponCanFire() && !GameManager.IsPaused()){
			resource.WeaponFired(currentWeapon.heatPerShot);
			audio.PlayOneShot(currentWeapon.fireSound);
			nextFire = Time.time + currentWeapon.fireDelay;


			if(currentWeapon.useRay){ //Does this weapon use a ray to hit?
				RaycastHit hit;
				for(int i=0; i< currentWeapon.rayNum; i++){
					//Find direction after shot spread
					Vector3 shotDir = cameraPos.forward;
					//Apply two rotations
					float angle1 = Random.Range(-currentWeapon.shotSpread, currentWeapon.shotSpread);
					float angle2 = Random.Range(-currentWeapon.shotSpread, currentWeapon.shotSpread);

					shotDir = Quaternion.AngleAxis(angle1, cameraPos.up) * Quaternion.AngleAxis(angle2, cameraPos.right) * shotDir;

					Physics.Raycast(cameraPos.position, shotDir, out hit, Mathf.Infinity);
					//Deal with the shot
					if(hit.collider.CompareTag("Player")){
						if(!hit.collider.networkView.isMine){
							hit.collider.GetComponent<PlayerResources>().TakeDamage(currentWeapon.damagePerShot);
						}
					}else if(hit.collider.CompareTag("BonusPickup")){
						hit.collider.GetComponent<DestroyOnNextFrame>().DestroyMe();
					}
					Instantiate(currentWeapon.hitParticle, hit.point, Quaternion.identity);

					shot = Instantiate(currentWeapon.projectile, gun.position, cameraPos.rotation) as GameObject;
					shot.transform.parent = cameraPos;
					LineRenderer render = shot.GetComponent<LineRenderer>();
					
					
					render.SetPosition(0, gun.InverseTransformPoint(gun.position));
					render.SetPosition(1, cameraPos.InverseTransformPoint(hit.point));
					
					
					if(currentWeapon.projectile == GameManager.weapon[0].projectile){
						networkView.RPC("MultiplayerLaserRender", RPCMode.Others, gun.position, hit.point);
					}else if(currentWeapon.projectile == GameManager.weapon[1].projectile){
						networkView.RPC("MultiplayerSlugRender", RPCMode.Others, gun.position, hit.point);
					}
				}
			}else{
				Network.Instantiate(currentWeapon.projectile, gun.position, cameraPos.rotation, 0);
			}
			


			if(currentWeapon.hasRecoil){
				motor.Recoil(currentWeapon.recoil);
			}
			
		}
	}

	[RPC]
	void MultiplayerLaserRender(Vector3 start, Vector3 end){

		WeaponSuperClass laser = GameManager.weapon[0];

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

		WeaponSuperClass slug = GameManager.weapon[1];

		Instantiate(slug.hitParticle, end, Quaternion.identity);
		audio.PlayOneShot(slug.fireSound);
		
		shot = Instantiate(slug.projectile, start, Quaternion.identity) as GameObject;
		LineRenderer render = shot.GetComponent<LineRenderer>();
		render.useWorldSpace = true;
		render.SetPosition(0, start);
		render.SetPosition(1, end);
	}

	public void ChangeWeapon(int weaponId){
		currentInventorySlot = weaponId;
		if(!resource.IsWeaponBusy()){
			if(GameManager.testMode){
				if(weaponId < GameManager.weapon.Count){
					currentWeapon = GameManager.weapon[weaponId];
					resource.ChangeWeapon(currentWeapon);
				}
			}
			else{
				if(weaponId < heldWeapons.Count){
					currentWeapon = heldWeapons[weaponId];
					resource.ChangeWeapon(currentWeapon);
				}
			}

		}
	}

	public bool IsWeaponHeld(int weaponId){
		return heldWeapons.Contains(GameManager.weapon[weaponId]);
	}
	
	public bool IsCurrentWeapon(int weaponId){
		return currentWeapon == GameManager.weapon[weaponId];
	}
	
	public int NumberWeaponsHeld(){
		return heldWeapons.Count;
	}

	public void AddWeapon(int weaponId){
		heldWeapons.Add(GameManager.weapon[weaponId]);
	}

	public void removeWeapon(WeaponSuperClass item){
		heldWeapons.Remove(item);
	}
	
}
