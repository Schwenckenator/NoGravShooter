using UnityEngine;
using System.Collections;

public class PlayerKillOnContact : MonoBehaviour {
	
	void OnCollisionEnter(Collision info){
        //if (info.collider.CompareTag("Player") && /*info.collider.GetComponent<//NetworkView>().isMine*/) {
            IDamageable damageable = info.collider.GetComponent(typeof(IDamageable)) as IDamageable;
			damageable.TakeDamage(100, Network.player); //Instakill
		//}
	}
}
