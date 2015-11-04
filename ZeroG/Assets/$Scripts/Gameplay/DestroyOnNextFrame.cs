using UnityEngine;
using System.Collections;
using System;

public class DestroyOnNextFrame : MonoBehaviour, IDamageable {
	private bool willBeKilled = false;

    public int Health {
        get {
            return 1;
        }
    }

    public int MaxHealth {
        get {
            return 1;
        }
    }

    public bool IsFullHealth {
        get {
            return true;
        }
    }

    public void DestroyMe(){
		if(!willBeKilled){
			StartCoroutine(KillOnNextFrame());
			willBeKilled = true;
		}
	}

	IEnumerator KillOnNextFrame(){
		yield return null;
		//GetComponent<ObjectCleanUp>().KillMe();
	}

    public void TakeDamage(int damage, NetworkPlayer from, int weaponId = -1) {
        DestroyMe();
    }

    public void RestoreHealth(int restore) {
        // Do nothing
    }

    public void TakeDamage(int damage, int weaponId = -1) {
        throw new NotImplementedException();
    }
}
