using UnityEngine;
using System.Collections;

public class WeaponReloadRotation : MonoBehaviour {
	
	public GameObject[] weaponModelList;

	private GameObject weaponModel;

	private Vector3 oldPosition;

	private float downTime;
	private float holdTime;
	private float upTime;

	private bool newModel = false;
	private int newModelNum = -1;

	void Start(){
		oldPosition = transform.localPosition;
	}

	public void ReloadRotation(float reloadTime, WeaponSuperClass newWeapon = null){


		downTime = reloadTime*0.35f;
		holdTime = reloadTime*0.3f;
		upTime = reloadTime*0.35f;

		if(newWeapon != null) NewModelPrep(newWeapon);

		StartCoroutine(MoveDown());
	}




	private void NewModelPrep(WeaponSuperClass newWeapon){


		for(int i=0; i < GameManager.weapon.Count; i++){
			if(GameManager.weapon[i].Equals(newWeapon)){
				newModelNum = i;
				newModel = true;
			}
		}
	}

	private void ChangeModel(){



		if(weaponModel != null){
			Destroy(weaponModel);
		}
		weaponModel = Instantiate(weaponModelList[newModelNum]) as GameObject;

		weaponModel.transform.parent = transform;

		weaponModel.transform.localPosition = Vector3.zero;
		weaponModel.transform.localEulerAngles = new Vector3(0, 270, 0);

		newModel = false;
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

		if(newModel){
			ChangeModel();
		}

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
		transform.localPosition = oldPosition;
		transform.localEulerAngles = Vector3.zero;
	}
}
