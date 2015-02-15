﻿using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (CapsuleCollider))]

public class NoGravCharacterMotor : MonoBehaviour {

	private GameManager gameManager;
	private AudioSource jetpackAudio;
	private AudioSource feetAudio;

	private PlayerResources resource;

	private MouseLook cameraMouseLook;
    private MouseLook characterMouseLook;
	private Transform cameraTransform;
    private float colliderHeight;

	private bool jetPackOn;
	private int magnetPower; // 1-5 is on, 0 is off
    private int magnetPowerMax = 5;

    [SerializeField]
    private AudioClip[] soundFootsteps;
    [SerializeField]
    private float volumeFootsteps;
    [SerializeField]
    private AudioClip soundJetpack;
    [SerializeField]
    private float volumeJetpack;
    [SerializeField]
    private AudioClip soundJetpackEmpty;
    [SerializeField]
    private float volumeJetpackEmpty;
    [SerializeField]
    private AudioClip soundJetpackShutoff;
    [SerializeField]
    private float volumeJetpackShutoff;
    [SerializeField]

	private bool jetpackSoundWasPlayed = false;
	private bool playJetSound = false;
	private bool playWalkingSound = false;

    [SerializeField]
    private float maxLandingAngle = 45f;
    [SerializeField]
    private float speed = 10.0f;
    [SerializeField]
    private float rollSpeed = 3.0f;
    [SerializeField]
    private float maxVelocityChange = 10.0f;
    [SerializeField]
    private bool canJump = true;
    [SerializeField]
    private float jumpForce = 40.0f;



    [SerializeField]
    private float sqrWalkingSoundVelocity;

	private bool grounded = false;
	private bool inAirFlag = false;

    [SerializeField]
    private float fuelSpend = 0.5f;

	private bool ragdoll = false;



	// Input Axes
	float horizontal = 0.0f;
	float vertical = 0.0f;
	float jetPackUpDown = 0.0f;
	float roll = 0.0f;
	bool jetPackInUse = false;

	#region Start
	void Start(){
        GameObject manager = GameObject.FindGameObjectWithTag("GameController");
		gameManager = manager.GetComponent<GameManager>();

		jetpackAudio = transform.FindChild("JetpackAudio").GetComponent<AudioSource>();
		feetAudio = transform.FindChild("FeetAudio").GetComponent<AudioSource>();

		rigidbody.freezeRotation = true;
		rigidbody.AddRelativeForce(new Vector3 (0, -jumpForce*4, 0), ForceMode.Force);
		jetPackOn = true;
		magnetPower = magnetPowerMax;

		resource = GetComponent<PlayerResources>();
		cameraTransform = transform.GetChild(0);
		cameraMouseLook = cameraTransform.GetComponent<MouseLook>();
        characterMouseLook = GetComponent<MouseLook>();
        colliderHeight = GetComponent<CapsuleCollider>().height;

		StartCoroutine("PlayJetpackSound");

		feetAudio.volume = volumeFootsteps;
		StartCoroutine("PlayFeetSound");

	}
	#endregion

