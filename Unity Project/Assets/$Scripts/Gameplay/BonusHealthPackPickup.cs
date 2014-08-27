using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ObjectCleanUp))]
public class BonusHealthPackPickup : MonoBehaviour {
	public int healStrength;

	void OnTriggerEnter(Collider info){
		if(info.CompareTag("Player")){
			
			PlayerResources res = info.collider.GetComponent<PlayerResources>();
			
			if(!res.IsFullHealth()){
				res.RestoreHealth(healStrength);
				GetComponent<ObjectCleanUp>().KillMe();
			}
			
		}
	}
}
