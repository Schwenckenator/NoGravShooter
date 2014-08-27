using UnityEngine;
using System.Collections;

public class DestroyOnNextFrame : MonoBehaviour {
	private bool killMe = false;

	public void DestroyMe(){
		if(!killMe){
			StartCoroutine(KillOnNextFrame());
			killMe = true;
		}
	}

	IEnumerator KillOnNextFrame(){
		yield return null;
		GetComponent<ObjectCleanUp>().KillMe();
	}
}
