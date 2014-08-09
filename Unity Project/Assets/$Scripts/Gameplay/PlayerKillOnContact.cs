using UnityEngine;
using System.Collections;

public class PlayerKillOnContact : MonoBehaviour {
	
	void OnCollisionEnter(Collision info){
		if(info.collider.CompareTag("Player") && info.collider.networkView.isMine){
			info.collider.GetComponent<PlayerResources>().TakeDamage(100); //Instakill
		}
	}
}
