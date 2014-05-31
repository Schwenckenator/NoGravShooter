using UnityEngine;
using System.Collections;

public class PlayerResources : MonoBehaviour {
	public int maxFuel;
	public int minFuel;
	private float fuel;

	public float fuelRecharge;
	public float maxRechargeWaitTime;
	private float rechargeWaitTime;
	// Use this for initialization
	void Start () {
		//Use them like percentage, for now
		maxFuel = 100;
		fuel = maxFuel;

		minFuel = 0;

		fuelRecharge = 100; // per second

		maxRechargeWaitTime = 1.0f; // seconds
		rechargeWaitTime = maxRechargeWaitTime;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		RechargeFuel(fuelRecharge);
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


	public float GetFuel(){
		return fuel;
	}
	public void SetFuel(int newFuel){
		fuel = newFuel;
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
}
