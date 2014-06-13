using UnityEngine;
using System.Collections;

public class WeaponValueInitialiser : MonoBehaviour {

	public struct Willy{
		int bum;
	}
	#region Laser
	LaserWeaponValues laser;
	public int laserDamagePerShot;
	public int laserHeatPerShot;
	public float laserFireDelay;
	public int laserClipSize;
	public float laserReloadTime;
	public GameObject laserProjectile;
	public GameObject laserParticle;
	public AudioClip laserFireSound;
	public AudioClip laserReloadSound = null;
	#endregion

	#region Slug Rifle
	SlugRifleWeaponValues slugRifle;
	public int slugRifleDamagePerShot;
	public int slugRifleHeatPerShot;
	public float slugRifleFireDelay;
	public int slugRifleClipSize;
	public float slugRifleReloadTime;
	public GameObject slugRifleProjectile;
	public GameObject slugRifleParticle;
	public AudioClip slugRifleFireSound;
	public AudioClip slugRifleReloadSound;
	#endregion

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(gameObject);

		LaserSetUp();
		SlugRifleSetUp();
	}

	#region Laser Set Up
	void LaserSetUp(){
		laser = ScriptableObject.CreateInstance<LaserWeaponValues>();

		laser.damagePerShot = laserDamagePerShot;
		laser.heatPerShot = laserHeatPerShot;
		laser.fireDelay = laserFireDelay;
		laser.clipSize = laserClipSize;
		laser.currentClip = laserClipSize;
		laser.reloadTime = laserReloadTime;

		laser.projectile = laserProjectile;
		laser.hitParticle = laserParticle;
		laser.fireSound = laserFireSound;
		laser.reloadSound = laserReloadSound;
	}
	#endregion

	#region Slug Rifle Set Up
	void SlugRifleSetUp(){
		slugRifle = ScriptableObject.CreateInstance<SlugRifleWeaponValues>();

		slugRifle.damagePerShot = slugRifleDamagePerShot;
		slugRifle.heatPerShot = slugRifleHeatPerShot;
		slugRifle.fireDelay = slugRifleFireDelay;
		slugRifle.clipSize = slugRifleClipSize;
		slugRifle.currentClip = slugRifleClipSize;
		slugRifle.reloadTime = slugRifleReloadTime;

		slugRifle.projectile = slugRifleProjectile;
		slugRifle.hitParticle = slugRifleParticle;
		slugRifle.fireSound = slugRifleFireSound;
		slugRifle.reloadSound = slugRifleReloadSound;
	}
	#endregion
}
