using UnityEngine;
using System.Collections;

public interface IWeaponValues{

	int DamagePerShot {
		get;
		set;
	}
	int HeatPerShot {
		get;
		set;
	}
	float FireDelay{
		get;
		set;
	}
	GameObject Projectile{
		get;
		set;
	}
	GameObject HitParticle{
		get;
		set;
	}
	AudioClip FireSound{
		get;
		set;
	}
}