	void OnGUI(){
		if(!ragdoll) return;
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), gameManager.GetComponent<GuiManager>().bloodyScreen);
	}

	#region UpdateInput
	void UpdateInput(){
		if(ragdoll) return;

		jetPackInUse = false;

        InputForward();

        InputRight();

        InputUp();

        InputRoll();
	}

    private void InputRoll() {
        // If exclusive input
        if (Input.GetKey(SettingsManager.keyBindings[(int)SettingsManager.KeyBind.RollRight]) ^ Input.GetKey(SettingsManager.keyBindings[(int)SettingsManager.KeyBind.RollLeft])) {
            //Roll Right
            if (Input.GetKey(SettingsManager.keyBindings[(int)SettingsManager.KeyBind.RollRight])) {
                if (roll > 0.0f) roll = 0.0f;
                roll = Mathf.MoveTowards(roll, -1.0f, 3 * Time.deltaTime);
            }
            //Roll Left
            if (Input.GetKey(SettingsManager.keyBindings[(int)SettingsManager.KeyBind.RollLeft])) {
                if (roll < 0.0f) roll = 0.0f;
                roll = Mathf.MoveTowards(roll, 1.0f, 3 * Time.deltaTime);
            }

        } else {// If both or neither
            //Move to rest
            roll = Mathf.MoveTowards(roll, 0.0f, 3 * Time.deltaTime);
        }
    }

    private void InputUp() {
        // If exclusive input
        if (Input.GetKey(SettingsManager.keyBindings[(int)SettingsManager.KeyBind.JetUp]) ^ Input.GetKey(SettingsManager.keyBindings[(int)SettingsManager.KeyBind.JetDown])) {
            jetPackInUse = true;
            //Move Up
            if (Input.GetKey(SettingsManager.keyBindings[(int)SettingsManager.KeyBind.JetUp])) {
                if (jetPackUpDown < 0.0f) jetPackUpDown = 0.0f;
                jetPackUpDown = Mathf.MoveTowards(jetPackUpDown, 1.0f, 3 * Time.deltaTime);
            }
            //Move Down
            if (Input.GetKey(SettingsManager.keyBindings[(int)SettingsManager.KeyBind.JetDown])) {
                if (jetPackUpDown > 0.0f) jetPackUpDown = 0.0f;
                jetPackUpDown = Mathf.MoveTowards(jetPackUpDown, -1.0f, 3 * Time.deltaTime);
            }

        } else {// If both or neither
            //Move to rest
            jetPackUpDown = Mathf.MoveTowards(jetPackUpDown, 0.0f, 3 * Time.deltaTime);
        }
    }

    private void InputRight() {
        // If exclusive input
        if (Input.GetKey(SettingsManager.keyBindings[(int)SettingsManager.KeyBind.MoveRight]) ^ Input.GetKey(SettingsManager.keyBindings[(int)SettingsManager.KeyBind.MoveLeft])) {
            jetPackInUse = true;
            //Move Right
            if (Input.GetKey(SettingsManager.keyBindings[(int)SettingsManager.KeyBind.MoveRight])) {
                if (horizontal < 0.0f) horizontal = 0.0f;
                horizontal = Mathf.MoveTowards(horizontal, 1.0f, 3 * Time.deltaTime);
            }
            //Move Left
            if (Input.GetKey(SettingsManager.keyBindings[(int)SettingsManager.KeyBind.MoveLeft])) {
                if (horizontal > 0.0f) horizontal = 0.0f;
                horizontal = Mathf.MoveTowards(horizontal, -1.0f, 3 * Time.deltaTime);
            }

        } else {// If both or neither
            //Move to rest
            horizontal = Mathf.MoveTowards(horizontal, 0.0f, 3 * Time.deltaTime);
        }
    }

    private void InputForward() {
        // If exclusive input
        if (Input.GetKey(SettingsManager.keyBindings[(int)SettingsManager.KeyBind.MoveForward]) ^ Input.GetKey(SettingsManager.keyBindings[(int)SettingsManager.KeyBind.MoveBack])) {
            jetPackInUse = true;
            //Move Forward
            if (Input.GetKey(SettingsManager.keyBindings[(int)SettingsManager.KeyBind.MoveForward])) {
                if (vertical < 0.0f) vertical = 0.0f;
                vertical = Mathf.MoveTowards(vertical, 1.0f, 3 * Time.deltaTime);
            }
            //Move Back
            if (Input.GetKey(SettingsManager.keyBindings[(int)SettingsManager.KeyBind.MoveBack])) {
                if (vertical > 0.0f) vertical = 0.0f;
                vertical = Mathf.MoveTowards(vertical, -1.0f, 3 * Time.deltaTime);
            }

        } else {// If both or neither
            //Move to rest
            vertical = Mathf.MoveTowards(vertical, 0.0f, 3 * Time.deltaTime);
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
            MovementWalk();
		}else if(jetPackOn){
            MovementJetpack();

		}
		grounded = false;
		magnetPower--;
	}

    private void MovementJetpack() {
        if (magnetPower > 0 && Physics.Raycast(transform.position, -transform.up, (colliderHeight * 2 / 3))) {
            rigidbody.AddRelativeForce(new Vector3(0, -1, 0), ForceMode.Impulse);
        }
        //Apply Jetpack force as Acceleration
        Vector3 force = GetJetpackForce();

        // If non-zero force, spend fuel
        if (jetPackInUse && !GameManager.IsPlayerMenu()) {
            if (resource.SpendFuel(fuelSpend)) {
                playJetSound = true;
                rigidbody.AddRelativeForce(force, ForceMode.Force);
            } else {
                jetpackSoundWasPlayed = false;
            }
        }

        Vector3 torque = GetJetpackTorque();
        transform.Rotate(torque);
    }

    private Vector3 GetJetpackTorque() {
        Vector3 torque;
        if (GameManager.IsPlayerMenu()) {
            torque = Vector3.zero;
        } else {
            torque = new Vector3(0, 0, roll); // the change wanted
        }
        torque = torque * rollSpeed;
        return torque;
    }

    private Vector3 GetJetpackForce() {
        Vector3 force;
        if (GameManager.IsPlayerMenu()) {
            force = Vector3.zero;
        } else {
            force = new Vector3(horizontal, jetPackUpDown, vertical);
            force = Vector3.ClampMagnitude(force, 1.0f);
        }
        force *= speed;
        return force;
    }

    private void MovementWalk() {
        // Calculate how fast we should be moving
        Vector3 targetVelocity;
        if (GameManager.IsPlayerMenu()) {
            targetVelocity = Vector3.zero;
        } else {
            targetVelocity = new Vector3(horizontal, 0, vertical);
        }

        targetVelocity = Vector3.ClampMagnitude(targetVelocity, 1.0f);
        targetVelocity = transform.TransformDirection(targetVelocity);

        Vector3 totalPushBackDir = Vector3.zero;
        //check for edges

        bool sneaking = false;
        if (Input.GetKey(SettingsManager.keyBindings[(int)SettingsManager.KeyBind.JetDown])) {
            sneaking = true;
            totalPushBackDir = EdgeDetection();
        }
        if (sneaking && totalPushBackDir.sqrMagnitude > 0) {
            totalPushBackDir.Normalize();
            Debug.DrawRay(transform.position, totalPushBackDir, Color.magenta, 10, false);
            Debug.Log(totalPushBackDir.ToString("G4"));
            //Make every value positive
            totalPushBackDir = new Vector3(1 - Mathf.Abs(totalPushBackDir.x), 1 - Mathf.Abs(totalPushBackDir.y), 1 - Mathf.Abs(totalPushBackDir.z));

            targetVelocity = Vector3.Scale(targetVelocity, totalPushBackDir);
        }
        //Multiply by speed
        targetVelocity *= speed;

        // Apply a force that attempts to reach our target velocity
        Vector3 velocity = rigidbody.velocity;
        Vector3 velocityChange = (targetVelocity - velocity);

        if (rigidbody.velocity.sqrMagnitude > sqrWalkingSoundVelocity) {
            playWalkingSound = true;
        }

        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = Mathf.Clamp(velocityChange.y, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);


        rigidbody.AddForce(velocityChange, ForceMode.Impulse);

        // Jump
        if (canJump && Input.GetKey(SettingsManager.keyBindings[(int)SettingsManager.KeyBind.JetUp]) && !GameManager.IsPlayerMenu()) {
            rigidbody.AddRelativeForce(new Vector3(0, CalculateJumpVerticalSpeed(), 0), ForceMode.Impulse);
        }
    }

    private Vector3 EdgeDetection() {
        Vector3 totalPushBackDir = new Vector3();
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


        for (int i = 0; i < numOfVectors; i++) {
            misses[i] = !Physics.Raycast(rayPosition, (dirs[i] + rayDown).normalized, rayLength);
        }

        for (int i = 0; i < numOfVectors; i++) {
            if (misses[i] && misses[(i + 1) % numOfVectors]) {
                //Found an edge!
                Vector3 edge = dirs[(i + 1) % numOfVectors] - dirs[i];
                //Debug.DrawLine(transform.position + dirs[i], transform.position + dirs[(i+1)%8], Color.magenta, 30, false);

                Vector3 up = transform.up;
                Vector3.OrthoNormalize(ref up, ref edge, ref pushBackDir);
                totalPushBackDir += pushBackDir;

            }
        }

        return totalPushBackDir;
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

	// Returns normal of surface
	Vector3 SurfaceNormal(Collision info){
		RaycastHit[] hits;
		Vector3 collisionObjNormal = Vector3.zero;

		hits = Physics.RaycastAll(GeneratePhysicsRay());
		foreach(RaycastHit hit in hits){
			if(hit.collider == info.collider){
				collisionObjNormal = hit.normal;
				break;
			}
		}
	
		return collisionObjNormal;
	}

    Ray GeneratePhysicsRay() {
        Ray r = new Ray();
        r.origin = transform.position;
        r.direction = -transform.up * (colliderHeight *  2/3);

        Debug.DrawRay(r.origin, r.direction, Color.green, 5.0f, false);

        return r;
    }
    /// <summary>
    /// Does the actual rotation of player
    /// </summary>
	void SnapToSurface(Vector3 colObjNorm){
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
		
		cameraMouseLook.SetX_Rotation(xRot);
	}

	#region Collisions
	void OnCollisionStay (Collision info) {
		if(!networkView.isMine || info.collider.CompareTag("NonWalkable")) return;

        Vector3 surfaceNorm = SurfaceNormal(info);
        float angle = Vector3.Angle(transform.up, surfaceNorm);

        if (angle > 1f && angle < maxLandingAngle) {
            SnapToSurface(surfaceNorm);
        }

        RaycastHit hit;
        Ray r = new Ray(transform.position, -transform.up);
        if (Physics.Raycast(r, out hit, (colliderHeight * 2 / 3))){
            grounded = true;
            magnetPower = magnetPowerMax;
        } else { // Hit a roof or some shit
            rigidbody.AddForce(info.contacts[0].normal, ForceMode.Impulse);
        }
	}
	#endregion
	void LockMouseLook(bool inAir){
		if(inAir){
            cameraMouseLook.axes = MouseLook.RotationAxes.MouseNone;
            characterMouseLook.axes = MouseLook.RotationAxes.MouseXAndY;
			FixRotation();

			if(!inAirFlag){
				inAirFlag = true;
			}

		}else {
            cameraMouseLook.axes = MouseLook.RotationAxes.MouseY;
            characterMouseLook.axes = MouseLook.RotationAxes.MouseX;
			if(inAirFlag){
				inAirFlag = false;
			}
		}
	}

	void FixRotation(){
		transform.Rotate(cameraTransform.localEulerAngles);
		cameraMouseLook.SetX_Rotation(0f);
	}


	float CalculateJumpVerticalSpeed () {
		return Mathf.Sqrt(jumpForce);
	}
	public void Recoil(float recoil){
		if(inAirFlag){
			transform.Rotate(-recoil, 0, 0);
		}else{
			cameraMouseLook.AddX_Rotation(recoil);
		}
	}

	public void Ragdoll(bool state){
		ragdoll = state;
		if(ragdoll){
			rigidbody.constraints = RigidbodyConstraints.None;
			rigidbody.AddTorque(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1), ForceMode.Impulse);
		}else{
			rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
		}
		cameraMouseLook.Ragdoll(state);
		GetComponent<MouseLook>().Ragdoll(state);
	}
}