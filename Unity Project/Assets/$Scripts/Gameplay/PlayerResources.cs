﻿using UnityEngine;
using System.Collections;

public class PlayerResources : MonoBehaviour {
	private GameManagerScript manager;

	public AudioClip soundOverheat;

	public int maxFuel = 100;
	public int minFuel = 0;
	public int maxHealth = 100;
	public int minHealth = 0;
	public int maxWeapon = 100;
	private int minWeapon = 0;

	private float fuel;
	private int health;
	private float weapon;

	public float fuelRecharge = 50;
	public float maxRechargeWaitTime = 1.0f;
	public float weaponRecharge = 30;
	public float weaponRechangeWaitTime = 2.0f;

	private float rechargeWaitTime;
	// Use this for initialization
	void Start () {
		manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>();
		//Use them like percentage, for now

		fuel = maxFuel;
		health = maxHealth;
		rechargeWaitTime = 0;
	}

	// Update is called once per frame
	void FixedUpdate () {
		RechargeFuel(fuelRecharge);
		RechargeWeapon(weaponRecharge);
		if(Input.GetKeyDown(KeyCode.K)){ //K is for kill!
			TakeDamage(10);
		}
	}
	private void RechargeFuel(float charge){
		//Are you waiting?
		if(rechargeWaitTime > 0){
			rechargeWaitTime -= Time.deltaTime;
		}else{
			fuel += charge * Time.deltaTime;
			if(fuel > maxFuel){
				fuel = maxFuel;
			}
		}
	}
	private void RechargeWeapon(float charge){
		weapon -= charge * Time.deltaTime;
		if(weapon < minWeapon){
			weapon = minWeapon;
		}
	}
	public int GetHealth(){
		return health;
	}

	public float GetFuel(){
		return fuel;
	}
	public float GetWeaponHeat(){
		return weapon;
	}

	public bool SpendFuel(float spentFuel){
		rechargeWaitTime = maxRechargeWaitTime;
		fuel -= spentFuel;
		if(fuel < minFuel){
			fuel = minFuel;
			return false;
		}
		return true;
	}
	
	public void TakeDamage(int damage){
		networkView.RPC("TakeDamageRPC", RPCMode.All, damage);
	}

	[RPC]
	void TakeDamageRPC(int damage){
		health -= damage;
		if(health <= 0){
			//You is dead nigs
			if(networkView.isMine){
				manager.PlayerDied();
				manager.ManagerDetachCamera();
				manager.CursorVisible(true);
			}
			Destroy(gameObject);
		}
	}

	public void RestoreHealth(int restore){
		health += restore;
		if(health > maxHealth){
			health = maxHealth;
		}
	}

	public bool WeaponCanFire(){
		return (weapon < maxWeapon);
	}

	public void WeaponFired(float addedHeat){
		weapon += addedHeat;
		if(weapon > maxWeapon){
			//Gun overheated
			audio.PlayOneShot(soundOverheat);
			weapon = maxWeapon + (weaponRecharge * weaponRechangeWaitTime);
		}
	}
}