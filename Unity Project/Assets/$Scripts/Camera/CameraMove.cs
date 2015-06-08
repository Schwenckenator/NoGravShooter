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

    public void AdjustSmooth() {
        // The player has been snapped to a terrain
        // This causes a stutter in the camera
        StartCoroutine(MoveCameraSmooth());
    }

    private IEnumerator MoveCameraSmooth() {
        // Detach from parent
        transform.parent = null;

        // Wait one frame, player will adjust without camera
        yield return null;

        // Re-attach to player camera object
        transform.parent = myPlayer;
        // Over .25s, lerp local position and rotation to 0
        bool translationComplete = false;
        bool rotationComplete = false;

        while (!(translationComplete && rotationComplete)){
            yield return null;

            if (!translationComplete) {
                Vector3 translation = Vector3.MoveTowards(transform.localPosition, Vector3.zero, maxTranslation * Time.deltaTime);
                transform.localPosition = translation;
            }
            if (!rotationComplete) {
                Quaternion rotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.identity, maxRotation * Time.deltaTime);
                transform.localRotation = rotation;
            }
            if (transform.localPosition.sqrMagnitude < sqrThreshold) {
                transform.localPosition = Vector3.zero;
                translationComplete = true;
            }
            if (transform.localRotation.eulerAngles.sqrMagnitude < sqrThreshold) {
                transform.localRotation = Quaternion.identity;
                rotationComplete = true;
            }
        } 
    }
}
