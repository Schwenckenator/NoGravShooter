using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {

    public float maxRotation;
    public float maxTranslation;
    public float threshold;

	private Transform myPlayer;
    private float sqrThreshold;

    void Start() {
        sqrThreshold = threshold * threshold;
    }

	public void PlayerSpawned(){
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach(GameObject player in players){
			if(player.networkView.isMine){
				myPlayer = player.transform.GetChild(0);
				AttachCamera();
			}
		}
	}

	void AttachCamera(){

		transform.position = myPlayer.position;
		transform.rotation = myPlayer.rotation;

		transform.parent = myPlayer;
	}

	public void DetachCamera(){
		transform.parent = null;
		transform.position = new Vector3(0, 30, 0);
	}

    public void PrepareAdjust() {
        // Detach from parent
        transform.parent = null;
    }
    public void SmoothAdjust() {
        StartCoroutine(MoveCameraSmooth());
    }

    private IEnumerator MoveCameraSmooth() {
        // Re-attach to player camera object
        transform.parent = myPlayer;
        
        bool translationComplete = false;
        bool rotationComplete = false;

        // Quickly lerp local position and rotation to 0
        while (!(translationComplete && rotationComplete)){
            transform.localPosition = Vector3.zero;
            yield return null;

            if (!rotationComplete) {
                Quaternion rotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.identity, maxRotation * Time.deltaTime);
                transform.localRotation = rotation;
            }
            if (transform.localRotation.eulerAngles.sqrMagnitude < sqrThreshold) {
                transform.localRotation = Quaternion.identity;
                rotationComplete = true;
            }
        } 
    }

}
