using UnityEngine;
using System.Collections;

public class PlasmaBlasterValues : WeaponSuperClass {
	public PlasmaBlasterValues(){
		this.useRay 				= false;
		this.hasRecoil 				= false;
		this.isEnergy 				= true;
		this.recoil 				= 0;
		this.rayNum 				= 0;
		
		this.shotSpread 			= 0;
		this.damagePerShot 			= 0;
		this.heatPerShot 			= 0;
		this.fireDelay 				= 1.5f;
		
		this.clipSize 				= 1000;
		this.currentClip 			= 1000;
		this.remainingAmmo 			= 5;
		this.defaultRemainingAmmo 	= this.remainingAmmo;
		this.reloadTime 			= 0.1f;
	}
	
}