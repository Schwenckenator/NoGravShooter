﻿using UnityEngine;
using System.Collections;

public class ControllerInput : MonoBehaviour, IControllerInput {

    float xMovement = 0; // Left - Right
    float yMovement = 0; // Up  - Down
    float zMovement = 0; // Forward - Back
    float rollMovement = 0;

    int changeRate = 3; // * deltaTime
	
	// Update is called once per frame
	void Update () {
        UpdateRollMovement();
        xMovement = Input.GetAxis("Horizontal");
        yMovement = Input.GetAxis("JetPackUpDown");
        zMovement = Input.GetAxis("Vertical");
	}
    void UpdateRollMovement() {
        int direction = 0;
        if (Input.GetButton("joystick button 8")) direction--;
        if (Input.GetButton("joystick button 9")) direction++;
        rollMovement = SnapInput(rollMovement, direction);
        rollMovement = Mathf.MoveTowards(rollMovement, direction, changeRate * Time.deltaTime);
    }


    public float GetRollMovement() {

        
        throw new System.NotImplementedException();
    }

    public float GetXMovement() {
        throw new System.NotImplementedException();
    }

    public float GetYMovement() {
        throw new System.NotImplementedException();
    }

    public float GetZMovement() {
        throw new System.NotImplementedException();
    }

    public bool IsMovementKeys() {
        throw new System.NotImplementedException();
    }

    public bool IsStopKey() {
        throw new System.NotImplementedException();
    }

    float SnapInput(float axis, float direction) {
        float difference = axis - direction;
        if (Mathf.Abs(difference) > 1f) {
            axis = 0.0f;
        }
        return axis;
    }
}
