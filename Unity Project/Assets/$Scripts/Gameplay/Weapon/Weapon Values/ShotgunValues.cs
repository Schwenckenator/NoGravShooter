using UnityEngine;
using System.Collections;

public class ShotgunValues : WeaponSuperClass {

	public ShotgunValues(){
		this.useRay 				= true;
		this.hasRecoil 				= true;
		this.recoil 				= 5;
		this.rayNum 				= 30;
		
		this.shotSpread 			= 10f;
		this.damagePerShot 			= 5;
		this.heatPerShot 			= 0;
		this.fireDelay 				= 0.75f;
		
		this.clipSize 				= 6;
		this.currentClip 			= 6;
		this.remainingAmmo 			= 12;
		this.defaultRemainingAmmo 	= this.remainingAmmo;
		this.reloadTime 			= 2f;
	}
	
}
