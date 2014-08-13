using UnityEngine;
using System.Collections;

public class WeaponSuperClass{
	
	#region Private Declarations
	private bool _useRay;
	private bool _hasRecoil;
	private float _recoil;
	private int _rayNum;
	private float _shotSpread;
	private int _damagePerShot;
	private int _heatPerShot;
	private float _fireDelay;
	private int _clipSize;
	private int _currentClip;
	private int _remainingAmmo;
	private int _defaultRemainingAmmo;
	private float _reloadTime;
	private GameObject _projectile;
	private GameObject _hitParticle;
	private AudioClip _fireSound;
	private AudioClip _reloadSound;
	#endregion
	
	#region Accessors and Mutators
	public bool hasRecoil{
		get{
			return _hasRecoil;
		}
		set{
			_hasRecoil = value;
		}
	}
	public float recoil{
		get{
			return _recoil;
		}
		set{
			_recoil = value;
		}
	}
	public bool useRay{
		get{
			return _useRay;
		}
		set{
			_useRay = value;
		}
	}
	public int rayNum{
		get{
			return _rayNum;
		}
		set{
			_rayNum = value;
		}
	}
	public float shotSpread{
		get{
			return _shotSpread;
		}
		set{
			_shotSpread = value;
		}
	}
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
	public int remainingAmmo{
		get{
			return _remainingAmmo;
		}
		set{
			_remainingAmmo = value;
		}
	}
	public int defaultRemainingAmmo{
		get{
			return _defaultRemainingAmmo;
		}
		set{
			_defaultRemainingAmmo = value;
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
}
