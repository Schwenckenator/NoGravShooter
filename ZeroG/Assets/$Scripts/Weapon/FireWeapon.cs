using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireWeapon : MonoBehaviour {
	public Transform gunFirePoint;
	public Transform cameraAnchor;
	ActorMotorManager motor;
	
    WeaponInventory inventory;
    WeaponResources weaponResources;
	//GameObject shot;

	float nextFire = 0;

    NetworkView networkView;
    AudioSource audioSource;
	// Use this for initialization
	void Awake () {
		motor = GetComponent<ActorMotorManager>();
        inventory = GetComponent<WeaponInventory>();
        weaponResources = GetComponent<WeaponResources>();
        networkView = GetComponent<NetworkView>();
        audioSource = GetComponent<AudioSource>();

        if (!networkView.isMine)
            this.enabled = false;
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
            FireRay(inventory.currentWeapon.rayNum);
        } else {
            SpawnProjectile(GameManager.WeaponClassToWeaponId(inventory.currentWeapon), gunFirePoint.position, cameraAnchor.rotation, Network.player);
        }
        if (inventory.currentWeapon.hasRecoil) {
            motor.Recoil(inventory.currentWeapon.recoil);
        }

    }
	
	private void FireRay(int rayNum) {
        List<Vector3> endPoints = new List<Vector3>();
        List<Vector3> hitParticlePoints = new List<Vector3>();

        int i = 0;
        do {
            RaycastHit hit = new RaycastHit();
            //Find direction after shot spread
            Vector3 shotDir = cameraAnchor.forward;
            //Apply two rotations
            float angle1 = Random.Range(-inventory.currentWeapon.shotSpread, inventory.currentWeapon.shotSpread);
            float angle2 = Random.Range(-inventory.currentWeapon.shotSpread, inventory.currentWeapon.shotSpread);

            shotDir = Quaternion.AngleAxis(angle1, cameraAnchor.up) * Quaternion.AngleAxis(angle2, cameraAnchor.right) * shotDir;

            Physics.Raycast(cameraAnchor.position, shotDir, out hit, Mathf.Infinity);
            
            //Deal with the shot
            bool spawnHitParticle = false;
            Collider col = hit.collider;
            if (col.CompareTag("Player") || col.CompareTag("BonusPickup") || col.CompareTag("Grenade")) {
                spawnHitParticle = true;
                IDamageable damageable = hit.collider.gameObject.GetInterface<IDamageable>();
                if (damageable != null) {
                    damageable.TakeDamage(inventory.currentWeapon.damagePerShot, Network.player, GameManager.WeaponClassToWeaponId(inventory.currentWeapon));
                }
            }

            endPoints.Add(hit.point);
            if (spawnHitParticle) {
                hitParticlePoints.Add(hit.point);
            }
        } while (inventory.currentWeapon.rayNum > ++i);



        ShotRender(gunFirePoint.position, endPoints.ToArray(), hitParticlePoints.ToArray());

        //Render shot everywhere else
        //for (int j = 0; j < inventory.currentWeapon.rayNum; j++) {
        //    Vector3 particlePos = hitParticlePoints[j] != null ? hitParticlePoints[j] : Vector3.zero;

        //    networkView.RPC("NetworkShotRender", RPCMode.Others, GameManager.WeaponClassToWeaponId(inventory.currentWeapon), gunFirePoint.position, endPoints[j], particlePos);
        //}
        networkView.RPC("NetworkShotRender", RPCMode.Others, GameManager.WeaponClassToWeaponId(inventory.currentWeapon), gunFirePoint.position, endPoints.ToArray(), hitParticlePoints.ToArray());

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
    void NetworkShotRender(int weaponID, Vector3 start, Vector3[] endPoints, Vector3[] hitParticle) {
        WeaponSuperClass weapon = GameManager.weapon[weaponID];

        foreach (Vector3 hit in hitParticle) {
            Instantiate(weapon.hitParticle, hit, Quaternion.identity);
        }

        foreach (Vector3 end in endPoints) {
            GameObject shot = Instantiate(weapon.projectile, start, Quaternion.identity) as GameObject;
            LineRenderer render = shot.GetComponent<LineRenderer>();
            render.useWorldSpace = true;
            render.SetPosition(0, start);
            render.SetPosition(1, end);
        }
    }

    void ShotRender(Vector3 start, Vector3[] endPoints, Vector3[] hitParticle) {
        // Spawn hit particle
        foreach(Vector3 hit in hitParticle){
            Instantiate(inventory.currentWeapon.hitParticle, hit, Quaternion.identity);
        }

        // Render shot line

        // Modify start point once
        start = cameraAnchor.InverseTransformPoint(start);

        foreach (Vector3 end in endPoints) {
            GameObject shot = Instantiate(inventory.currentWeapon.projectile, cameraAnchor.position, cameraAnchor.rotation) as GameObject;
            shot.transform.parent = cameraAnchor;
            LineRenderer lineRenderer = shot.GetComponent<LineRenderer>();

            Vector3 newEnd = cameraAnchor.InverseTransformPoint(end);

            lineRenderer.useWorldSpace = false;
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, newEnd);
        }
    }
	
    void PlayFireWeaponSound() {
        if (networkView.isMine) {
            audioSource.PlayOneShot(inventory.currentWeapon.fireSound);

            networkView.RPC("RPCPlayFireWeaponSound", RPCMode.Others, GameManager.WeaponClassToWeaponId(inventory.currentWeapon));
        }
    }
    [RPC]
    void RPCPlayFireWeaponSound(int weaponID) {
        WeaponSuperClass weapon = GameManager.weapon[weaponID];
        audioSource.PlayOneShot(weapon.fireSound);
    }
}
