using UnityEngine;
using System.Collections;

public interface IWeaponValues{

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
