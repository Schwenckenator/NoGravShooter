using UnityEngine;
using System.Collections;

public interface IControllerInput {

    float GetRollMovement();
    float GetXMovement();
    float GetYMovement();
    float GetZMovement();
    bool IsMovementKeys();
    bool IsStopKey();

}
public class ControllerInput : MonoBehaviour, IControllerInput{
    
    float xMovement = 0; // Left - Right
    float yMovement = 0; // Up  - Down
    float zMovement = 0; // Forward - Back
    float rollMovement = 0;

    int changeRate = 3; // * deltaTime

    public float GetRollMovement() {
        return rollMovement;
    }
    public float GetXMovement() {
        return xMovement;
    }
    public float GetYMovement() {
        return yMovement;
    }
    public float GetZMovement() {
        return zMovement;
    }

    public bool IsStopKey() {
        return IsDown(KeyBind.StopMovement);
    }
    
    public bool IsMovementKeys() {
        return IsDown(KeyBind.MoveRight) ||
            IsDown(KeyBind.MoveLeft) ||

            IsDown(KeyBind.JetUp) ||
            IsDown(KeyBind.JetDown) ||

            IsDown(KeyBind.MoveForward) ||
            IsDown(KeyBind.MoveBack);
    }
    private bool IsDown(KeyBind key) {
        return InputConverter.GetKey(key);
    }

    void Update() {
        rollMovement = UpdateMovement(KeyBind.RollLeft, KeyBind.RollRight, rollMovement);
        xMovement = UpdateMovement(KeyBind.MoveRight, KeyBind.MoveLeft, xMovement);
        yMovement = UpdateMovement(KeyBind.JetUp, KeyBind.JetDown, yMovement);
        zMovement = UpdateMovement(KeyBind.MoveForward, KeyBind.MoveBack, zMovement);
    }

    float UpdateMovement(KeyBind positive, KeyBind negative, float axis) {
        int direction = 0;
        if (InputConverter.GetKey(positive)) direction++;
        if (InputConverter.GetKey(negative)) direction--;

        axis = SnapInput(axis, direction);

        axis = Mathf.MoveTowards(axis, direction, changeRate * Time.deltaTime);

        return axis;
    }

    float SnapInput(float axis, float direction) {
        float difference = axis - direction;
        if (Mathf.Abs(difference) > 1f) {
            axis = 0.0f;
        }
        return axis;
    }
}
