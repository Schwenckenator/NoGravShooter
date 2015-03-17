using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireWeapon : MonoBehaviour {
	public int maxHeldWeapons = 2;

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
	

	// Use this for initialization
	void Awake () {
		heldWeapons = new List<WeaponSuperClass>();
        SetWeaponLoadout();

		currentInventorySlot = 0;
		currentWeapon = heldWeapons[0];
		gunFirePoint = transform.FindChild("CameraPos").FindChild("Weapon").FindChild("FirePoint");
		cameraPos = transform.FindChild("CameraPos");
		motor = GetComponent<NoGravCharacterMotor>();
		playerResource = GetComponent<PlayerResources>();
        
        if (networkView.isMine) {
            ChangeWeapon(0);
        }
		
	}
    void SetWeaponLoadout() {
        if (GameManager.IsSceneTutorial()) {
            int slugRifle = 1;
            int shotGun = 3;
            AddWeapon(slugRifle);
            AddWeapon(shotGun);
        } else {
            int[] temp = GameManager.instance.GetStartingWeapons();
            foreach (int id in temp) {
                if (id < GameManager.weapon.Count) {
                    AddWeapon(id);
                }
            }
        }
    }
	void Update(){
        MouseWheelWeaponChange();

        if (!currentWeapon.useRay && CanWeaponFire()) {
            WeaponFired();
        } else if (!CanWeaponFire()) {
            playerResource.SafeStartReload();
        }
	}

    void FixedUpdate() {
        if (currentWeapon.useRay && CanWeaponFire()) {
            WeaponFired();
        } else if (!CanWeaponFire()) {
            playerResource.SafeStartReload();
        }
    }

    private bool CanWeaponFire() {
        return (Input.GetAxisRaw("Fire1") > 0) && (Time.time > nextFire) && playerResource.WeaponCanFire() && !GameManager.IsPlayerMenu();
    }

    private void WeaponFired() {
        playerResource.WeaponFired(currentWeapon.heatPerShot);
        PlayFireWeaponSound();
        nextFire = Time.time + currentWeapon.fireDelay;

        if (currentWeapon.useRay) { //Does this weapon use a ray to hit?
            
            for (int i = 0; i < currentWeapon.rayNum; i++) {
                FireRay();
            }
        } else {
            SpawnProjectile(GameManager.WeaponClassToWeaponId(currentWeapon), gunFirePoint.position, cameraPos.rotation, Network.player);
        }
        if (currentWeapon.hasRecoil) {
            motor.Recoil(currentWeapon.recoil);
        }

    }
	
	private void FireRay() {
        RaycastHit hit = new RaycastHit();
        //Find direction after shot spread
        Vector3 shotDir = cameraPos.forward;
        //Apply two rotations
        float angle1 = Random.Range(-currentWeapon.shotSpread, currentWeapon.shotSpread);
        float angle2 = Random.Range(-currentWeapon.shotSpread, currentWeapon.shotSpread);

        shotDir = Quaternion.AngleAxis(angle1, cameraPos.up) * Quaternion.AngleAxis(angle2, cameraPos.right) * shotDir;
		
        Physics.Raycast(cameraPos.position, shotDir, out hit, Mathf.Infinity);

        //Deal with the shot
        if (hit.collider.CompareTag("Player")) {
            if (!hit.collider.networkView.isMine) {
                hit.collider.GetComponent<PlayerResources>().TakeDamage(currentWeapon.damagePerShot, Network.player, GameManager.WeaponClassToWeaponId(currentWeapon));
                Instantiate(currentWeapon.hitParticle, hit.point, Quaternion.identity);
            }
        } else if (hit.collider.CompareTag("BonusPickup")) {
            hit.collider.GetComponent<DestroyOnNextFrame>().DestroyMe();
        } else if (hit.collider.CompareTag("Grenade")) {
            hit.collider.GetComponent<MineDetonation>().ForceDetonate();
        }
        //Instantiate(currentWeapon.hitParticle, hit.point, Quaternion.identity);

        ShotRender(gunFirePoint.position, hit.point);

        //Render shot everywhere else
        networkView.RPC("NetworkShotRender", RPCMode.Others, GameManager.WeaponClassToWeaponId(currentWeapon), gunFirePoint.position, hit.point);
    }

    private void MouseWheelWeaponChange() {
        //change weapons by mouse wheel
        //checks if player has max number of weapons
        if (playerResource.IsWeaponBusy()) return;

        if (Input.GetAxis("Mouse ScrollWheel") < 0) {
            currentInventorySlot++;
            currentInventorySlot %= GetMaxWeapon();
            ChangeWeapon(currentInventorySlot);

        } else if (Input.GetAxis("Mouse ScrollWheel") > 0) {
            currentInventorySlot--;
            if (currentInventorySlot < 0) currentInventorySlot += GetMaxWeapon();
            ChangeWeapon(currentInventorySlot);
        }
    }

    private int GetMaxWeapon() {
        if (DebugManager.IsAllWeapon()) {
            return GameManager.weapon.Count;
        } else {
            return NumberWeaponsHeld();
        }
    }

    [RPC]
    void SpawnProjectile(int weaponID, Vector3 position, Quaternion rotation, NetworkPlayer owner) {
        if (Network.isServer) {
            GameObject newObj = Network.Instantiate(GameManager.weapon[weaponID].projectile, position, rotation, 0) as GameObject;
            if (newObj.GetComponent<Owner>() != null) {
                newObj.GetComponent<Owner>().ID = owner;
            }

        } else {
            networkView.RPC("SpawnProjectile", RPCMode.Server, weaponID, position, rotation, owner);
        }
    }

    [RPC]
    void NetworkShotRender(int weaponID, Vector3 start, Vector3 end) {
        WeaponSuperClass weapon = GameManager.weapon[weaponID];

        Instantiate(weapon.hitParticle, end, Quaternion.identity);


        shot = Instantiate(weapon.projectile, start, Quaternion.identity) as GameObject;
        LineRenderer render = shot.GetComponent<LineRenderer>();
        render.useWorldSpace = true;
        render.SetPosition(0, start);
        render.SetPosition(1, end);
    }

    void ShotRender(Vector3 start, Vector3 end){
        shot = Instantiate(currentWeapon.projectile, cameraPos.position, cameraPos.rotation) as GameObject;
        shot.transform.parent = cameraPos;
        LineRenderer lineRenderer = shot.GetComponent<LineRenderer>();

        start = cameraPos.InverseTransformPoint(start);
        end = cameraPos.InverseTransformPoint(end);

        lineRenderer.useWorldSpace = false;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

	public void ChangeWeapon(int weaponId){
		currentInventorySlot = weaponId;
		if(!playerResource.IsWeaponBusy()){
            if (DebugManager.IsAllWeapon()) {
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
		return maxHeldWeapons;
	}

	public void AddWeapon(int weaponId){
		heldWeapons.Add(GameManager.weapon[weaponId]);
	}
    public void AddWeapon(int weaponId, int index){
        heldWeapons.Insert(index, GameManager.weapon[weaponId]);
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

            networkView.RPC("RPCPlayFireWeaponSound", RPCMode.Others, GameManager.WeaponClassToWeaponId(currentWeapon));
        }
    }
    [RPC]
    void RPCPlayFireWeaponSound(int weaponID) {
        WeaponSuperClass weapon = GameManager.weapon[weaponID];
        audio.PlayOneShot(weapon.fireSound);
    }
}
