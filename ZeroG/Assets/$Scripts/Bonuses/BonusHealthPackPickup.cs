using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ObjectCleanUp))]
public class BonusHealthPackPickup : MonoBehaviour {
	[SerializeField]
    private int healStrength;

	void OnTriggerEnter(Collider info){
		if(info.CompareTag("Player")){
			
            IDamageable health = info.GetComponent<Collider>().gameObject.GetInterface<IDamageable>();

            if (!health.IsFullHealth()) {
                health.RestoreHealth(healStrength);
				//GetComponent<ObjectCleanUp>().KillMe();
			}
		}
	}
}
