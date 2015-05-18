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
    private static int maxFuel = 150;
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

	private ParticleSystem smokeParticle;
	private AudioSource jetpackAudio;

	private int minFuel = 0;
	private int minHeat = 0;
	
	private float fuel;
	private float heat;
	private float rechargeWaitTime;

	private int[] grenades;				        // Id is alphabetical
	private int grenadeTypes = 3; 		        // Black Hole, EMP, Frag
	private static int currentGrenadeType = 0;	//      0       1     2

	private bool isRecharging = true;
	private bool isReloading = false;
	private bool isJetpackDisabled = false;
	private bool isWeaponBusy = false;

	private WeaponSuperClass currentWeapon;
    private WeaponReloadRotation weaponReloadRotation;
	#endregion
	
	#region Start()
	void Awake () {
		jetpackAudio = transform.FindChild("JetpackAudio").GetComponent<AudioSource>();
        weaponReloadRotation = GetComponentInChildren<WeaponReloadRotation>();
		smokeParticle = GetComponentInChildren<ParticleSystem>();
		smokeParticle.emissionRate = 0;
		smokeParticle.Play();

		fuel = maxFuel;
		rechargeWaitTime = 0;


		grenades = new int[grenadeTypes];
		for(int i=0; i<grenadeTypes; i++){
			grenades[i] = 0;
		}
	}
	#endregion

	void Update () {
        if (InputConverter.GetKeyDown(KeyBind.Reload) && !isReloading && !isWeaponFull()){
            StartCoroutine("WeaponReload");
		}
        if (InputConverter.GetKeyDown(KeyBind.GrenadeSwitch)) {
            ChangeGrenade();
        }
		if(isRecharging){
			RechargeFuel(fuelRecharge);
		}
		RechargeWeapon(heatOverheat);

        WeaponSmokeCheck();
	}

    private bool isWeaponFull() {
        return GetCurrentClip() == GetMaxClip();
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
	public static float GetMaxFuel(){
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

        if (DebugManager.IsAllGrenade()) grenades[currentGrenadeType]++;

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
    

	#region Variable Checkers

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

    public void SafeStartReload() {
        if (!isWeaponBusy && !isReloading && !isWeaponFull()) {
            StartCoroutine("WeaponReload");
        }
    }

	IEnumerator WeaponReload(){
		
		// If no remaining ammo and NOT testmode, don't attempt to reload
        if (currentWeapon.remainingAmmo <= 0 && !DebugManager.IsAllAmmo()) {
			if(currentWeapon.currentClip <= 0){

				//GetComponent<FireWeapon>().removeWeapon(currentWeapon);
				//GetComponent<FireWeapon>().ChangeWeapon(0);
			}

			yield break;
		}

        isReloading = true;
		if(currentWeapon.reloadTime > 1.0f){
            weaponReloadRotation.ReloadRotation(currentWeapon.reloadTime);
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
        if (DebugManager.IsAllAmmo()) {
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
        weaponReloadRotation.ReloadRotation(waitTime, currentWeapon);
        networkView.RPC("NetworkWeaponChange", RPCMode.Others, waitTime, GameManager.WeaponClassToWeaponId(currentWeapon));
		yield return new WaitForSeconds(waitTime);
		isWeaponBusy = false;
	}
    [RPC]
    void NetworkWeaponChange(float waitTime, int currentWeaponID) {
        weaponReloadRotation.ReloadRotation(waitTime, GameManager.weapon[currentWeaponID]);
    }
	public void ChangeWeapon(WeaponSuperClass newWeapon){
        if (newWeapon == null) {
            currentWeapon = null;
            return;
        }
		if(!isWeaponBusy){
			currentWeapon = newWeapon;
			StartCoroutine(WeaponChange());
			heat = 0;
		}
	}
	public void ChangeGrenade(){
		currentGrenadeType++;
		currentGrenadeType %= grenadeTypes;
		for(int i = 0; i < grenadeTypes; i++){
			if(grenades[currentGrenadeType] == 0){
				currentGrenadeType++;
				currentGrenadeType %= grenadeTypes;
			}
		}
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
    public bool IsHoldingWeapon() {
        return currentWeapon != null;
    }
	
	public void EMPglitch(){
		glitched = true;
	}
	private bool glitched = false;
	void OnGUI(){
		if(!glitched) return;
		if(!GameManager.IsPlayerMenu() && networkView.isMine){
			GUI.depth = 0;
            GUI.DrawTexture(new Rect(20 + Random.Range(-10, 11), Screen.height - 240 + Random.Range(-10, 11), 220, 220), GuiManager.instance.EMPradar[Random.Range(0, 5)]);
            GUI.DrawTexture(new Rect(Screen.width / 2 - 55 / 2, Screen.height / 2 - 45 / 2, 55, 45), GuiManager.instance.EMPcursor[Random.Range(0, 4)]);
            GUI.DrawTexture(new Rect(Screen.width - 265, Screen.height - 235 + Random.Range(-5, 6), 265, 235), GuiManager.instance.EMPstats[Random.Range(0, 4)]);
		}
	}

}
