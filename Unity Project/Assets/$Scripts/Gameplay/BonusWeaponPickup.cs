using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ObjectCleanUp))]
public class BonusWeaponPickup : MonoBehaviour {


	void OnTriggerEnter(Collider info){
		if(info.CompareTag("Player")){

			FireWeapon weapon = info.GetComponent<FireWeapon>();

			int id = Random.Range(0,6);
			if(weapon.IsWeaponHeld(id)){
				GameManager.weapon[id].remainingAmmo += (GameManager.weapon[id].clipSize + GameManager.weapon[id].defaultRemainingAmmo);
			}else{
				weapon.AddWeapon(id);
			}

			GetComponent<ObjectCleanUp>().KillMe();
		}
	}
}
