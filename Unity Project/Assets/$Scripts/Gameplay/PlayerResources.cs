using UnityEngine;
using System.Collections;

public class PlayerResources : MonoBehaviour {
	#region Public Declarations
	public AudioClip soundOverheat;
	public AudioClip soundJetpackRecharge;
	public AudioClip soundChangeWeapon;

	public AudioClip soundJetpackEmpty;
	public float volumeJetpackEmpty;
	public AudioClip soundJetpackShutoff;
	public float volumeJetpackShutoff;

	public int maxFuel = 150;
	public int maxHealth = 100;
	public int maxHeat = 100;

	public float fuelRecharge = 50;
	public float maxRechargeWaitTime = 1.0f;
	public float heatOverheat = 30;
	public float heatCooldownWaitTime = 2.0f;

	#endregion

	#region Private Declarations
	private GameManager manager;
	private ParticleSystem smokeParticle;
	private AudioSource jetpackAudio;

	private int minFuel = 0;
	private int minHealth = 0;
	private int minHeat = 0;
	
	private float fuel;
	private int health;
	private float heat;
	private float rechargeWaitTime;
	private int grenades;

	private bool recharging = true;
	private bool jetpackDisabled = false;
	private bool weaponBusy = false;

	private WeaponSuperClass currentWeapon;
	#endregion
	
	#region Start()
	void Start () {
		manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
		jetpackAudio = transform.FindChild("JetpackAudio").GetComponent<AudioSource>();
		//smokeParticle = transform.FindChild("CameraPos").FindChild("GunSmokeParticle").GetComponent<ParticleSystem>();
		smokeParticle = GetComponentInChildren<ParticleSystem>();
		smokeParticle.emissionRate = 0;
		smokeParticle.Play();
		//Use them like percentage, for now

		fuel = maxFuel;
		health = maxHealth;
		rechargeWaitTime = 0;
		grenades = 0;
	}
	#endregion

	#region Fixed Update()
	void FixedUpdate () {
		if(Input.GetKeyDown(GameManager.keyBindings[(int)GameManager.KeyBind.Reload])){ //Because fuck you it's P // nevermind
			StartCoroutine("WeaponReload");
		}
		if(recharging){
			RechargeFuel(fuelRecharge);
		}
		RechargeWeapon(heatOverheat);
		if(Input.GetKeyDown(KeyCode.K) && networkView.isMine){ //K is for kill! // This is for testing purposes only
			TakeDamage(100);
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
	public float GetMaxFuel(){
		return maxFuel;
	}
	public float GetFuel(){
		return fuel;
	}
	public float GetWeaponHeat(){
		return heat;
	}
	public int GetGrenades(){
		return grenades;
	}
	#endregion

	#region Variable Mutators
	// Checks itself to see if there is fuel available
	// Returns false if fuel empty
	public bool SpendFuel(float spentFuel){
		recharging = true;
		if(jetpackDisabled){
			return false;
		}
		fuel -= spentFuel;
		if(fuel < minFuel){
			fuel = minFuel;
			jetpackDisabled = true;
			rechargeWaitTime = maxRechargeWaitTime*2;
			StartCoroutine(PlayJetpackEmptySound());
			return false;
		}
		rechargeWaitTime = maxRechargeWaitTime;
		return true;
	}

	// Checks to see if there is grenades available
	// Returns false if no grenades
	public bool ThrowGrenade(){
		if(grenades > 0){
			grenades--;
			return true;
		}else{
			return false;
		}
	}

	public void PickUpGrenades(int amount){
		grenades += amount;
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
				jetpackDisabled = false;
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
				GetComponent<NoGravCharacterMotor>().Ragdoll(true);
				StartCoroutine(PlayerCleanup());
			}

		}
	}
	#endregion

	IEnumerator PlayerCleanup(){
		yield return new WaitForSeconds(3.0f);
		manager.PlayerDied();
		manager.ManagerDetachCamera();
		manager.CursorVisible(true);
		Network.Destroy(gameObject);
	}

	#region Variable Checkers
	public bool IsFullHealth(){
		return health == maxHealth;
	}

	public bool WeaponCanFire(){
		return (heat < maxHeat) && (currentWeapon.currentClip > 0) && !weaponBusy;
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
	IEnumerator PlayJetpackEmptySound(){

		jetpackAudio.clip = soundJetpackEmpty;
		jetpackAudio.Play();
		yield return new WaitForSeconds(1.25f);
		jetpackAudio.Stop();
		jetpackAudio.PlayOneShot(soundJetpackShutoff);
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
		// If no remaining ammo, don't attempt to reload
		if(currentWeapon.remainingAmmo <= 0){
			if(currentWeapon.currentClip <= 0){

				//GetComponent<FireWeapon>().removeWeapon(currentWeapon);
				//GetComponent<FireWeapon>().ChangeWeapon(0);
			}

			yield break;
		}


		if(currentWeapon.reloadTime > 1.0f){
			GetComponentInChildren<WeaponReloadRotation>().ReloadRotation(currentWeapon.reloadTime);
		}
		weaponBusy = true;
		audio.clip = currentWeapon.reloadSound;
		audio.Play ();
		float wait = 0;
		while(wait < currentWeapon.reloadTime){
			wait += Time.deltaTime;
			yield return null;
		}
		weaponBusy = false;
		int newBullets = currentWeapon.clipSize - currentWeapon.currentClip;
		currentWeapon.currentClip = Mathf.Min (currentWeapon.clipSize, currentWeapon.remainingAmmo+currentWeapon.currentClip);

		currentWeapon.remainingAmmo -= newBullets; 
		currentWeapon.remainingAmmo = Mathf.Max (currentWeapon.remainingAmmo, 0);
	}

	IEnumerator WeaponChange(){
		audio.PlayOneShot(soundChangeWeapon);
		weaponBusy = true;
		float waitTime = 1.0f;
		GetComponentInChildren<WeaponReloadRotation>().ReloadRotation(waitTime);
		yield return new WaitForSeconds(waitTime);
		weaponBusy = false;
	}

	public void ChangeWeapon(WeaponSuperClass newWeapon){
		if(!weaponBusy){
			StartCoroutine(WeaponChange());
			currentWeapon = newWeapon;
		}
	}

	public int GetCurrentClip(){
		return currentWeapon.currentClip;
	}
	public int GetMaxClip(){
		return currentWeapon.clipSize;
	}
	public int GetRemainingAmmo(){
		return currentWeapon.remainingAmmo;
	}

	public bool IsJetpackDisabled(){
		return jetpackDisabled;
	}

	public bool IsWeaponBusy(){
		return weaponBusy;
	}


}
