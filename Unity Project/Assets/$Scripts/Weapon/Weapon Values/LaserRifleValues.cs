using UnityEngine;
using System.Collections;

public class LaserRifleValues : WeaponSuperClass {
	// All detail for weapon are set here

	public LaserRifleValues(){
		this.useRay 				= true;
		this.hasRecoil 				= false;
		this.recoil 				= 0;
		this.rayNum 				= 1;
		
		this.shotSpread 			= 0.2f;
		this.damagePerShot 			= 10;
		this.heatPerShot 			= 10;
		this.fireDelay 				= 0.2f;
		
		this.clipSize 				= 100;
		this.currentClip 			= 100;
		this.remainingAmmo 			= 0;
		this.defaultRemainingAmmo 	= this.remainingAmmo;
		this.reloadTime 			= 0.1f;
	}

}