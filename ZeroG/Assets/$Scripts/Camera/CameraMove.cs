using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {

	private Transform myAnchor;

	public void AttachToPlayer(GameObject actor){
        myAnchor = actor.transform.GetChild(0).GetChild(0);
        AttachCamera();
    }

    void AttachCamera(){

		transform.position = myAnchor.position;
		transform.rotation = myAnchor.rotation;

		transform.parent = myAnchor;
	}

	public void DetachCamera(){
		transform.parent = null;
		transform.position = new Vector3(0, 30, 0);
	}

    //void OnLevelWasLoaded() {
    //    if (GameManager.IsSceneMenu()) {
    //        DetachCamera();
    //        Destroy(gameObject);
    //    }
    //}
}
