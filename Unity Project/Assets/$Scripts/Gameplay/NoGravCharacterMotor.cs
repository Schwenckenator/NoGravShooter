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

	private bool ragdoll = false;



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

	void OnGUI(){
		if(!ragdoll) return;
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), manager.GetComponent<GUIScript>().bloodyScreen);
	}

	#region UpdateInput
	void UpdateInput(){
		if(ragdoll) return;

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

	#region FixedUpdate
	void FixedUpdate () {
		if(ragdoll) return;

		LockMouseLook(!grounded);
		playJetSound = false;
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

			Vector3 totalPushBackDir = Vector3.zero;
			//check for edges

			bool sneaking = false;
			if(Input.GetKey(GameManagerScript.keyBindings[(int)GameManagerScript.KeyBind.JetDown])){
				sneaking = true;
				int numOfVectors = 16;
				//Fire rays!
				Vector3 rayPosition = transform.TransformPoint(0, -0.5f, 0);
				Vector3 rayDown = transform.TransformDirection(0, -1, 0);

				Vector3[] dirs = new Vector3[numOfVectors];
				int vec = 0;
				dirs[vec++] = transform.TransformDirection(0, 0, 1);
				dirs[vec++] = transform.TransformDirection(0.5f, 0, 1);

				dirs[vec++] = transform.TransformDirection(.707f, 0, .707f);
				dirs[vec++] = transform.TransformDirection(1, 0, 0.5f);

				dirs[vec++] = transform.TransformDirection(1, 0, 0);
				dirs[vec++] = transform.TransformDirection(1, 0, -0.5f);

				dirs[vec++] = transform.TransformDirection(.707f, 0, -.707f);
				dirs[vec++] = transform.TransformDirection(0.5f, 0, -1);

				
				dirs[vec++] = transform.TransformDirection(0, 0, -1);
				dirs[vec++] = transform.TransformDirection(-0.5f, 0, -1);

				dirs[vec++] = transform.TransformDirection(-.707f, 0, -.707f);
				dirs[vec++] = transform.TransformDirection(-1, 0, -0.5f);

				dirs[vec++] = transform.TransformDirection(-1, 0, 0);
				dirs[vec++] = transform.TransformDirection(-1, 0, 0.5f);

				dirs[vec++] = transform.TransformDirection(-.707f, 0, .707f);
				dirs[vec++] = transform.TransformDirection(-0.5f, 0, 1);





				bool[] misses = new bool[numOfVectors];
				float rayLength = 3.0f;
				Vector3 pushBackDir = Vector3.zero;


				for(int i=0; i<numOfVectors; i++){
					misses[i] = !Physics.Raycast(rayPosition, (dirs[i]+rayDown).normalized, rayLength);
				}

				for(int i=0; i<numOfVectors; i++){
					if(misses[i] && misses[(i+1)%numOfVectors]){
						//Found an edge!
						Vector3 edge = dirs[(i+1)%numOfVectors] - dirs[i];
						//Debug.DrawLine(transform.position + dirs[i], transform.position + dirs[(i+1)%8], Color.magenta, 30, false);

						Vector3 up = transform.up;
						Vector3.OrthoNormalize(ref up, ref edge, ref pushBackDir);
						totalPushBackDir += pushBackDir;

					}
				}
			}
			if(sneaking && totalPushBackDir.sqrMagnitude > 0){
				totalPushBackDir.Normalize();
				Debug.DrawRay(transform.position, totalPushBackDir, Color.magenta, 10, false);
				Debug.Log (totalPushBackDir.ToString("G4"));
				//Make every value positive
				totalPushBackDir = new Vector3(1 - Mathf.Abs(totalPushBackDir.x), 1 - Mathf.Abs(totalPushBackDir.y), 1 - Mathf.Abs(totalPushBackDir.z));

				targetVelocity = Vector3.Scale(targetVelocity, totalPushBackDir);
			}
			//Multiply by speed
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
					jetpackSoundWasPlayed = false;
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
	#endregion

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
				RaycastHit[] hits;
				Vector3 colObjNorm = Vector3.zero;
				Ray r = new Ray(transform.position, info.collider.transform.position - transform.position);

				hits = Physics.RaycastAll(r);
				foreach(RaycastHit hit in hits){
					if(hit.collider == info.collider){
						colObjNorm = hit.normal;
						break;
					}
				}

				float angle = Vector3.Angle(transform.up, colObjNorm);

				if(angle < maxLandingAngle){

					// Preserve camera angle
					Quaternion curCamRot = cameraTransform.rotation;

					transform.rotation = Quaternion.LookRotation(colObjNorm, transform.forward);
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

	public void Ragdoll(bool state){
		ragdoll = state;
		if(ragdoll){
			rigidbody.constraints = RigidbodyConstraints.None;
			rigidbody.AddTorque(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1), ForceMode.VelocityChange);
		}else{
			rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
		}
		cameraLook.Ragdoll(state);
		GetComponent<MouseLook>().Ragdoll(state);
	}
}