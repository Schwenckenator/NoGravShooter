using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ObjectCleanUp))]
public class BonusBlackHoleMinePackPickup : MonoBehaviour {

	public int amount;

	void OnTriggerEnter(Collider info){
		if(info.CompareTag("Player")){
			
			PlayerResources res = info.collider.GetComponent<PlayerResources>();

			res.PickUpGrenades(amount);
			GetComponent<ObjectCleanUp>().KillMe();
		}
	}
}
