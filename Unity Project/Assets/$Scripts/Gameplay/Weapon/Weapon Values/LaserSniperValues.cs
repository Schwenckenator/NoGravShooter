using UnityEngine;
using System.Collections;

public class LaserSniperValues : WeaponSuperClass {
	// All detail for weapon are set here
	#region Public in Variables
	public bool in_useRay			= true;
	public bool in_hasRecoil		= false;
	public float in_recoil			= 0;
	public int in_rayNum			= 1;
	
	public float in_shotSpread		= 0.1f;
	public int in_damagePerShot		= 50;
	public int in_heatPerShot		= 30;
	public float in_fireDelay		= 0.5f;
	
	public int in_clipSize			= 100;
	public int in_currentClip		= 1;
	public int in_remainingAmmo		= 0;
	public float in_reloadTime		= 0.1f;

	#endregion
	
	public LaserSniperValues(){
		//Debug.Log (this.ToString());

		this.useRay = in_useRay;
		this.rayNum = in_rayNum;
		this.shotSpread = in_shotSpread;
		this.damagePerShot = in_damagePerShot;
		
		this.hasRecoil = in_hasRecoil;
		this.recoil = in_recoil;
		this.heatPerShot = in_heatPerShot;
		this.fireDelay = in_fireDelay;
		
		this.clipSize = in_clipSize;
		this.currentClip = in_currentClip;
		this.remainingAmmo = in_remainingAmmo;
		this.defaultRemainingAmmo = in_remainingAmmo;
		this.reloadTime = in_reloadTime;
	}
	
}
