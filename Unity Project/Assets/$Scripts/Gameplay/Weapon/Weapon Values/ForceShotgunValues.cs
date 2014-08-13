using UnityEngine;
using System.Collections;

public class ForceShotgunValues : WeaponSuperClass {
	// All detail for weapon are set here
	public ForceShotgunValues(){

		this.useRay 				= false;
		this.hasRecoil 				= true;
		this.recoil 				= 5;
		this.rayNum 				= 0;

		this.shotSpread 			= 0;
		this.damagePerShot 			= 0;
		this.heatPerShot 			= 0;
		this.fireDelay 				= 0.75f;
		
		this.clipSize 				= 6;
		this.currentClip 			= 6;
		this.remainingAmmo 			= 12;
		this.defaultRemainingAmmo 	= this.remainingAmmo;
		this.reloadTime 			= 2f;
	}
	
}