using UnityEngine;
using System.Collections;

public class LaserSniperValues : WeaponSuperClass {
	// All detail for weapon are set here
	
	public LaserSniperValues(){
		this.useRay 				= true;
		this.hasRecoil 				= false;
		this.recoil 				= 0;
		this.rayNum 				= 1;
		
		this.shotSpread 			= 0.1f;
		this.damagePerShot 			= 50;
		this.heatPerShot 			= 70;
		this.fireDelay 				= 2f;
		
		this.clipSize 				= 100;
		this.currentClip 			= 100;
		this.remainingAmmo 			= 0;
		this.defaultRemainingAmmo 	= this.remainingAmmo;
		this.reloadTime 			= 0.1f;
	}
	
}
