using UnityEngine;
using System.Collections;

public class SlugRifleWeaponValues : ScriptableObject, IWeaponValues{
	#region CopyPaste for new Weapon.  No I don't like it either
	
	#region Private Declarations
	private static int _damagePerShot;
	private static int _heatPerShot;
	private static float _fireDelay;
	private static int _clipSize;
	private static int _currentClip;
	private static float _reloadTime;
	private static GameObject _projectile;
	private static GameObject _hitParticle;
	private static AudioClip _fireSound;
	private static AudioClip _reloadSound;
	#endregion
	
	#region Accessors and Mutators
	public int damagePerShot{
		get{
			return _damagePerShot;
		}
		set{
			_damagePerShot = value;
		}
	}
	public int heatPerShot{
		get{
			return _heatPerShot;
		}
		set{
			_heatPerShot = value;
		}
	}
	public float fireDelay{
		get{
			return _fireDelay;
		}
		set{
			_fireDelay = value;
		}
	}
	public int clipSize{
		get{
			return _clipSize;
		}
		set{
			_clipSize = value;
		}
	}
	public int currentClip{
		get{
			return _currentClip;
		}
		set{
			_currentClip = value;
		}
	}
	public float reloadTime{
		get{
			return _reloadTime;
		}
		set{
			_reloadTime = value;
		}
	}
	
	public GameObject projectile{
		get{
			return _projectile;
		}
		set{
			_projectile = value;
		}
	}
	public GameObject hitParticle{
		get{
			return _hitParticle;
		}
		set{
			_hitParticle = value;
		}
	}
	public AudioClip fireSound{
		get{
			return _fireSound;
		}
		set {
			_fireSound = value;
		}
	}
	public AudioClip reloadSound{
		get{
			return _reloadSound;
		}
		set {
			_reloadSound = value;
		}
	}
	#endregion
	
	#endregion
}