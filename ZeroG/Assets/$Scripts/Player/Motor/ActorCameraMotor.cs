using UnityEngine;
using System.Collections;

public class ActorCameraMotor : MonoBehaviour {

    public Transform cameraTransform;
    
    MouseLook cameraMouseLook;
    MouseLook characterMouseLook;


	// Use this for initialization
	void Start () {
        cameraTransform = transform.GetChild(0);
        cameraMouseLook = cameraTransform.GetComponent<MouseLook>();
        characterMouseLook = GetComponent<MouseLook>();

        LockMouseLook(true); // While there is no walking, just lock to air for now.
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void LockMouseLook(bool inAir) {
        if (inAir) {
            cameraMouseLook.axes = MouseLook.RotationAxes.MouseNone;
            characterMouseLook.axes = MouseLook.RotationAxes.MouseXAndY;
            FixRotation();

        } else {
            cameraMouseLook.axes = MouseLook.RotationAxes.MouseY;
            characterMouseLook.axes = MouseLook.RotationAxes.MouseX;
        }
    }

    void FixRotation() {
        transform.Rotate(cameraTransform.localEulerAngles);
        cameraMouseLook.SetX_Rotation(0f);
    }
}
