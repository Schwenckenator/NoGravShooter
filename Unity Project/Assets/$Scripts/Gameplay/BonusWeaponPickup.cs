using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ObjectCleanUp))]
public class BonusWeaponPickup : MonoBehaviour {
	
	public int maxWeapons = 2;
	public int id;
	private GameManager manager;
	private FireWeapon weapon;
	private int autoPickup = 0;
	private int weaponcount;
	private bool playerColliding = false;
	void Start(){
		if(GameManager.testMode){
			maxWeapons = 99;
		}
		id = Random.Range(0,6);
		manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
	}
	
	void OnTriggerEnter(Collider info){
		if(info.CompareTag("Player")){
			playerColliding = true;
			autoPickup = PlayerPrefs.GetInt("autoPickup", 0);
			weapon = info.GetComponent<FireWeapon>();
			weaponcount = weapon.NumberWeaponsHeld();
		}
	}
	
	void OnTriggerExit(Collider info){
		if(info.CompareTag("Player")){
			playerColliding = false;
		}
	}
	
	public float swapTimeout = 2f;
	void FixedUpdate(){
		if(playerColliding){
			if(weapon.IsWeaponHeld(id)){
				GameManager.weapon[id].remainingAmmo += (GameManager.weapon[id].clipSize + GameManager.weapon[id].defaultRemainingAmmo);
				GetComponent<ObjectCleanUp>().KillMe();
			}else{
				if(weaponcount >= maxWeapons){
					if(swapTimeout < Time.time){
						swapTimeout = 2f;
						manager.GetComponent<GUIScript>().ButtonPrompt((int)GameManager.KeyBind.Interact, "Swap Weapons");
						if(Input.GetKey(GameManager.keyBindings[(int)GameManager.KeyBind.Interact])){
							for(int i=0; i < 7; i++){
								if(weapon.IsCurrentWeapon(i)){
									//weapon.removeWeapon(i);
									weapon.removeWeapon(GameManager.weapon[i]);
									weapon.AddWeapon(id);
									weapon.ChangeWeapon(id);
									id = i;
									swapTimeout = Time.time + 2f;
									i = 99;//lol
								}
							}
						}
					}
				}else{
					weapon.AddWeapon(id);
					if(autoPickup == 1){
						weapon.ChangeWeapon(weaponcount);
					}
					GetComponent<ObjectCleanUp>().KillMe();
				}
			}
		}
	}
}
