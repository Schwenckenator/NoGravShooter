using UnityEngine;
using System.Collections;


public class LaserWeaponValues : ScriptableObject, IWeaponValues{
	#region Private Declarations
	private static int _damagePerShot;
	private static int _heatPerShot;
	private static float _fireDelay;
	private static GameObject _projectile;
	private static GameObject _hitParticle;
	private static AudioClip _fireSound;
	#endregion

	#region Accessors and Mutators
	public int DamagePerShot{
		get{
			return _damagePerShot;
		}
		set{
			_damagePerShot = value;
		}
	}
	public int HeatPerShot{
		get{
			return _heatPerShot;
		}
		set{
			_heatPerShot = value;
		}
	}
	public float FireDelay{
		get{
			return _fireDelay;
		}
		set{
			_fireDelay = value;
		}
	}

	public GameObject Projectile{
		get{
			return _projectile;
		}
		set{
			_projectile = value;
		}
	}
	public GameObject HitParticle{
		get{
			return _hitParticle;
		}
		set{
			_hitParticle = value;
		}
	}
	public AudioClip FireSound{
		get{
			return _fireSound;
		}
		set {
			_fireSound = value;
		}
	}
	#endregion
}
