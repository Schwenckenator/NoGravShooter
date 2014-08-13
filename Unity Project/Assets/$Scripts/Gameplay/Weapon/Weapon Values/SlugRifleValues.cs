using UnityEngine;
using System.Collections;

public class SlugRifleValues : WeaponSuperClass {

	public SlugRifleValues(){
		this.useRay 				= true;
		this.hasRecoil 				= true;
		this.recoil 				= 1;
		this.rayNum 				= 1;
		
		this.shotSpread 			= 1f;
		this.damagePerShot 			= 10;
		this.heatPerShot 			= 0;
		this.fireDelay 				= 0.125f;
		
		this.clipSize 				= 30;
		this.currentClip 			= 30;
		this.remainingAmmo 			= 30;
		this.defaultRemainingAmmo 	= this.remainingAmmo;
		this.reloadTime 			= 1.6f;
	}
	
}