using UnityEngine;
using System.Collections;

public class PlayerResources : MonoBehaviour {
	private GameManagerScript manager;
	
	private ParticleSystem smokeParticle;



	public AudioClip soundOverheat;
	public AudioClip soundJetpackRecharge;

	private AudioSource jetpackAudio;

	public int maxFuel = 100;
	public int minFuel = 0;
	public int maxHealth = 100;
	public int minHealth = 0;
	public int maxWeapon = 100;
	private int minWeapon = 0;

	private float fuel;
	private int health;
	private float weapon;

	private bool recharging = true;

	public float fuelRecharge = 50;
	public float maxRechargeWaitTime = 1.0f;
	public float weaponRecharge = 30;
	public float weaponRechangeWaitTime = 2.0f;

	private float rechargeWaitTime;
	// Use this for initialization
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

	// Update is called once per frame
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
	public int GetHealth(){
		return health;
	}

	public float GetFuel(){
		return fuel;
	}
	public float GetWeaponHeat(){
		return weapon;
	}

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

	[RPC]
	void TakeDamageRPC(int damage){
		health -= damage;
		if(health <= 0){
			//You is dead nigs
			if(networkView.isMine){
				manager.PlayerDied();
				manager.ManagerDetachCamera();
				manager.CursorVisible(true);
			}
			Destroy(gameObject);
		}
	}

	public void RestoreHealth(int restore){
		health += restore;
		if(health > maxHealth){
			health = maxHealth;
		}
	}

	public bool WeaponCanFire(){
		return (weapon < maxWeapon);
	}

	public void WeaponFired(float addedHeat){
		weapon += addedHeat;
		if(weapon > maxWeapon){
			//Gun overheated
			audio.PlayOneShot(soundOverheat);
			weapon = maxWeapon + (weaponRecharge * weaponRechangeWaitTime);
		}
	}

	void PlayRechargeSound(){
		if(!jetpackAudio.isPlaying){
			jetpackAudio.volume = 0.25f;
			jetpackAudio.clip = soundJetpackRecharge;
			jetpackAudio.Play();
		}
	}
	IEnumerator StopRechargeSound(){

		while (jetpackAudio.volume > 0){
			if(jetpackAudio.clip == soundJetpackRecharge){
				jetpackAudio.volume -= Time.deltaTime * 0.4f;
			}else{
				break;
			}

			yield return null;
		}
	}
}
