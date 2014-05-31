using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (CapsuleCollider))]

public class NoGravCharacterMotor : MonoBehaviour {

	private PlayerResources resource;

	private MouseLook cameraLook; // Camera Mouse Look
	private Transform cameraTransform;

	private bool jetPackOn;
	private int magnetPower; // 0-4 is on, 5 is off

	public float speed = 10.0f;
	public float rollSpeed = 3.0f;
	public float maxVelocityChange = 10.0f;
	public bool canJump = true;
	public float jumpForce = 40.0f;
	public float airPitchSensitivity = 2.0f;

	private bool grounded = false;
	private bool inAirFlag = false;

	public float fuelSpend = 1.0f;

	void Awake () {
		rigidbody.useGravity = false;
	}


	void Start(){
		rigidbody.freezeRotation = true;
		rigidbody.AddRelativeForce(new Vector3 (0, -jumpForce*4, 0));
		jetPackOn = true;
		magnetPower = 0;

		resource = GetComponent<PlayerResources>();
		cameraTransform = transform.GetChild(1);
		cameraLook = cameraTransform.GetComponent<MouseLook>();

	}

	void FixedUpdate () {
		LockMouseLook(!grounded);

		if (grounded) {

			// Calculate how fast we should be moving
			Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			targetVelocity = transform.TransformDirection(targetVelocity);
			targetVelocity *= speed;
			
			// Apply a force that attempts to reach our target velocity
			Vector3 velocity = rigidbody.velocity;
			Vector3 velocityChange = (targetVelocity - velocity);

			velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
			velocityChange.y = Mathf.Clamp(velocityChange.y, -maxVelocityChange, maxVelocityChange);
			velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);


			rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
			
			// Jump
			if (canJump && Input.GetButton("Jump")) {
				rigidbody.AddRelativeForce (new Vector3(0, CalculateJumpVerticalSpeed(), 0), ForceMode.VelocityChange);
			}
		}else if(jetPackOn){
			if(magnetPower < 5){
				rigidbody.AddRelativeForce(new Vector3 (0, -10, 0));
			}
			//Apply Jetpack force as Acceleration
			Vector3 force = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis ("JetPackUpDown"), Input.GetAxis("Vertical"));
			force *= speed;

			// If non-zero force, spend fuel
			if(force.sqrMagnitude > 0){
				if(resource.SpendFuel(fuelSpend)){
					rigidbody.AddRelativeForce(force, ForceMode.Acceleration);
				}
			}

			//Rotation
			Vector3 torque = new Vector3(Input.GetAxis("Mouse Y")* airPitchSensitivity * cameraLook.GetYDirection(), 0, Input.GetAxis("Roll")); // the change wanted
			torque = torque * rollSpeed;
			transform.Rotate(torque);


		}

		grounded = false;
		magnetPower++;

	}

	void OnCollisionEnter(Collision info){
		Vector3 colNormal = info.contacts[0].normal;
		Vector3 UpNorm = colNormal + transform.up; // Up + Normal = UpNorm


		// Means the collision is on your feet
		if(UpNorm.sqrMagnitude > 3.3f){
			RaycastHit hit;
			Physics.Raycast(transform.position, -transform.up, out hit, 1.5f);

			// Preserve camera angle
			Quaternion curCamRot = cameraTransform.rotation;

			// Rotate main body
			transform.rotation = Quaternion.LookRotation(hit.normal, transform.forward);
			transform.Rotate(new Vector3(-90, 180, 0));


			// Rotate Camera
			cameraTransform.rotation = curCamRot;

			// But the mouse code will overwrite this
			// Find the local X rot
			float xRot = cameraTransform.localEulerAngles.x;

			// Make sure Y and Z local rotations are 0
			cameraTransform.localEulerAngles = new Vector3(cameraTransform.localEulerAngles.x, 0, 0);

			// Make it digestable
			if(xRot > 180){
				xRot -= 360;
			}

			// Set as new X value, need to take negative for god knows why
			cameraLook.SetX_Rotation(-xRot);
		}
	}

	void OnCollisionStay (Collision info) {
		RaycastHit hit;
		if(Physics.Raycast(transform.position, -transform.up, out hit, 1.10f)){
			grounded = true; 
			magnetPower = 0;
		}else{ // Hit a roof or some shit
			rigidbody.AddForce(info.contacts[0].normal, ForceMode.VelocityChange);
		}
		
	}
	void LockMouseLook(bool inAir){
		if(inAir){
			cameraLook.sensitivityY = 0;
			if(!inAirFlag){
				inAirFlag = true;
				transform.Rotate(cameraTransform.localEulerAngles);
				
				cameraLook.SetX_Rotation(0f);
			}

		}else {
			cameraLook.sensitivityY = 10;
			if(inAirFlag){
				inAirFlag = false;
			}
		}
	}

	float CalculateJumpVerticalSpeed () {
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(jumpForce);
	}
}