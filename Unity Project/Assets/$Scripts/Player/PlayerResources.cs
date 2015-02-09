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
    private AudioClip soundTakeDamage;

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

	private GameManager gameManager;
    private SettingsManager settingsManager;
    private ChatManager chatManager;
	private ParticleSystem smokeParticle;
	private AudioSource jetpackAudio;
	private AudioSource helmetAudio;

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

	private bool isRecharging = true;
	private bool isReloading = false;
	private bool isJetpackDisabled = false;
	private bool isWeaponBusy = false;
	private bool isDying = false;

	private WeaponSuperClass currentWeapon;
	#endregion
	
	#region Start()
	void Awake () {
        GameObject manager = GameObject.FindGameObjectWithTag("GameController");
		gameManager = manager.GetComponent<GameManager>();
        settingsManager = manager.GetComponent<SettingsManager>();
        chatManager = manager.GetComponent<ChatManager>();

		jetpackAudio = transform.FindChild("JetpackAudio").GetComponent<AudioSource>();
		helmetAudio = transform.FindChild("HelmetAudio").GetComponent<AudioSource>();
		smokeParticle = GetComponentInChildren<ParticleSystem>();
		smokeParticle.emissionRate = 0;
		smokeParticle.Play();

		fuel = maxFuel;
		health = maxHealth;
		rechargeWaitTime = 0;


		grenades = new int[grenadeTypes];
		for(int i=0; i<grenadeTypes; i++){
			grenades[i] = 0;
		}
	}
	#endregion

	void Update () {
        if (InputManager.GetKeyDown(SettingsManager.KeyBind.Reload) && !isReloading) {
			StartCoroutine("WeaponReload");
		}
		if(isRecharging){
			RechargeFuel(fuelRecharge);
		}
		RechargeWeapon(heatOverheat);
		if(Input.GetKeyDown(KeyCode.K) && networkView.isMine){ //K is for kill! // This is for testing purposes only
			TakeDamage(100, Network.player);
			chatManager.AddToChat("committed Seppuku!");
		}

        WeaponSmokeCheck();
	}

    private void WeaponSmokeCheck() {
        if (heat > maxHeat) {
            smokeParticle.emissionRate = 10;
            smokeParticle.startColor = Color.black;


        } else if (heat > maxHeat * 2 / 3) {

            smokeParticle.emissionRate = 10;
            smokeParticle.GetComponent<ParticleSystem>().startColor = Color.grey;
        } else if (heat > maxHeat * 1 / 3) {

            smokeParticle.emissionRate = 10;
            smokeParticle.GetComponent<ParticleSystem>().startColor = Color.white;
        } else {
            smokeParticle.emissionRate = 0;
        }
    }

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
    

    /// <summary>
    /// Checks itself to see if there is fuel available
	/// Returns false if fuel empty
    /// </summary>
    /// <param name="spentFuel"></param>
    /// <param name="forceSpend"></param>
    /// <returns></returns>
	public bool SpendFuel(float spentFuel, bool forceSpend = false){
		isRecharging = true;
		if(isJetpackDisabled && !forceSpend){
			return false;
		}
		fuel -= spentFuel;
		if(fuel < minFuel){
			fuel = minFuel;
			isJetpackDisabled = true;
			rechargeWaitTime = maxRechargeWaitTime*2;
			StartCoroutine(PlayJetpackEmptySound());
			return false;
		}
		rechargeWaitTime = maxRechargeWaitTime;
		return true;
	}

	// Checks to see if there is grenades available
	// Returns false if no grenades
	public bool CanThrowGrenade(){
		
        if(GameManager.testMode) grenades[currentGrenadeType]++;

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

    private void PlaySoundTakeDamage() {
        helmetAudio.PlayOneShot(soundTakeDamage);
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
			glitched = false;
			fuel += charge * Time.deltaTime;
			if(fuel > maxFuel){
				fuel = maxFuel;
				isRecharging = false;
				isJetpackDisabled = false;
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

		if(isDying) return; // Don't bother if you are already dying
        PlaySoundTakeDamage();
        health -= damage;
		if(health <= minHealth){
			health = minHealth;
			isDying = true;//You is dead nigs

			if(networkView.isMine){
				if(NetworkManager.connectedPlayers[fromPlayer] != null && weaponId != -1){
					if(fromPlayer != Network.player){
                        string killMessage = NetworkManager.connectedPlayers[fromPlayer] + KillMessageGenerator(weaponId) + settingsManager.PlayerName;
                        chatManager.AddToChat(killMessage, false);
						gameManager.GetComponent<ScoreVictoryManager>().KillScored(fromPlayer);
					} else {
                        string killMessage = NetworkManager.connectedPlayers[Network.player] + KillMessageGenerator(weaponId) + "themselves.";
                        chatManager.AddToChat(killMessage, false);
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
			return " forced ";
		case 5:
			return " exploded ";
		case 6:
			return " plasmered ";
		}

		return " killed ";

	}

	IEnumerator PlayerCleanup(){
        float playerDyingTime = 3.0f;
		yield return new WaitForSeconds(playerDyingTime);
		gameManager.PlayerDied();
		gameManager.ManagerDetachCamera();
		gameManager.SetCursorVisibility(true);
		GetComponent<ObjectCleanUp>().ClientKillMe();
	}

	#region Variable Checkers
	public bool IsFullHealth(){
		return health == maxHealth;
	}

	public bool WeaponCanFire(){
		return (heat < maxHeat) && (currentWeapon.currentClip > 0) && !isWeaponBusy;
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
		
		// If no remaining ammo and NOT testmode, don't attempt to reload
		if(currentWeapon.remainingAmmo <= 0 && !GameManager.testMode){
			if(currentWeapon.currentClip <= 0){

				//GetComponent<FireWeapon>().removeWeapon(currentWeapon);
				//GetComponent<FireWeapon>().ChangeWeapon(0);
			}

			yield break;
		}

        isReloading = true;
		if(currentWeapon.reloadTime > 1.0f){
			GetComponentInChildren<WeaponReloadRotation>().ReloadRotation(currentWeapon.reloadTime);
		}
		isWeaponBusy = true;
		audio.clip = currentWeapon.reloadSound;
		audio.Play ();
		float wait = 0;
		while(wait < currentWeapon.reloadTime){
			wait += Time.deltaTime;
			yield return null;
		}
		isWeaponBusy = false;
		int newBullets = currentWeapon.clipSize - currentWeapon.currentClip;
		if(GameManager.testMode){
			currentWeapon.currentClip = currentWeapon.clipSize;
		}else{
			currentWeapon.currentClip = Mathf.Min (currentWeapon.clipSize, currentWeapon.remainingAmmo+currentWeapon.currentClip);
		}

		currentWeapon.remainingAmmo -= newBullets; 
		currentWeapon.remainingAmmo = Mathf.Max (currentWeapon.remainingAmmo, 0);
		isReloading = false;
	}

	IEnumerator WeaponChange(){
		audio.PlayOneShot(soundChangeWeapon);
		isWeaponBusy = true;
		float waitTime = 1.0f;
		GetComponentInChildren<WeaponReloadRotation>().ReloadRotation(waitTime, currentWeapon);
		yield return new WaitForSeconds(waitTime);
		isWeaponBusy = false;
	}

	public void ChangeWeapon(WeaponSuperClass newWeapon){
		if(!isWeaponBusy){
			currentWeapon = newWeapon;
			StartCoroutine(WeaponChange());
			heat = 0;
		}
	}

	public void ChangeGrenade(){
		currentGrenadeType++;
		currentGrenadeType %= grenadeTypes; // Keep value within range
	}
	
	public void ChangeGrenade(int typeOfGrenade){
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
		return isJetpackDisabled;
	}

	public bool IsWeaponBusy(){
		return isWeaponBusy;
	}

	
	public void EMPglitch(){
		glitched = true;
	}
	private bool glitched = false;
	void OnGUI(){
		if(!glitched) return;
		if(!GameManager.IsPlayerMenu() && networkView.isMine){
			GUI.depth = 0;
			GUI.DrawTexture(new Rect(20+Random.Range(-10, 11), Screen.height-240+Random.Range(-10, 11), 220, 220), gameManager.GetComponent<GuiManager>().EMPradar[Random.Range(0, 5)]);
			GUI.DrawTexture(new Rect(Screen.width/2 - 55/2, Screen.height/2 - 45/2, 55, 45), gameManager.GetComponent<GuiManager>().EMPcursor[Random.Range(0, 4)]);
			GUI.DrawTexture(new Rect(Screen.width-265, Screen.height-235+Random.Range(-5, 6), 265, 235), gameManager.GetComponent<GuiManager>().EMPstats[Random.Range(0, 4)]);
		}
	}

}
