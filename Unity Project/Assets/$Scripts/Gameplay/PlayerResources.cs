using UnityEngine;
using System.Collections;

public class PlayerResources : MonoBehaviour {
	#region Public Declarations
	public AudioClip soundOverheat;
	public AudioClip soundJetpackRecharge;

	public int maxFuel = 100;
	public int maxHealth = 100;
	public int maxHeat = 100;

	public float fuelRecharge = 50;
	public float maxRechargeWaitTime = 1.0f;
	public float heatOverheat = 30;
	public float heatCooldownWaitTime = 2.0f;

	#endregion

	#region Private Declarations
	private GameManagerScript manager;
	private ParticleSystem smokeParticle;
	private AudioSource jetpackAudio;

	private int minFuel = 0;
	private int minHealth = 0;
	private int minHeat = 0;
	
	private float fuel;
	private int health;
	private float heat;
	private float rechargeWaitTime;

	private bool recharging = true;

	private IWeaponValues currentWeapon;
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
		if(Input.GetKeyDown(KeyCode.P)){ //Because fuck you it's P
			StartCoroutine("WeaponReload");
		}
		if(recharging){
			RechargeFuel(fuelRecharge);
		}
		RechargeWeapon(heatOverheat);
		if(Input.GetKeyDown(KeyCode.K)){ //K is for kill!
			TakeDamage(10);
		}

		if(heat > maxHeat){
			smokeParticle.emissionRate = 10;
			smokeParticle.startColor = Color.black;
			
			
		}else if(heat > maxHeat*2/3){
			
			smokeParticle.emissionRate = 10;
			smokeParticle.GetComponent<ParticleSystem>().startColor = Color.grey;
		}else if(heat > maxHeat*1/3){
			
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
		return heat;
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
		StopCoroutine("WeaponReload");
		audio.Stop();
		heat += addedHeat;
		currentWeapon.currentClip--;
		if(heat > maxHeat){
			//Gun overheated
			audio.PlayOneShot(soundOverheat);

			heat = maxHeat + (heatOverheat * heatCooldownWaitTime);
		}
		if(currentWeapon.currentClip <= 0){
			StartCoroutine("WeaponReload");
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
		heat -= charge * Time.deltaTime;
		if(heat < minHeat){
			heat = minHeat;
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
		return (heat < maxHeat) && (currentWeapon.currentClip > 0);
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

	IEnumerator WeaponReload(){
		audio.clip = currentWeapon.reloadSound;
		audio.Play ();
		float wait = 0;
		while(wait < currentWeapon.reloadTime){
			wait += Time.deltaTime;
			yield return null;
		}
		currentWeapon.currentClip = currentWeapon.clipSize;
	}

	public void ChangeWeapon(IWeaponValues newWeapon){
		currentWeapon = newWeapon;
		if(currentWeapon.currentClip == 0){
			StopCoroutine("WeaponReload");
			StartCoroutine("WeaponReload");
		}
	}

	public int GetCurrentClip(){
		return currentWeapon.currentClip;
	}
	public int GetMaxClip(){
		return currentWeapon.clipSize;
	}

}
