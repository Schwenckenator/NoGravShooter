using UnityEngine;
using System.Collections;

/// <summary>
/// Placed on the anchor that connects to the CameraPos
/// </summary>
public class CameraSmoothMove : MonoBehaviour {

    public float maxRotation;
    public float threshold;

    private Transform cameraPos;
    private float sqrThreshold;

    void Start() {
        cameraPos = transform.parent;
        sqrThreshold = threshold * threshold;
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
        transform.parent = cameraPos;
        transform.localPosition = Vector3.zero;

        bool rotationComplete = false;
        
        // Quickly lerp local position and rotation to 0
        while (!rotationComplete) {
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
