using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireWeapon : MonoBehaviour {
	public Transform gunFirePoint;
	public Transform cameraAnchor;
	NoGravCharacterMotor motor;
	
    WeaponInventory inventory;
    WeaponResources weaponResources;
	GameObject shot;

	float nextFire = 0;
	// Use this for initialization
	void Awake () {
		//gunFirePoint = transform.FindChild("CameraPos").FindChild("Weapon").FindChild("FirePoint");
		//cameraAnchor = transform.FindChild("CameraPos");
		motor = GetComponent<NoGravCharacterMotor>();
        inventory = GetComponent<WeaponInventory>();
        weaponResources = GetComponent<WeaponResources>();
	}
	void Update(){
        if (inventory.HasNoWeapons()) return;
        

        if (!inventory.currentWeapon.useRay && IsWeaponFiring() && CanWeaponFire()) {
            WeaponFired();
        }

        // Check for dry fire
        if (IsWeaponFiring() && weaponResources.isWeaponEmpty()) {
           weaponResources.SafeStartReload();
        }
	}

    void FixedUpdate() {
        if (inventory.HasNoWeapons()) return;
        if (inventory.currentWeapon.useRay && IsWeaponFiring() && CanWeaponFire()) {
            WeaponFired();
        } 
    }

    private bool IsWeaponFiring() {
        return (Input.GetAxisRaw("Fire1") > 0);
    }
    private bool CanWeaponFire() {
        return (Time.time > nextFire) && weaponResources.WeaponCanFire() && !WeaponInventory.isChanging && !GameManager.IsPlayerMenu();
    }

    private void WeaponFired() {
        weaponResources.WeaponFired(inventory.currentWeapon.heatPerShot);
        PlayFireWeaponSound();
        nextFire = Time.time + inventory.currentWeapon.fireDelay;

        if (inventory.currentWeapon.useRay) { //Does this weapon use a ray to hit?

            for (int i = 0; i < inventory.currentWeapon.rayNum; i++) {
                FireRay();
            }
        } else {
            SpawnProjectile(GameManager.WeaponClassToWeaponId(inventory.currentWeapon), gunFirePoint.position, cameraAnchor.rotation, Network.player);
        }
        if (inventory.currentWeapon.hasRecoil) {
            motor.Recoil(inventory.currentWeapon.recoil);
        }

    }
	
	private void FireRay() {
        RaycastHit hit = new RaycastHit();
        //Find direction after shot spread
        Vector3 shotDir = cameraAnchor.forward;
        //Apply two rotations
        float angle1 = Random.Range(-inventory.currentWeapon.shotSpread, inventory.currentWeapon.shotSpread);
        float angle2 = Random.Range(-inventory.currentWeapon.shotSpread, inventory.currentWeapon.shotSpread);

        shotDir = Quaternion.AngleAxis(angle1, cameraAnchor.up) * Quaternion.AngleAxis(angle2, cameraAnchor.right) * shotDir;
		
        Physics.Raycast(cameraAnchor.position, shotDir, out hit, Mathf.Infinity);

        //Deal with the shot
        Collider col = hit.collider;
        if (col.CompareTag("Player") || col.CompareTag("BonusPickup") || col.CompareTag("Grenade")) {
            IDamageable damageable = hit.collider.gameObject.GetInterface<IDamageable>();
            if (damageable != null) {
                damageable.TakeDamage(inventory.currentWeapon.damagePerShot, Network.player, GameManager.WeaponClassToWeaponId(inventory.currentWeapon));
            }
        }


        ShotRender(gunFirePoint.position, hit.point);

        //Render shot everywhere else
        GetComponent<NetworkView>().RPC("NetworkShotRender", RPCMode.Others, GameManager.WeaponClassToWeaponId(inventory.currentWeapon), gunFirePoint.position, hit.point);
    }

    [RPC]
    void SpawnProjectile(int weaponID, Vector3 position, Quaternion rotation, NetworkPlayer owner) {
        if (Network.isServer) {
            GameObject newObj = Network.Instantiate(GameManager.weapon[weaponID].projectile, position, rotation, 0) as GameObject;
            if (newObj.GetComponent<Owner>() != null) {
                newObj.GetComponent<Owner>().ID = owner;
            }

        } else {
            GetComponent<NetworkView>().RPC("SpawnProjectile", RPCMode.Server, weaponID, position, rotation, owner);
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
        shot = Instantiate(inventory.currentWeapon.projectile, cameraAnchor.position, cameraAnchor.rotation) as GameObject;
        shot.transform.parent = cameraAnchor;
        LineRenderer lineRenderer = shot.GetComponent<LineRenderer>();

        start = cameraAnchor.InverseTransformPoint(start);
        end = cameraAnchor.InverseTransformPoint(end);

        lineRenderer.useWorldSpace = false;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

	
	
    void PlayFireWeaponSound() {
        if (GetComponent<NetworkView>().isMine) {
            GetComponent<AudioSource>().PlayOneShot(inventory.currentWeapon.fireSound);

            GetComponent<NetworkView>().RPC("RPCPlayFireWeaponSound", RPCMode.Others, GameManager.WeaponClassToWeaponId(inventory.currentWeapon));
        }
    }
    [RPC]
    void RPCPlayFireWeaponSound(int weaponID) {
        WeaponSuperClass weapon = GameManager.weapon[weaponID];
        GetComponent<AudioSource>().PlayOneShot(weapon.fireSound);
    }
}
