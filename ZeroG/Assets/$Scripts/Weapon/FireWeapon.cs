using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class FireWeapon : NetworkBehaviour {
    public GameObject paintSplat;
	public Transform gunFirePoint;
	public Transform cameraAnchor;
	ActorMotorManager motor;
	
    WeaponInventory inventory;
    WeaponResources weaponResources;

    public AudioSource audioSource; // Assign in inspector

    public Player thisPlayer;
	// Use this for initialization
	void Awake () {
		motor = GetComponent<ActorMotorManager>();
        inventory = GetComponent<WeaponInventory>();
        weaponResources = GetComponent<WeaponResources>();
        thisPlayer = GetComponent<Player>();
	}
	void Update(){
        if (!isLocalPlayer) return;
        CheckForFire(WeaponType.Projectile);
        // Check for dry fire
        if (IsWantToFire() && !inventory.HasNoWeapons() && inventory.currentWeapon.isEmpty) {
           weaponResources.SafeStartReload();
        }
	}

    void FixedUpdate() {
        if (!isLocalPlayer) return;
        CheckForFire(WeaponType.Ray);
    }

    // Check for fire
    void CheckForFire(WeaponType type) {
        if (inventory.HasNoWeapons()) return;
        if (inventory.currentWeapon.type == type && IsWantToFire() && CanWeaponFire()) {
            if (inventory.currentWeapon.TryFire()) {
                WeaponFired();
            }
        }
    }

    private bool IsWantToFire() {
        return (Input.GetAxisRaw("Fire1") > 0);
    }
    private bool CanWeaponFire() {
        return weaponResources.WeaponCanFire() && !WeaponInventory.isChanging && !UIPauseSpawn.IsShown;
    }

    private void WeaponFired() {
        // Play sounds
        PlayFireWeaponSound();

        if (inventory.currentWeapon.type == WeaponType.Ray) { //Does this weapon use a ray to hit?
            FireRay(inventory.currentWeapon.rayNum);
        } else {
            CmdSpawnProjectile(inventory.currentWeapon.id, cameraAnchor.rotation);
        }
        inventory.currentWeapon.AfterFire();
        weaponResources.WeaponFired();
        if (inventory.currentWeapon.hasRecoil) {
            motor.Recoil(inventory.currentWeapon.recoil);
        }
    }
	
	private void FireRay(int rayNum) {
        List<Vector3> dirs = new List<Vector3>();

        int i = 0;
        do {
            //RaycastHit hit = new RaycastHit();
            //Find direction after shot spread
            Vector3 shotDir = cameraAnchor.forward;
            //Apply two rotations
            Vector2 angle = Random.insideUnitCircle * inventory.currentWeapon.shotSpread;

            shotDir = Quaternion.AngleAxis(angle.x, cameraAnchor.up) * Quaternion.AngleAxis(angle.y, cameraAnchor.right) * shotDir;
            dirs.Add(shotDir);
            //if (DebugManager.paintballMode){
            //    Instantiate(paintSplat, hit.point, Quaternion.identity);
            //}

        } while (inventory.currentWeapon.rayNum > ++i);

        CmdFireRays(inventory.currentWeapon.id, dirs.ToArray());
    }

    [Command]
    void CmdFireRays(int weaponID, Vector3[] dirs) {
        StartCoroutine(FixedFireRay(weaponID, dirs));
    }

    IEnumerator FixedFireRay(int weaponID, Vector3[] dirs) {
        yield return new WaitForFixedUpdate();
        Weapon weapon = WeaponManager.weapon[weaponID];
        List<Vector3> endPoints = new List<Vector3>();
        List<Vector3> spawnHitParticlePoints = new List<Vector3>();

        foreach (Vector3 dir in dirs) {
            RaycastHit hit = new RaycastHit();
            Physics.Raycast(cameraAnchor.position, dir, out hit, Mathf.Infinity);
            endPoints.Add(hit.point);
            Collider col = hit.collider;
            if (col.CompareTag("Player") || col.CompareTag("BonusPickup") || col.CompareTag("Grenade")) {
                spawnHitParticlePoints.Add(hit.point);
                IDamageable damageable = hit.collider.gameObject.GetInterface<IDamageable>();
                if (damageable != null) {
                    damageable.TakeDamage(weapon.damage, thisPlayer, weapon.id);
                }
            }
        }
        RenderRays(weaponID, gunFirePoint.position, endPoints.ToArray()); // Render on server
        RpcRenderRays(weaponID, gunFirePoint.position, endPoints.ToArray()); // Render everywhere else
    }

    void RenderRays(int weaponID, Vector3 start, Vector3[] ends) {
        Weapon weapon = WeaponManager.weapon[weaponID];
        if (isLocalPlayer) {
            ShotRender(gunFirePoint.position, ends); // Local render
        } else {
            foreach (Vector3 end in ends) { // Other's render
                GameObject shot = Instantiate(weapon.projectile, start, Quaternion.identity) as GameObject;
                LineRenderer render = shot.GetComponent<LineRenderer>();
                render.useWorldSpace = true;
                render.SetPosition(0, start);
                render.SetPosition(1, end);
            }
        }
    }

    [ClientRpc]
    void RpcRenderRays(int weaponID, Vector3 start, Vector3[] ends) {
        RenderRays(weaponID, start, ends);
    }

    [Command]
    void CmdSpawnProjectile(int weaponID, Quaternion rotation) {
        GameObject newObj = Network.Instantiate(WeaponManager.weapon[weaponID].projectile, gunFirePoint.position, rotation, 0) as GameObject;
        NetworkServer.Spawn(newObj);
    }

    [ClientRpc]
    void RpcSpawnHitParticles(int weaponID, Vector3[] positions) {
        Weapon weapon = WeaponManager.weapon[weaponID];
        foreach(Vector3 pos in positions) {
            Instantiate(weapon.hitParticle, pos, Quaternion.identity);
        }
    }

    void ShotRender(Vector3 start, Vector3[] endPoints) {
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
        if (isLocalPlayer) {
            audioSource.PlayOneShot(inventory.currentWeapon.fireSound);
        }
    }
    [ClientRpc]
    void RpcPlayFireWeaponSound(int weaponID) {
        if (isLocalPlayer) return;
        Weapon weapon = WeaponManager.weapon[weaponID];
        audioSource.PlayOneShot(weapon.fireSound);
    }
}
