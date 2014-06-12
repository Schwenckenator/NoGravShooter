using UnityEngine;
using System.Collections;

public class PlayerResources : MonoBehaviour {
	#region Public Declarations
	public AudioClip soundOverheat;
	public AudioClip soundJetpackRecharge;

	public int maxFuel = 100;
	public int maxHealth = 100;
	public int maxWeapon = 100;

	public float fuelRecharge = 50;
	public float maxRechargeWaitTime = 1.0f;
	public float weaponRecharge = 30;
	public float weaponRechangeWaitTime = 2.0f;

	#endregion

	#region Private Declarations
	private GameManagerScript manager;
	private ParticleSystem smokeParticle;
	private AudioSource jetpackAudio;

	private int minFuel = 0;
	private int minHealth = 0;
	private int minWeapon = 0;
	
	private float fuel;
	private int health;
	private float weapon;
	private float rechargeWaitTime;

	private bool recharging = true;

	#endregion
	
	#region Start()
	void Start () {
		manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>();
		jetpackAudio = transform.FindChild("JetpackAudio").GetComponent<AudioSource>();
		smokeParticle = transform.FindChild("CameraPos").FindChild("GunSmokeParticle").GetComponent<ParticleSystem>();
		smokeParticle.emissionRate = 0;
		smokeParticle.Play();
		//Use them like percentage, for now

		fuel = maxFuel;
		health = maxHealth;
		rechargeWaitTime = 0;
	}
	#endregion

	#region Fixed Update()
	void FixedUpdate () {
		if(recharging){
			RechargeFuel(fuelRecharge);
		}
		RechargeWeapon(weaponRecharge);
		if(Input.GetKeyDown(KeyCode.K)){ //K is for kill!
			TakeDamage(10);
		}

		if(weapon > maxWeapon){
			smokeParticle.emissionRate = 10;
			smokeParticle.startColor = Color.black;
			
			
		}else if(weapon > maxWeapon*2/3){
			
			smokeParticle.emissionRate = 10;
			smokeParticle.GetComponent<ParticleSystem>().startColor = Color.grey;
		}else if(weapon > maxWeapon*1/3){
			
			smokeParticle.emissionRate = 10;
			smokeParticle.GetComponent<ParticleSystem>().startColor = Color.white;
		}else{
			smokeParticle.emissionRate = 0;
		}
	}
	#endregion

	#region Variable Accessors
	public int GetHealth(){
		return health;
	}
	public float GetFuel(){
		return fuel;
	}
	public float GetWeaponHeat(){
		return weapon;
	}
	#endregion

	#region Variable Mutators
	public bool SpendFuel(float spentFuel){
		recharging = true;
		rechargeWaitTime = maxRechargeWaitTime;
		fuel -= spentFuel;
		if(fuel < minFuel){
			fuel = minFuel;
			return false;
		}
		return true;
	}
	
	public void TakeDamage(int damage){
		networkView.RPC("TakeDamageRPC", RPCMode.All, damage);
	}

	public void RestoreHealth(int restore){
		health += restore;
		if(health > maxHealth){
			health = maxHealth;
		}
	}

	public void WeaponFired(float addedHeat){
		weapon += addedHeat;
		if(weapon > maxWeapon){
			//Gun overheated
			audio.PlayOneShot(soundOverheat);
			weapon = maxWeapon + (weaponRecharge * weaponRechangeWaitTime);
		}
	}
	#endregion

	#region Rechargers
	private void RechargeFuel(float charge){
		if(rechargeWaitTime > 0){
			rechargeWaitTime -= Time.deltaTime;
		}else{
			fuel += charge * Time.deltaTime;
			if(fuel > maxFuel){
				fuel = maxFuel;
				recharging = false;
				StartCoroutine("StopRechargeSound");
			}else{
				PlayRechargeSound();
			}
		}
	}
	private void RechargeWeapon(float charge){
		weapon -= charge * Time.deltaTime;
		if(weapon < minWeapon){
			weapon = minWeapon;
		}
	}
	#endregion

	#region RPC

	[RPC]
	void TakeDamageRPC(int damage){
		health -= damage;
		if(health <= minHealth){
			//You is dead nigs
			if(networkView.isMine){
				manager.PlayerDied();
				manager.ManagerDetachCamera();
				manager.CursorVisible(true);
			}
			Destroy(gameObject);
		}
	}
	#endregion

	#region Variable Checkers
	public bool IsFullHealth(){
		return health == maxHealth;
	}

	public bool WeaponCanFire(){
		return (weapon < maxWeapon);
	}
	#endregion
	
	#region JetpackSounds
	void PlayRechargeSound(){
		if(!jetpackAudio.isPlaying){
			jetpackAudio.volume = 0.5f;
			jetpackAudio.clip = soundJetpackRecharge;
			jetpackAudio.Play();
		}
	}
	IEnumerator StopRechargeSound(){

		while (jetpackAudio.volume > 0){
			if(jetpackAudio.clip == soundJetpackRecharge){
				jetpackAudio.volume /= 1+ (3* Time.deltaTime);
			}else{
				break;
			}

			yield return null;
		}
	}
	#endregion
}
