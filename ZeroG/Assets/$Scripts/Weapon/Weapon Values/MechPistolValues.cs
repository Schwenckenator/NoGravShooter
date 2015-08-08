using UnityEngine;
using System.Collections;

public class MechPistolValues : WeaponSuperClass {

    public MechPistolValues() {
		this.useRay 				= true;
		this.hasRecoil 				= true;
		this.isEnergy 				= false;
		this.recoil 				= 1;
		this.rayNum 				= 1;
		
		this.shotSpread 			= 0.15f;
		this.damagePerShot 			= 25;
		this.heatPerShot 			= 0;
		this.fireDelay 				= 0.3f;
		
		this.clipSize 				= 13;
		this.currentClip 			= 13;
		this.remainingAmmo 			= 39;
		this.defaultRemainingAmmo 	= this.remainingAmmo;
		this.reloadTime 			= 1.0f;
	}
}
