using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (CapsuleCollider))]

public class NoGravCharacterMotor : MonoBehaviour {

	private GameManagerScript manager;
	private AudioSource jetpackAudio;
	private AudioSource feetAudio;

	private PlayerResources resource;

	private MouseLook cameraLook; // Camera Mouse Look
	private Transform cameraTransform;

	private bool jetPackOn;
	private int magnetPower; // 0-4 is on, 5 is off

	public AudioClip[] soundFootsteps;
	public float volumeFootsteps;
	public AudioClip soundJetpack;
	public float volumeJetpack;
	public AudioClip soundJetpackEmpty;
	public float volumeJetpackEmpty;
	public AudioClip soundJetpackShutoff;
	public float volumeJetpackShutoff;

	private bool jetpackSoundWasPlayed = false;
	private bool playJetSound = false;
	private bool playJetEmpty = false;
	private bool playWalkingSound = false;

	public float speed = 10.0f;
	public float rollSpeed = 3.0f;
	public float maxVelocityChange = 10.0f;
	public bool canJump = true;
	public float jumpForce = 40.0f;
	public float airPitchSensitivity = 2.0f;

	public float sqrWalkingSoundVelocity;

	private bool grounded = false;
	private bool inAirFlag = false;

	public float fuelSpend = 0.5f;


	void Start(){
		manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>();
		jetpackAudio = transform.FindChild("JetpackAudio").GetComponent<AudioSource>();
		feetAudio = transform.FindChild("FeetAudio").GetComponent<AudioSource>();

		rigidbody.freezeRotation = true;
		rigidbody.AddRelativeForce(new Vector3 (0, -jumpForce*4, 0));
		jetPackOn = true;
		magnetPower = 0;

		resource = GetComponent<PlayerResources>();
		cameraTransform = transform.GetChild(0);
		cameraLook = cameraTransform.GetComponent<MouseLook>();

		StartCoroutine("PlayJetpackSound");

		feetAudio.volume = volumeFootsteps;
		StartCoroutine("PlayFeetSound");
	}

	void FixedUpdate () {
		LockMouseLook(!grounded);
		playJetSound = false;
		playJetEmpty = false;
		playWalkingSound = false;


		if (grounded) {

			// Calculate how fast we should be moving
			Vector3 targetVelocity;
			if(manager.IsPaused()){
				targetVelocity = Vector3.zero;
			}else{
				targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			}

			targetVelocity = transform.TransformDirection(targetVelocity);
			targetVelocity *= speed;
			
			// Apply a force that attempts to reach our target velocity
			Vector3 velocity = rigidbody.velocity;
			Vector3 velocityChange = (targetVelocity - velocity);

			if(rigidbody.velocity.sqrMagnitude > sqrWalkingSoundVelocity){
				playWalkingSound = true;
			}

			velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
			velocityChange.y = Mathf.Clamp(velocityChange.y, -maxVelocityChange, maxVelocityChange);
			velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);


			rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
			
			// Jump
			if (canJump && Input.GetButton("Jump") && !manager.IsPaused()) {
				rigidbody.AddRelativeForce (new Vector3(0, CalculateJumpVerticalSpeed(), 0), ForceMode.VelocityChange);
			}
		}else if(jetPackOn){
			if(magnetPower < 5){
				rigidbody.AddRelativeForce(new Vector3 (0, -10, 0));
			}
			//Apply Jetpack force as Acceleration
			Vector3 force;
			if(manager.IsPaused()){
				force = Vector3.zero;
			}else{
				force = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis ("JetPackUpDown"), Input.GetAxis("Vertical"));
			}
			force *= speed;

			// If non-zero force, spend fuel
			if(new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw ("JetPackUpDown"), Input.GetAxisRaw("Vertical")).sqrMagnitude > 0 && !manager.IsPaused()){
				if(resource.SpendFuel(fuelSpend)){
					playJetSound = true;
					rigidbody.AddRelativeForce(force, ForceMode.Acceleration);
				}else{
					playJetEmpty = true;
				}
			}

			//Rotation
			Vector3 torque;
			if(manager.IsPaused()){
				torque = Vector3.zero;
			}else{
				torque = new Vector3(Input.GetAxis("Mouse Y")* airPitchSensitivity * cameraLook.GetYDirection(), 0, Input.GetAxis("Roll")); // the change wanted
			}
			torque = torque * rollSpeed;
			transform.Rotate(torque);


		}

		grounded = false;
		magnetPower++;



	}
	IEnumerator PlayJetpackSound(){
		while(true){
			if(playJetSound){
				jetpackSoundWasPlayed = true;
				if(!jetpackAudio.isPlaying || jetpackAudio.clip != soundJetpack){
					jetpackAudio.clip = soundJetpack;
					jetpackAudio.volume = volumeJetpack;
					jetpackAudio.Play();
				}
			}else if(playJetEmpty){
				jetpackSoundWasPlayed = true;
				if(!jetpackAudio.isPlaying || jetpackAudio.clip != soundJetpackEmpty){
					jetpackAudio.clip = soundJetpackEmpty;
					jetpackAudio.volume = volumeJetpackEmpty;
					jetpackAudio.Play ();
				}
			}else if(jetpackSoundWasPlayed){
				jetpackAudio.clip = soundJetpackShutoff;
				jetpackAudio.volume = volumeJetpackShutoff;
				jetpackAudio.Play();
				jetpackSoundWasPlayed = false;
			}
			yield return null;
		}

	}
	IEnumerator PlayFeetSound(){
		int stepKind = 0;
		while(true){
			if(playWalkingSound){
				feetAudio.clip = soundFootsteps[stepKind];
				feetAudio.Play();
				yield return new WaitForSeconds(soundFootsteps[stepKind].length);
				stepKind = (stepKind+1)%2;
			}else{
				stepKind = 0;
				feetAudio.Stop();
			}
			yield return null;
		}
	}

	void OnCollisionEnter(Collision info){
		if(networkView.isMine){
			RaycastHit hit;
			if(Physics.Raycast(transform.position, -transform.up, out hit, 1.5f)){

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
				cameraLook.SetX_Rotation(xRot);
			}
		}
	}

	void OnCollisionStay (Collision info) {
		if(networkView.isMine){
			RaycastHit hit;
			if(Physics.Raycast(transform.position, -transform.up, out hit, 1.10f)){
				grounded = true; 
				magnetPower = 0;
			}else{ // Hit a roof or some shit
				rigidbody.AddForce(info.contacts[0].normal, ForceMode.VelocityChange);
			}
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