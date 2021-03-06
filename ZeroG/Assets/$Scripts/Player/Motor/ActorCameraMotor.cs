﻿using UnityEngine;
using System.Collections;

public class ActorCameraMotor : MonoBehaviour, IResetable {

    public Transform cameraTransform;
    public CameraSmoothMove smoothMove;
    
    MouseLook cameraMouseLook;
    MouseLook characterMouseLook;

	// Use this for initialization
	void Start () {
        cameraMouseLook = cameraTransform.GetComponent<MouseLook>();
        characterMouseLook = GetComponent<MouseLook>();

        LockMouseLook(true); // Actor starts in air
	}

    public void LockMouseLook(bool inAir) {
        if (inAir) {
            cameraMouseLook.axes = MouseLook.RotationAxes.MouseNone;
            characterMouseLook.axes = MouseLook.RotationAxes.MouseXAndY;
            ZeroLocalRotations();

        } else {
            cameraMouseLook.axes = MouseLook.RotationAxes.MouseY;
            characterMouseLook.axes = MouseLook.RotationAxes.MouseX;
        }
    }

    /// <summary>
    /// From walking to air. Zeros local rotations and rotates root transform to match
    /// </summary>
    void ZeroLocalRotations() {
        transform.Rotate(cameraTransform.localEulerAngles);
        cameraMouseLook.SetX_Rotation(0f);
    }

    public void SnapToSurface(Vector3 colNormal) {
        smoothMove.PrepareAdjust();
        // Preserve camera angle
        Quaternion currentCameraRotation = cameraTransform.rotation;

        transform.rotation = Quaternion.LookRotation(colNormal, transform.forward);
        transform.Rotate(new Vector3(-90, 180, 0));

        // Rotate Camera
        cameraTransform.rotation = currentCameraRotation;

        // But the mouse code will overwrite this
        // Find the local X rot
        float xRot = cameraTransform.localEulerAngles.x;
        float yRot = cameraTransform.localEulerAngles.y;


        // Make sure Y and Z local rotations are 0
        cameraTransform.localEulerAngles = new Vector3(cameraTransform.localEulerAngles.x, 0, 0);
        transform.Rotate(0, yRot, 0);

        // Make it digestable
        if (xRot > 180) {
            xRot -= 360;
        }

        cameraMouseLook.SetX_Rotation(xRot);

        //Adjust Camera
        smoothMove.SmoothAdjust();
    }

    void OnDeath() {
        cameraMouseLook.Ragdoll(true);
        characterMouseLook.Ragdoll(true);
    }
    public void Reset() {
        //cameraMouseLook.Ragdoll(false);
        //characterMouseLook.Ragdoll(false);
        StartCoroutine(CoUnRagdoll());
    }
    IEnumerator CoUnRagdoll() {
        //yield return new WaitForSeconds(0.1f);
        
        Debug.Log("Unragdolling");
        cameraMouseLook.Ragdoll(false);
        characterMouseLook.Ragdoll(false);

        yield return null;
    }

    public void Recoil(float angle) {
        // If here, actor is grounded
        cameraMouseLook.AddX_Rotation(angle);
    }

    public void NotMine() {
        cameraMouseLook.enabled = false;
        characterMouseLook.enabled = false;
    }
}
