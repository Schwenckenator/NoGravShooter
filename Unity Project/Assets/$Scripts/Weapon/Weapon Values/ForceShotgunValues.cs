using UnityEngine;
using System.Collections;

public class ForceShotgunValues : WeaponSuperClass {
	// All detail for weapon are set here
	public ForceShotgunValues(){

		this.useRay 				= false;
		this.hasRecoil 				= false;
		this.isEnergy 				= true;
		this.recoil 				= 0;
		this.rayNum 				= 0;

		this.shotSpread 			= 0;
		this.damagePerShot 			= 0;
		this.heatPerShot 			= 30;
		this.fireDelay 				= 0.75f;
		
		this.clipSize 				= 1000;
		this.currentClip 			= 1000;
		this.remainingAmmo 			= 20;
		this.defaultRemainingAmmo 	= this.remainingAmmo;
		this.reloadTime 			= 0.1f;
	}
	
}