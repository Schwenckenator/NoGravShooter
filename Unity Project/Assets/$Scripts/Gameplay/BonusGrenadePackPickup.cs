using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ObjectCleanUp))]
public class BonusGrenadePackPickup : MonoBehaviour {
    [SerializeField]
	private int amount;
    [SerializeField]
	private int grenadeType;
	private int autoPickup = 0;

	void Start(){
		//Randomise here, just for now
		grenadeType = Random.Range(0, 3);
		Debug.Log (grenadeType.ToString());
	}

	void OnTriggerEnter(Collider info){
		if(info.CompareTag("Player")){
			
			PlayerResources res = info.collider.GetComponent<PlayerResources>();

			autoPickup = PlayerPrefs.GetInt("autoPickup", 0);
			if(autoPickup == 1){
				res.ChangeGrenadeTypeTo(grenadeType);
			}
			res.PickUpGrenades(amount, grenadeType);
			
			GetComponent<ObjectCleanUp>().KillMe();
		}
	}
}
