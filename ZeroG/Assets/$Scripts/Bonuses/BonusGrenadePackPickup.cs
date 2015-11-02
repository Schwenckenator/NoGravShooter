using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ObjectCleanUp))]
public class BonusGrenadePackPickup : MonoBehaviour {

    [SerializeField]
	private int amount;
    [SerializeField]
	private int grenadeType;
    [SerializeField]
	private bool randomtype;

	void Start(){
        //Randomise here, just for now
        if(randomtype){
			grenadeType = Random.Range(0, 3);
		}
	}

	void OnTriggerEnter(Collider info){
		if(info.CompareTag("Player")){
			
			ActorGrenades actorGrenade = info.GetComponent<Collider>().GetComponent<ActorGrenades>();

			if(SettingsManager.instance.AutoPickup){
				actorGrenade.ChangeGrenade(grenadeType);
			}
			actorGrenade.PickUpGrenades(amount, grenadeType);
			
			////GetComponent<ObjectCleanUp>().KillMe();
		}
	}
}
