using UnityEngine;
using System.Collections;

public interface IWeaponValues{

	bool useRay {
		get;
		set;
	}
	bool hasRecoil{
		get;
		set;
	}
	bool isEnergy{
		get;
		set;
	}
	float recoil{
		get;
		set;
	}
	int rayNum{
		get;
		set;
	}
	float shotSpread{
		get;
		set;
	}
	int damagePerShot {
		get;
		set;
	}
	int heatPerShot {
		get;
		set;
	}
	float fireDelay{
		get;
		set;
	}
	int clipSize{
		get;
		set;
	}
	int currentClip{
		get;
		set;
	}
	float reloadTime{
		get;
		set;
	}
	GameObject projectile{
		get;
		set;
	}
	GameObject hitParticle{
		get;
		set;
	}
	AudioClip fireSound{
		get;
		set;
	}
	AudioClip reloadSound{
		get;
		set;
	}
}
