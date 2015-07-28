using UnityEngine;
using System.Collections;

public class PlayerResources : MonoBehaviour {
	#region Declarations
    
    [SerializeField]
    private AudioClip soundJetpackRecharge;

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
    private float fuelRecharge = 50;
    [SerializeField]
    private float maxRechargeWaitTime = 1.0f;

	private AudioSource jetpackAudio;

	private int minFuel = 0;
	
	private float fuel;
	private float rechargeWaitTime;

	private int[] grenades;				        // Id is alphabetical
	private int grenadeTypes = 3; 		        // Black Hole, EMP, Frag
	private static int currentGrenadeType = 0;	//      0       1     2

	private bool isRecharging = true;
	private bool isJetpackDisabled = false;
	#endregion
	
	#region Start()
    NetworkView networkView;
	void Awake () {

        networkView = GetComponent<NetworkView>();
		jetpackAudio = transform.FindChild("JetpackAudio").GetComponent<AudioSource>();

		fuel = maxFuel;
		rechargeWaitTime = 0;

		grenades = new int[grenadeTypes];
		for(int i=0; i<grenadeTypes; i++){
			grenades[i] = 0;
		}

        if (!networkView.isMine) {
            this.enabled = false;
        }
	}
	#endregion

	void Update () {
        if (InputConverter.GetKeyDown(KeyBind.GrenadeSwitch)) {
            ChangeGrenade();
        }
		if(isRecharging){
			RechargeFuel(fuelRecharge);
		}
	}

	#region Variable Accessors
	public static float GetMaxFuel(){
		return maxFuel;
	}
	public float GetFuel(){
		return fuel;
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
            DisableJetpack(true);
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
                DisableJetpack(false);
				StartCoroutine("StopRechargeSound");
			}else{
				PlayRechargeSound();
			}
		}
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
	public bool IsJetpackDisabled(){
		return isJetpackDisabled;
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
            GUI.DrawTexture(new Rect(Screen.width - 330, Screen.height - 285 + Random.Range(-5, 6), 330, 285), GuiManager.instance.EMPstats[Random.Range(0, 4)]);
		}
	}

    void DisableJetpack(bool disable) {
        isJetpackDisabled = disable;
        UIPlayerHUD.JetpackDisabled(disable);
    }

}
