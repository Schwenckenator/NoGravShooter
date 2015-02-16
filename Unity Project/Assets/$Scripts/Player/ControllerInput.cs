using UnityEngine;
using System.Collections;

public interface IControllerInput {


    float GetRollMovement();
    float GetXMovement();
    float GetYMovement();
    float GetZMovement();
    bool IsMovementKeys();

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
    
    public bool IsMovementKeys() {
        return IsDown(SettingsManager.KeyBind.MoveRight) ||
            IsDown(SettingsManager.KeyBind.MoveLeft) ||

            IsDown(SettingsManager.KeyBind.JetUp) ||
            IsDown(SettingsManager.KeyBind.JetDown) ||

            IsDown(SettingsManager.KeyBind.MoveForward) ||
            IsDown(SettingsManager.KeyBind.MoveBack);
    }
    private bool IsDown(SettingsManager.KeyBind key) {
        return InputConverter.GetKey(key);
    }

    void Update() {
        rollMovement = UpdateMovement(SettingsManager.KeyBind.RollLeft, SettingsManager.KeyBind.RollRight, rollMovement);
        xMovement = UpdateMovement(SettingsManager.KeyBind.MoveRight, SettingsManager.KeyBind.MoveLeft, xMovement);
        yMovement = UpdateMovement(SettingsManager.KeyBind.JetUp, SettingsManager.KeyBind.JetDown, yMovement);
        zMovement = UpdateMovement(SettingsManager.KeyBind.MoveForward, SettingsManager.KeyBind.MoveBack, zMovement);
    }

    float UpdateMovement(SettingsManager.KeyBind positive, SettingsManager.KeyBind negative, float axis) {
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
