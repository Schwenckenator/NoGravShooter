using UnityEngine;
using System.Collections;

public interface IControllerInput {
    bool canWalk { get; set; }
    bool canJump { get; set; }

    float GetRollMovement();
    float GetXMovement();
    float GetYMovement();
    float GetZMovement();

    bool IsMovementKeys();
    bool IsStopKey();

}
public class KeyboardInput : MonoBehaviour, IControllerInput{
    
    float xMovement = 0; // Left - Right
    float yMovement = 0; // Up  - Down
    float zMovement = 0; // Forward - Back
    float rollMovement = 0;

    int changeRate = 3; // * deltaTime

    // Movement restriction Flags
    // For tutorial Only
    public bool canWalk { get; set; }
    public bool canJump { get; set; }

    #region ExternalAccessors
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
        return InputKey.GetKey(key);
    }

    #endregion

    #region Internal Logic
    void Awake() {
        canWalk = true;
        canJump = true;
    }
    void Update() {
        rollMovement = UpdateMovement(KeyBind.RollLeft, KeyBind.RollRight, rollMovement);
        xMovement = UpdateMovement(KeyBind.MoveRight, KeyBind.MoveLeft, xMovement);
        yMovement = UpdateMovement(KeyBind.JetUp, KeyBind.JetDown, yMovement);
        zMovement = UpdateMovement(KeyBind.MoveForward, KeyBind.MoveBack, zMovement);

        RestrictMovement();
    }

    float UpdateMovement(KeyBind positive, KeyBind negative, float axis) {
        int direction = 0;
        if (InputKey.GetKey(positive)) direction++;
        if (InputKey.GetKey(negative)) direction--;

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
    void RestrictMovement() {
        if (!canWalk) {
            xMovement = 0;
            zMovement = 0;
        }
        if (!canJump) {
            yMovement = 0;
        }
    }
    #endregion
}
