using UnityEngine;
using System.Collections;

public class WeaponValueInitialiser : MonoBehaviour {
	#region Laser
	LaserWeaponValues laser;
	public int laser_DamagePerShot;
	public int laser_HeatPerShot;
	public float laser_FireDelay;
	public GameObject laser_Projectile;
	public GameObject laser_Particle;
	public AudioClip laser_FireSound;
	#endregion

	#region Slug Rifle
	SlugRifleWeaponValues slugRifle;
	public int slugRifle_DamagePerShot;
	public int slugRifle_HeatPerShot;
	public float slugRifle_FireDelay;
	public GameObject slugRifle_Projectile;
	public GameObject slugRifle_Particle;
	public AudioClip slugRifle_FireSound;
	#endregion

	// Use this for initialization
	void Start () {
		LaserSetUp();
		SlugRifleSetUp();
	}

	#region Laser Set Up
	void LaserSetUp(){
		laser = ScriptableObject.CreateInstance<LaserWeaponValues>();
		laser.DamagePerShot = laser_DamagePerShot;
		laser.HeatPerShot = laser_HeatPerShot;
		laser.FireDelay = laser_FireDelay;
		laser.Projectile = laser_Projectile;
		laser.HitParticle = laser_Particle;
		laser.FireSound = laser_FireSound;
	}
	#endregion

	#region Slug Rifle Set Up
	void SlugRifleSetUp(){
		slugRifle = ScriptableObject.CreateInstance<SlugRifleWeaponValues>();
		slugRifle.DamagePerShot = slugRifle_DamagePerShot;
		slugRifle.HeatPerShot = slugRifle_HeatPerShot;
		slugRifle.FireDelay = slugRifle_FireDelay;
		slugRifle.Projectile = slugRifle_Projectile;
		slugRifle.HitParticle = slugRifle_Particle;
		slugRifle.FireSound = slugRifle_FireSound;
	}
	#endregion
}
