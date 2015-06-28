using UnityEngine;
using System.Collections;

public class DestroyOnNextFrame : MonoBehaviour, IDamageable {
	private bool willBeKilled = false;

	public void DestroyMe(){
		if(!willBeKilled){
			StartCoroutine(KillOnNextFrame());
			willBeKilled = true;
		}
	}

	IEnumerator KillOnNextFrame(){
		yield return null;
		GetComponent<ObjectCleanUp>().KillMe();
	}

    public int GetHealth() {
        return 1;
    }

    public int GetMaxHealth() {
        return 1;
    }

    public bool IsFullHealth() {
        return true;
    }

    public void TakeDamage(int damage, NetworkPlayer from, int weaponId = -1) {
        DestroyMe();
    }

    public void RestoreHealth(int restore) {
        // Do nothing
    }
}
