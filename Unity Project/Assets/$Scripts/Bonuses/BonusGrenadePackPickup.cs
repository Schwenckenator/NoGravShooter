using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ObjectCleanUp))]
public class BonusGrenadePackPickup : MonoBehaviour {

    [SerializeField]
	private int amount;
    [SerializeField]
	private int grenadeType;

	void Start(){

        if (GameManager.IsTutorialScene()) {
            grenadeType = 0;
        } else {
            //Randomise here, just for now
            grenadeType = Random.Range(0, 3);
        }
	}

	void OnTriggerEnter(Collider info){
		if(info.CompareTag("Player")){
			
			PlayerResources playerResource = info.collider.GetComponent<PlayerResources>();

			if(SettingsManager.instance.AutoPickup){
				playerResource.ChangeGrenade(grenadeType);
			}
			playerResource.PickUpGrenades(amount, grenadeType);
			
			GetComponent<ObjectCleanUp>().KillMe();
		}
	}
}
