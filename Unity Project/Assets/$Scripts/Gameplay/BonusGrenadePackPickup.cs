using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ObjectCleanUp))]
public class BonusGrenadePackPickup : MonoBehaviour {
    [SerializeField]
	private int amount;
    [SerializeField]
	private int grenadeType;

	void Start(){
		//Randomise here, just for now
		grenadeType = Random.Range(0, 2);
		Debug.Log (grenadeType.ToString());
	}

	void OnTriggerEnter(Collider info){
		if(info.CompareTag("Player")){
			
			PlayerResources res = info.collider.GetComponent<PlayerResources>();

			res.PickUpGrenades(amount, grenadeType);
			GetComponent<ObjectCleanUp>().KillMe();
		}
	}
}
