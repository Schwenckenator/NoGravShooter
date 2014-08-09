using UnityEngine;
using System.Collections;

public class WeaponValueInitialiser : MonoBehaviour {
	

	public const int numWeapon = 7;

	public bool[] useRay;
	public bool[] hasRecoil;
	public float[] recoil;
	public int[] rayNum;
	public float[] shotSpread;

	public int[] damagePerShot;
	public int[] heatPerShot;
	public float[] fireDelay;
	public int[] clipSize;
	public float[] reloadTime;
	public GameObject[] projectile;
	public GameObject[] particle;
	public AudioClip[] fireSound;
	public AudioClip[] reloadSound;


	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(gameObject);

		SetUp();
	}

	void SetUp(){
//		IWeaponValues[] weapon = {
//
//			ScriptableObject.CreateInstance<LaserRifleValues>(), 
//			ScriptableObject.CreateInstance<SlugRifleWeaponValues>(), 
//			ScriptableObject.CreateInstance<LaserSniperValues>(),
//
//			ScriptableObject.CreateInstance<ShotgunValues>(), 
//
//			ScriptableObject.CreateInstance<ForceShotgunValues>(),
//			ScriptableObject.CreateInstance<RocketLauncherValues>(),
//			ScriptableObject.CreateInstance<PlasmaBlasterValues>()
//
//		};


		for(int i=0; i< numWeapon; i++){
			GameManagerScript.weapon[i].clipSize = clipSize[i];
			GameManagerScript.weapon[i].damagePerShot = damagePerShot[i];
			GameManagerScript.weapon[i].fireDelay = fireDelay[i];
			GameManagerScript.weapon[i].fireSound = fireSound[i];
			GameManagerScript.weapon[i].hasRecoil = hasRecoil[i];
			GameManagerScript.weapon[i].heatPerShot = heatPerShot[i];
			GameManagerScript.weapon[i].hitParticle = particle[i];
			GameManagerScript.weapon[i].projectile = projectile[i];
			GameManagerScript.weapon[i].rayNum = rayNum[i];
			GameManagerScript.weapon[i].recoil = recoil[i];
			GameManagerScript.weapon[i].reloadSound = reloadSound[i];
			GameManagerScript.weapon[i].reloadTime = reloadTime[i];
			GameManagerScript.weapon[i].shotSpread = shotSpread[i];
			GameManagerScript.weapon[i].useRay = useRay[i];
		}
	}
}
