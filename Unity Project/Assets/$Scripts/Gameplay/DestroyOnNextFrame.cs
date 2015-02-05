using UnityEngine;
using System.Collections;

public class DestroyOnNextFrame : MonoBehaviour {
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
}
