using UnityEngine;
using System.Collections;

public class WeaponReloadRotation : MonoBehaviour {

	private float downTime;
	private float holdTime;
	private float upTime;

	public void ReloadRotation(float reloadTime){
		downTime = reloadTime*0.35f;
		holdTime = reloadTime*0.3f;
		upTime = reloadTime*0.35f;

		StartCoroutine(MoveDown());
	}

	IEnumerator MoveDown(){ // 0.35
		float currentTime = Time.time;
		float endTime = currentTime + downTime;
		while (currentTime < endTime){
			transform.RotateAround(transform.TransformPoint(0, 0, -0.5f), transform.TransformDirection(1, 0, 0), (90/downTime)*Time.deltaTime);
			currentTime += Time.deltaTime;
			yield return null;
		}
		StartCoroutine(HoldStill());
	}

	IEnumerator HoldStill(){ // 0.3
		yield return new WaitForSeconds(holdTime);
		StartCoroutine(MoveUp());
	}

	IEnumerator MoveUp(){ // 0.35
		float currentTime = Time.time;
		float endTime = currentTime + upTime;
		while (currentTime < endTime){
			transform.RotateAround(transform.TransformPoint(0, 0, -0.5f), transform.TransformDirection(1, 0, 0), -(90/upTime)*Time.deltaTime);
			currentTime += Time.deltaTime;
			yield return null;
		}
		transform.localPosition = new Vector3(0.25f, -0.4f, 1);
		transform.localEulerAngles = Vector3.zero;
	}
}
