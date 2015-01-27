using UnityEngine;
using System.Collections;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour {

    private SettingsManager settingsManager;
	private bool ragdoll = false;

	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2, MouseNone }
	public RotationAxes axes = RotationAxes.MouseXAndY;
    public float maxSensitivity = 15f;
    public float sensitivityX = 15F;
	public float sensitivityY = 15F;

	public float minimumX = -360F;
	public float maximumX = 360F;

	public float minimumY = -60F;
	public float maximumY = 60F;

	float rotationY = 0F;

	int yDirection = -1;
	
	private AimingFOVChanger cameraFOV;
	private float zoomCameraSlow;

	void Start(){
        settingsManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<SettingsManager>();
		cameraFOV = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AimingFOVChanger>();

        sensitivityX = settingsManager.xMouseSensitivity;
        sensitivityY = settingsManager.yMouseSensitivity;

	}

	void Update ()
	{
		if(ragdoll) return;

		if(!GameManager.IsPaused()){
            zoomCameraSlow = cameraFOV.zoomRotationRatio();
			if (axes == RotationAxes.MouseXAndY)
			{
                float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX * maxSensitivity * zoomCameraSlow;

                rotationY += Input.GetAxis("Mouse Y") * sensitivityY * maxSensitivity * zoomCameraSlow;
				rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
				
				transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
			}
			else if (axes == RotationAxes.MouseX)
			{
                transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX * maxSensitivity * zoomCameraSlow, 0);
			}
			else if (axes == RotationAxes.MouseY)
			{
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY * maxSensitivity * zoomCameraSlow;
				rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);

				transform.localEulerAngles = new Vector3(rotationY * yDirection, transform.localEulerAngles.y, 0);
			}
		}
	}
	
	public void SetX_Rotation(float newRot){

		rotationY = newRot * yDirection;
		rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);

	}

	public void AddX_Rotation(float addRot){
		rotationY -= addRot * yDirection;
		rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
	}

	// *= this shit dawg
	public void MultYDirection(int input){
		int newIn = Mathf.Clamp(input, -1, 1);

		yDirection *= newIn;
	}
	public void SetYDirection(int input){
		int newIn = Mathf.Clamp(input, -1, 1);
		yDirection = newIn;
	}

	public void Ragdoll(bool state){
		ragdoll = state;
	}

	public int GetYDirection(){
		return yDirection;
	}

}