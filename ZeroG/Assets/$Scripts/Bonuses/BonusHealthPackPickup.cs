using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[RequireComponent(typeof(ObjectCleanUp))]
public class BonusHealthPackPickup : NetworkBehaviour{
	
    public int healStrength;

    [ServerCallback]
	void OnTriggerEnter(Collider info){
		if(info.CompareTag("Player")){
			
            IDamageable health = info.GetComponent<Collider>().gameObject.GetInterface<IDamageable>();

            if (!health.IsFullHealth) {
                health.RestoreHealth(healStrength);
				//GetComponent<ObjectCleanUp>().KillMe();
			}
		}
	}
}
