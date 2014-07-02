using UnityEngine;
using System.Collections;

public class BonusBlackHoleMinePackPickup : MonoBehaviour {

	public int amount;

	void OnTriggerEnter(Collider info){
		if(info.CompareTag("Player")){
			
			PlayerResources res = info.collider.GetComponent<PlayerResources>();

			res.PickUpGrenades(amount);
			Network.Destroy(gameObject);
		}
	}
}
