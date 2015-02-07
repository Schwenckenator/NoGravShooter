using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireWeapon : MonoBehaviour {
	public int maxWeapons = 2;

	Transform gunFirePoint;
	Transform cameraPos;
	NoGravCharacterMotor motor;
	
	private WeaponSuperClass currentWeapon;
	
	private int currentInventorySlot;
	
	public static int startingWeapon1 = 99;
	public static int startingWeapon2 = 99;
	
	PlayerResources playerResource;

	GameObject shot;

	float nextFire = 0;

	private List<WeaponSuperClass> heldWeapons;
	
    private GameManager gameManager;

	// Use this for initialization
	void Awake () {

		heldWeapons = new List<WeaponSuperClass>();

        GameObject manager = GameObject.FindGameObjectWithTag("GameController");
		gameManager = manager.GetComponent<GameManager>();
        
        SetWeaponLoadout();

		currentInventorySlot = 0;
		currentWeapon = heldWeapons[0];
		gunFirePoint = transform.FindChild("CameraPos").FindChild("Weapon").FindChild("FirePoint");
		cameraPos = transform.FindChild("CameraPos");
		motor = GetComponent<NoGravCharacterMotor>();
		playerResource = GetComponent<PlayerResources>();
		ChangeWeapon(0);
	}
    void SetWeaponLoadout() {
        if (GameManager.IsTutorialScene()) {
            int slugRifle = 1;
            int shotGun = 3;
            AddWeapon(slugRifle);
            AddWeapon(shotGun);
        } else {
            int[] temp = gameManager.GetStartingWeapons();
            foreach (int id in temp) {
                if (id < GameManager.weapon.Count) {
                    AddWeapon(id);
                }
            }
        }
    }
	void FixedUpdate(){

		//change weapons by mouse wheel
		//checks if player has max number of weapons
		if(GameManager.testMode){
			if (Input.GetAxis("Mouse ScrollWheel") < 0){
				currentInventorySlot++;
				if(NumberWeaponsHeld() < 7){
					if(currentInventorySlot >= 7){
						currentInventorySlot = 0;
					}
				} else {
					if(currentInventorySlot >= 7){
						currentInventorySlot = 0;
					}
				}
				ChangeWeapon(currentInventorySlot);
			} else if (Input.GetAxis("Mouse ScrollWheel") > 0){
				currentInventorySlot--;
				if(NumberWeaponsHeld() < 7){
					if(currentInventorySlot < 0){
						currentInventorySlot = 7 - 1;
					}
				} else {
					if(currentInventorySlot < 0){
						currentInventorySlot = 7 - 1;
					}
				}
				ChangeWeapon(currentInventorySlot);
			}
		} else {
			if (Input.GetAxis("Mouse ScrollWheel") < 0){
				currentInventorySlot++;
				if(NumberWeaponsHeld() < maxWeapons){
					if(currentInventorySlot >= NumberWeaponsHeld()){
						currentInventorySlot = 0;
					}
				} else {
					if(currentInventorySlot >= maxWeapons){
						currentInventorySlot = 0;
					}
				}
				ChangeWeapon(currentInventorySlot);
			} else if (Input.GetAxis("Mouse ScrollWheel") > 0){
				currentInventorySlot--;
				if(NumberWeaponsHeld() < maxWeapons){
					if(currentInventorySlot < 0){
						currentInventorySlot = NumberWeaponsHeld() - 1;
					}
				} else {
					if(currentInventorySlot < 0){
						currentInventorySlot = maxWeapons - 1;
					}
				}
				ChangeWeapon(currentInventorySlot);
			}
		}
		
		if((Input.GetAxisRaw("Fire1") > 0) && (Time.time > nextFire) && playerResource.WeaponCanFire() && !GameManager.IsPlayerMenu()){
			playerResource.WeaponFired(currentWeapon.heatPerShot);
            PlayFireWeaponSound();
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
							hit.collider.GetComponent<PlayerResources>().TakeDamage(currentWeapon.damagePerShot, Network.player, GameManager.WeaponClassToWeaponId(currentWeapon));
						}
					}else if(hit.collider.CompareTag("BonusPickup")){
						hit.collider.GetComponent<DestroyOnNextFrame>().DestroyMe();
					}else if(hit.collider.CompareTag("GrenadeMine")){
						hit.collider.GetComponent<MineDetonation>().Shot ();
					}
					Instantiate(currentWeapon.hitParticle, hit.point, Quaternion.identity);

					shot = Instantiate(currentWeapon.projectile, gunFirePoint.position, cameraPos.rotation) as GameObject;
					shot.transform.parent = cameraPos;
					LineRenderer render = shot.GetComponent<LineRenderer>();
					
					
					render.SetPosition(0, gunFirePoint.InverseTransformPoint(gunFirePoint.position));
					render.SetPosition(1, cameraPos.InverseTransformPoint(hit.point));
					
					
					if(currentWeapon.projectile == GameManager.weapon[0].projectile){
						networkView.RPC("MultiplayerLaserRender", RPCMode.Others, gunFirePoint.position, hit.point);
					}else if(currentWeapon.projectile == GameManager.weapon[1].projectile){
						networkView.RPC("MultiplayerSlugRender", RPCMode.Others, gunFirePoint.position, hit.point);
					}
				}
			}else{
                SpawnProjectile(gunFirePoint.position, cameraPos.rotation, Network.player);
			}
			if(currentWeapon.hasRecoil){
				motor.Recoil(currentWeapon.recoil);
			}
			
		}
	}

    [RPC]
    void SpawnProjectile(Vector3 position, Quaternion rotation, NetworkPlayer owner) {
        if (Network.isServer) {
            GameObject newObj = Network.Instantiate(currentWeapon.projectile, position, rotation, 0) as GameObject;
            if (newObj.GetComponent<ProjectileOwnerName>() != null) {
                newObj.GetComponent<ProjectileOwnerName>().ProjectileOwner = owner;
            }

        } else {
            networkView.RPC("SpawnExplosion", RPCMode.Server, position, rotation, owner);
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
		if(!playerResource.IsWeaponBusy()){
			if(GameManager.testMode){
				if(weaponId < GameManager.weapon.Count){
					currentWeapon = GameManager.weapon[weaponId];
					playerResource.ChangeWeapon(currentWeapon);
				}
			}
			else{
				if(weaponId < heldWeapons.Count){
					currentWeapon = heldWeapons[weaponId];
					playerResource.ChangeWeapon(currentWeapon);
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
	
	public int CurrentWeaponSlot(){
		return currentInventorySlot;
	}
	
	public int NumberWeaponsHeld(){
		return heldWeapons.Count;
	}
	
	public int WeaponLimit(){
		return maxWeapons;
	}

	public void AddWeapon(int weaponId){
		heldWeapons.Add(GameManager.weapon[weaponId]);
	}

	public void removeWeapon(WeaponSuperClass item){
		heldWeapons.Remove(item);
	}
	
	[RPC]
	void setWeapons(int weapon1, int weapon2){
		startingWeapon1 = weapon1;
		startingWeapon2 = weapon2;
	}

    void PlayFireWeaponSound() {
        if (networkView.isMine) {
            audio.PlayOneShot(currentWeapon.fireSound);
        }
    }
}
