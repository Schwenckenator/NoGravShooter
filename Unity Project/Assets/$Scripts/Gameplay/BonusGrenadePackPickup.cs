using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ObjectCleanUp))]
public class BonusGrenadePackPickup : MonoBehaviour {
    private SettingsManager settingsManager;

    [SerializeField]
	private int amount;
    [SerializeField]
	private int grenadeType;
	private int autoPickup = 0;

	void Start(){
        settingsManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<SettingsManager>();

		//Randomise here, just for now
		grenadeType = Random.Range(0, 3);
		Debug.Log (grenadeType.ToString());
	}

	void OnTriggerEnter(Collider info){
		if(info.CompareTag("Player")){
			
			PlayerResources playerResource = info.collider.GetComponent<PlayerResources>();

            autoPickup = settingsManager.AutoPickup;

			if(autoPickup == 1){
				playerResource.ChangeGrenadeTypeTo(grenadeType);
			}
			playerResource.PickUpGrenades(amount, grenadeType);
			
			GetComponent<ObjectCleanUp>().KillMe();
		}
	}
}
