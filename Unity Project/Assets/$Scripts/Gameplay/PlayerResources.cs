using UnityEngine;
using System.Collections;

public class PlayerResources : MonoBehaviour {
	#region Declarations
    
    [SerializeField]
	private AudioClip soundOverheat;
    [SerializeField]
    private AudioClip soundJetpackRecharge;
    [SerializeField]
    private AudioClip soundChangeWeapon;

    [SerializeField]
    private AudioClip soundJetpackEmpty;
    [SerializeField]
    private float volumeJetpackEmpty;
    [SerializeField]
    private AudioClip soundJetpackShutoff;
    [SerializeField]
    private float volumeJetpackShutoff;

    [SerializeField]
    private int maxFuel = 150;
    [SerializeField]
    private int maxHealth = 100;
    [SerializeField]
    private int maxHeat = 100;

    [SerializeField]
    private float fuelRecharge = 50;
    [SerializeField]
    private float maxRechargeWaitTime = 1.0f;
    [SerializeField]
    private float heatOverheat = 30;
    [SerializeField]
    private float heatCooldownWaitTime = 2.0f;

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

	private int[] grenades;				// Id is alphabetical
	private int grenadeTypes = 3; 		// Black Hole, EMP, Frag
	private int currentGrenadeType = 0;	//      0       1     2

	private bool recharging = true;
	private bool reloading = false;
	private bool jetpackDisabled = false;
	private bool weaponBusy = false;
	private bool dying = false;

	private WeaponSuperClass currentWeapon;
	#endregion
	
	#region Start()
	void Awake () {
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


		grenades = new int[grenadeTypes];
		for(int i=0; i<grenadeTypes; i++){
			grenades[i] = 0;
		}
	}
	#endregion

	#region Fixed Update()
	void Update () {
		if(Input.GetKeyDown(GameManager.keyBindings[(int)GameManager.KeyBind.Reload]) && !reloading){
			StartCoroutine("WeaponReload");
		}
		if(recharging){
			RechargeFuel(fuelRecharge);
		}
		RechargeWeapon(heatOverheat);
		if(Input.GetKeyDown(KeyCode.K) && networkView.isMine){ //K is for kill! // This is for testing purposes only
			TakeDamage(100, Network.player);
			manager.AddToChat("committed Seppuku!");
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


	public int GetCurrentGrenadeCount(){
		return grenades[currentGrenadeType];
	}
	public int GetCurrentGrenadeType(){
		return currentGrenadeType;
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
		if(GameManager.testMode){
			grenades[currentGrenadeType]++;
		}
		if(grenades[currentGrenadeType] > 0){
			grenades[currentGrenadeType]--;
			return true;
		}else{
			return false;
		}
	}

	public void PickUpGrenades(int amount, int grenadeId){
		grenades[grenadeId] += amount;
	}
	
	public void TakeDamage(int damage, NetworkPlayer fromPlayer, int weaponId = -1){
		networkView.RPC("TakeDamageRPC", RPCMode.All, damage, fromPlayer, weaponId);
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
	void TakeDamageRPC(int damage, NetworkPlayer fromPlayer, int weaponId){

		if(dying) return; // Don't bother if you are already dying

		health -= damage;
		if(health <= minHealth){
			health = minHealth;
			dying = true;//You is dead nigs

			if(networkView.isMine){
				if(GameManager.connectedPlayers[fromPlayer] != null && weaponId != -1){
					if(fromPlayer != Network.player){
						string killMessage = GameManager.connectedPlayers[fromPlayer] + KillMessageGenerator(weaponId) + manager.CurrentPlayerName;
						manager.AddToChat(killMessage, false);
						manager.GetComponent<ScoreAndVictoryTracker>().KillScored(fromPlayer);
					} else {
						string killMessage = GameManager.connectedPlayers[Network.player] + KillMessageGenerator(weaponId) + "themselves.";
						manager.AddToChat(killMessage, false);
					}
				}
				GetComponent<NoGravCharacterMotor>().Ragdoll(true);
				StartCoroutine(PlayerCleanup());
			}

		}
	}
	#endregion

	string KillMessageGenerator(int weaponId){
		switch(weaponId){
		case 0:
			return " lasered ";
		case 1:
			return " shot ";
		case 2:
			return " sniped ";
		case 3:
			return " shotgunned ";
		case 4:
			return " forced? ";
		case 5:
			return " exploded? ";
		case 6:
			return " plasmered? ";
		}

		return " killed ";

	}

	IEnumerator PlayerCleanup(){
		yield return new WaitForSeconds(3.0f);
		manager.PlayerDied();
		manager.ManagerDetachCamera();
		manager.CursorVisible(true);
		GetComponent<ObjectCleanUp>().KillMe();
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
		reloading = true;
		// If no remaining ammo and NOT testmode, don't attempt to reload
		if(currentWeapon.remainingAmmo <= 0 && !GameManager.testMode){
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
		if(GameManager.testMode){
			currentWeapon.currentClip = currentWeapon.clipSize;
		}else{
			currentWeapon.currentClip = Mathf.Min (currentWeapon.clipSize, currentWeapon.remainingAmmo+currentWeapon.currentClip);
		}

		currentWeapon.remainingAmmo -= newBullets; 
		currentWeapon.remainingAmmo = Mathf.Max (currentWeapon.remainingAmmo, 0);
		reloading = false;
	}

	IEnumerator WeaponChange(){
		audio.PlayOneShot(soundChangeWeapon);
		weaponBusy = true;
		float waitTime = 1.0f;
		GetComponentInChildren<WeaponReloadRotation>().ReloadRotation(waitTime, currentWeapon);
		yield return new WaitForSeconds(waitTime);
		weaponBusy = false;
	}

	public void ChangeWeapon(WeaponSuperClass newWeapon){
		if(!weaponBusy){
			currentWeapon = newWeapon;
			StartCoroutine(WeaponChange());
			heat = 0;
		}
	}

	public void ChangeGrenade(){
		currentGrenadeType++;
		currentGrenadeType %= grenadeTypes; // Keep value within range
	}
	
	public void ChangeGrenadeTypeTo(int typeOfGrenade){
		currentGrenadeType = typeOfGrenade;
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
