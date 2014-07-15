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

	public float maxLandingAngle = 45f;
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



	// Input Axes
	float horizontal = 0.0f;
	float vertical = 0.0f;
	float jetPackUpDown = 0.0f;
	float roll = 0.0f;
	bool jetPackInUse = false;

	#region Start
	void Start(){
		manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>();
		jetpackAudio = transform.FindChild("JetpackAudio").GetComponent<AudioSource>();
		feetAudio = transform.FindChild("FeetAudio").GetComponent<AudioSource>();

		rigidbody.freezeRotation = true;
		rigidbody.AddRelativeForce(new Vector3 (0, -jumpForce*4, 0), ForceMode.Acceleration);
		jetPackOn = true;
		magnetPower = 0;

		resource = GetComponent<PlayerResources>();
		cameraTransform = transform.GetChild(0);
		cameraLook = cameraTransform.GetComponent<MouseLook>();

		StartCoroutine("PlayJetpackSound");

		feetAudio.volume = volumeFootsteps;
		StartCoroutine("PlayFeetSound");

	}
	#endregion

	#region UpdateInput
	void UpdateInput(){
		jetPackInUse = false;

		// If exclusive input
		if(Input.GetKey(GameManagerScript.keyBindings[(int)GameManagerScript.KeyBind.MoveForward]) ^ Input.GetKey(GameManagerScript.keyBindings[(int)GameManagerScript.KeyBind.MoveBack]) ){
			//Move forward
			if(Input.GetKey(GameManagerScript.keyBindings[(int)GameManagerScript.KeyBind.MoveForward])){
				if(vertical < 0.0f) vertical = 0.0f;
				vertical = Mathf.MoveTowards(vertical, 1.0f, 3*Time.deltaTime);
				jetPackInUse = true;
			}
			//Move back
			if(Input.GetKey(GameManagerScript.keyBindings[(int)GameManagerScript.KeyBind.MoveBack])){
				if(vertical > 0.0f) vertical = 0.0f;
				vertical = Mathf.MoveTowards(vertical, -1.0f, 3*Time.deltaTime);
				jetPackInUse = true;
			}
		
		}else {// If both or neither
			//Move to rest
			vertical = Mathf.MoveTowards(vertical, 0.0f, 3*Time.deltaTime);
		}

		// If exclusive input
		if(Input.GetKey(GameManagerScript.keyBindings[(int)GameManagerScript.KeyBind.MoveRight]) ^ Input.GetKey(GameManagerScript.keyBindings[(int)GameManagerScript.KeyBind.MoveLeft]) ){
			//Move Right
			if(Input.GetKey(GameManagerScript.keyBindings[(int)GameManagerScript.KeyBind.MoveRight])){
				if(horizontal < 0.0f) horizontal = 0.0f;
				horizontal = Mathf.MoveTowards(horizontal, 1.0f, 3*Time.deltaTime);
				jetPackInUse = true;
			}
			//Move Left
			if(Input.GetKey(GameManagerScript.keyBindings[(int)GameManagerScript.KeyBind.MoveLeft])){
				if(horizontal > 0.0f) horizontal = 0.0f;
				horizontal = Mathf.MoveTowards(horizontal, -1.0f, 3*Time.deltaTime);
				jetPackInUse = true;
			}
			
		}else {// If both or neither
			//Move to rest
			horizontal = Mathf.MoveTowards(horizontal, 0.0f, 3*Time.deltaTime);
		}

		// If exclusive input
		if(Input.GetKey(GameManagerScript.keyBindings[(int)GameManagerScript.KeyBind.JetUp]) ^ Input.GetKey(GameManagerScript.keyBindings[(int)GameManagerScript.KeyBind.JetDown]) ){
			//Move forward
			if(Input.GetKey(GameManagerScript.keyBindings[(int)GameManagerScript.KeyBind.JetUp])){
				if(jetPackUpDown < 0.0f) jetPackUpDown = 0.0f;
				jetPackUpDown = Mathf.MoveTowards(jetPackUpDown, 1.0f, 3*Time.deltaTime);
				jetPackInUse = true;
			}
			//Move back
			if(Input.GetKey(GameManagerScript.keyBindings[(int)GameManagerScript.KeyBind.JetDown])){
				if(jetPackUpDown > 0.0f) jetPackUpDown = 0.0f;
				jetPackUpDown = Mathf.MoveTowards(jetPackUpDown, -1.0f, 3*Time.deltaTime);
				jetPackInUse = true;
			}
			
		}else {// If both or neither
			//Move to rest
			jetPackUpDown = Mathf.MoveTowards(jetPackUpDown, 0.0f, 3*Time.deltaTime);
		}

		// If exclusive input
		if(Input.GetKey(GameManagerScript.keyBindings[(int)GameManagerScript.KeyBind.RollRight]) ^ Input.GetKey(GameManagerScript.keyBindings[(int)GameManagerScript.KeyBind.RollLeft]) ){
			//Move forward
			if(Input.GetKey(GameManagerScript.keyBindings[(int)GameManagerScript.KeyBind.RollRight])){
				if(roll > 0.0f) roll = 0.0f;
				roll = Mathf.MoveTowards(roll, -1.0f, 3*Time.deltaTime);
			}
			//Move back
			if(Input.GetKey(GameManagerScript.keyBindings[(int)GameManagerScript.KeyBind.RollLeft])){
				if(roll < 0.0f) roll = 0.0f;
				roll = Mathf.MoveTowards(roll, 1.0f, 3*Time.deltaTime);
			}
			
		}else {// If both or neither
			//Move to rest
			roll = Mathf.MoveTowards(roll, 0.0f, 3*Time.deltaTime);
		}
	}
	#endregion

	void FixedUpdate () {
		LockMouseLook(!grounded);
		playJetSound = false;
		playJetEmpty = false;
		playWalkingSound = false;

		UpdateInput();

		if (grounded) {

			// Calculate how fast we should be moving
			Vector3 targetVelocity;
			if(manager.IsPaused()){
				targetVelocity = Vector3.zero;
			}else{
				targetVelocity = new Vector3(horizontal, 0, vertical);
			}

			targetVelocity = Vector3.ClampMagnitude(targetVelocity, 1.0f);
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
			if (canJump && Input.GetKey(GameManagerScript.keyBindings[(int)GameManagerScript.KeyBind.JetUp]) && !manager.IsPaused()) {
				rigidbody.AddRelativeForce (new Vector3(0, CalculateJumpVerticalSpeed(), 0), ForceMode.VelocityChange);
			}
		}else if(jetPackOn){
			if(magnetPower < 5 && Physics.Raycast(transform.position, -transform.up, 1.5f)){
				rigidbody.AddRelativeForce(new Vector3 (0, -1, 0), ForceMode.VelocityChange);
			}
			//Apply Jetpack force as Acceleration
			Vector3 force;
			if(manager.IsPaused()){
				force = Vector3.zero;
			}else{
				force = new Vector3(horizontal, jetPackUpDown, vertical);
				force = Vector3.ClampMagnitude(force, 1.0f);
			}
			force *= speed;

			// If non-zero force, spend fuel
			if(jetPackInUse && !manager.IsPaused()){
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
				torque = new Vector3(Input.GetAxis("Mouse Y")* airPitchSensitivity * cameraLook.GetYDirection(), 0, roll); // the change wanted
			}
			torque = torque * rollSpeed;
			transform.Rotate(torque);


		}

		grounded = false;
		magnetPower++;



	}
	#region Play Sounds
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
	#endregion

	#region Collisions
	void OnCollisionEnter(Collision info){
		if(networkView.isMine){
			if(info.collider.CompareTag("Walkable")){
				// Find angle between vectors

				float angle = Vector3.Angle(-transform.up, info.contacts[0].normal);
				Debug.Log(angle.ToString());
				RaycastHit hit;

				if(angle > 180 - maxLandingAngle && Physics.Raycast(transform.position, -transform.up, 2.0f)){

					// Preserve camera angle
					Quaternion curCamRot = cameraTransform.rotation;

					transform.rotation = Quaternion.LookRotation(info.contacts[0].normal, transform.forward);
					transform.Rotate(new Vector3(-90, 180, 0));

					Physics.Raycast(transform.position, -transform.up, out hit, 2.0f);

					transform.rotation = Quaternion.LookRotation(hit.normal, transform.forward);
					transform.Rotate(new Vector3(-90, 180, 0));

					// Rotate Camera
					cameraTransform.rotation = curCamRot;

					// But the mouse code will overwrite this
					// Find the local X rot
					float xRot = cameraTransform.localEulerAngles.x;
					float yRot = cameraTransform.localEulerAngles.y;


					// Make sure Y and Z local rotations are 0
					cameraTransform.localEulerAngles = new Vector3(cameraTransform.localEulerAngles.x, 0, 0);
					transform.Rotate(0, yRot, 0);

					// Make it digestable
					if(xRot > 180){
						xRot -= 360;
					}

					cameraLook.SetX_Rotation(xRot);
				}
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
	#endregion
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
	public void Recoil(){
		if(inAirFlag){
			transform.Rotate(-1, 0, 0);
		}else{
			cameraLook.AddX_Rotation(1);
		}
	}
}