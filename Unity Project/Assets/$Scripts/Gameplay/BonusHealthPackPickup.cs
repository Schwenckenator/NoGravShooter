using UnityEngine;
using System.Collections;

public class BonusHealthPackPickup : MonoBehaviour {
	public int healStrength;

	void OnTriggerEnter(Collider info){
		if(info.CompareTag("Player")){
			
			PlayerResources res = info.collider.GetComponent<PlayerResources>();
			
			if(!res.IsFullHealth()){
				res.RestoreHealth(healStrength);
				Destroy(gameObject);
			}
			
		}
	}
}
