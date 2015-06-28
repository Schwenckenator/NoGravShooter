using UnityEngine;
using System.Collections;

public class LaserSniperValues : WeaponSuperClass {
	// All detail for weapon are set here
	
	public LaserSniperValues(){
		this.useRay 				= true;
		this.hasRecoil 				= false;
		this.isEnergy 				= true;
		this.recoil 				= 0;
		this.rayNum 				= 1;
		
		this.shotSpread 			= 0.0f;
		this.damagePerShot 			= 50;
		this.heatPerShot 			= 34;
		this.fireDelay 				= 2f;
		
		this.clipSize 				= 1000;
		this.currentClip 			= 1000;
		this.remainingAmmo 			= 20;
		this.defaultRemainingAmmo 	= this.remainingAmmo;
		this.reloadTime 			= 0.1f;
	}
	
}
