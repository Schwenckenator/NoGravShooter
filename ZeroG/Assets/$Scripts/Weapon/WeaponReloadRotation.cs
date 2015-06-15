using UnityEngine;
using System.Collections;

public class WeaponReloadRotation : MonoBehaviour {
	
	public GameObject[] weaponModelList;
	public float[] firePointZPosition;

	private Transform firePoint;

	private GameObject weaponModel;

	private Vector3 oldPosition;

	private float downTime;
	private float holdTime;
	private float upTime;

	private bool newModel = false;
	private int newModelNum = -1;

	void Awake(){
		oldPosition = transform.localPosition;
		firePoint = transform.FindChild("FirePoint");
	}

	public void ReloadRotation(float reloadTime, WeaponSuperClass newWeapon = null){
        StopAllCoroutines();

		downTime = reloadTime*0.35f;
		holdTime = reloadTime*0.3f;
		upTime = reloadTime*0.35f;

		if(newWeapon != null) NewModelPrep(newWeapon);

		StartCoroutine("MoveDown");
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
        
        if(transform.root.GetComponent<NetworkView>().isMine)
            SetLayers(weaponModel);

		weaponModel.transform.parent = transform;

		weaponModel.transform.localPosition = new Vector3(0, 0, 0.5f);
		weaponModel.transform.localEulerAngles = new Vector3(0, 270, 0);

		firePoint.localPosition = new Vector3(0, 0, firePointZPosition[newModelNum] + 0.5f);

		newModel = false;
	}

    private void SetLayers(GameObject weapon) {
        string layerName = "GunLayer";
        Transform[] children = weapon.GetComponentsInChildren<Transform>();

        foreach (Transform child in children) {
            child.gameObject.layer = LayerMask.NameToLayer(layerName);
        }
    }


	IEnumerator MoveDown(){ // 0.35
        float endTime = Time.time + downTime;
        float t = 0;
        float startAngle = transform.localEulerAngles.x;
        float endAngle = 90f; // -90 as a positive rotation

		while (Time.time < endTime){
            transform.localEulerAngles = new Vector3(Mathf.LerpAngle(startAngle, endAngle, (t / upTime)), 0, 0);
            t += Time.deltaTime;
            LogRotation();
			yield return null;
		}
		StartCoroutine("HoldStill");
	}

	IEnumerator HoldStill(){ // 0.3
		if(newModel){
			ChangeModel();
		}
		yield return new WaitForSeconds(holdTime);
		StartCoroutine("MoveUp");
	}

	IEnumerator MoveUp(){ // 0.35
		float endTime = Time.time + upTime;
        float t = 0;
        float startAngle = transform.localEulerAngles.x;
        float endAngle = 0f;

        while (Time.time < endTime) {
            transform.localEulerAngles = new Vector3(Mathf.LerpAngle(startAngle, endAngle, (t / upTime)), 0, 0);
            t += Time.deltaTime;
            LogRotation();
			yield return null;
		}
		transform.localPosition = oldPosition;
		transform.localEulerAngles = Vector3.zero;
    }

    void LogRotation() {
        if (DebugManager.IsDebugMode()) {
            Debug.Log(transform.localEulerAngles);
        }
    }


}
