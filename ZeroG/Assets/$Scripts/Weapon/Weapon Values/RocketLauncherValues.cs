using UnityEngine;
using System.Collections;

public class RocketLauncherValues : WeaponSuperClass {
	// All detail for weapon are set here
	
	public RocketLauncherValues(){
		this.useRay 				= false;
		this.hasRecoil 				= false;
		this.isEnergy 				= false;
		this.recoil 				= 0;
		this.rayNum 				= 0;
		
		this.shotSpread 			= 0;
		this.damagePerShot 			= 0;
		this.heatPerShot 			= 0;
		this.fireDelay 				= 1f;
		
		this.clipSize 				= 1;
		this.currentClip 			= 1;
		this.remainingAmmo 			= 4;
		this.defaultRemainingAmmo 	= this.remainingAmmo;
		this.reloadTime 			= 3f;
	}
	
}