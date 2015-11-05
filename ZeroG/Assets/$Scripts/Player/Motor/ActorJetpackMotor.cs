using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class ActorJetpackMotor : MonoBehaviour, IActorMotor, IResetable {

    public AudioSource jetpackAudioSource;

    public AudioClip soundJetpackBurn;
    public float volumeJetpackBurn;
    public AudioClip soundJetpackEmpty;
    public float volumeJetpackEmpty;
    public AudioClip soundJetpackShutoff;
    public float volumeJetpackShutoff;
    public float rollSpeed = 3.0f;

    //bool ragdoll = false;
    bool jetpackSoundWasPlayed = false;
    bool playJetSound = false;

    new Rigidbody rigidbody;
    ActorJetpackFuel jetpack;

    IControllerInput input;
    IActorStats stats;

    void Start() {
        stats = gameObject.GetInterface<IActorStats>();

        rigidbody = GetComponent<Rigidbody>();
        input = gameObject.GetInterface<IControllerInput>();

        rigidbody.freezeRotation = true;

        jetpack = GetComponent<ActorJetpackFuel>();

        Reset();
    }

    public void Reset() {
        StopAllCoroutines();
    }

    // Update is called once per frame
    void Update() {
        Sound();
    }

    public void Movement() {
        Vector3 torque = GetJetpackTorque();
        transform.Rotate(torque);

        Vector3 force = GetJetpackForce();

        // If non-zero force, spend fuel
        if (force.sqrMagnitude > 0 && (input.IsMovementKeys() || input.IsStopKey()) && !UIPauseSpawn.IsShown) {
            if (jetpack.SpendFuel(stats.fuelSpend)) {
                playJetSound = true;
                rigidbody.AddRelativeForce(force);
            } else {
                playJetSound = false;
            }
        } else {
            playJetSound = false;
        }
    }
    private Vector3 GetJetpackTorque() {
        Vector3 torque;

        torque = new Vector3(0, 0, input.GetRollMovement()); // the change wanted
        torque = torque * rollSpeed;
        return torque;
    }
    private Vector3 GetJetpackForce() {
        Vector3 force = Vector3.zero;
        if (input.IsMovementKeys()) {
            force = new Vector3(input.GetXMovement(), input.GetYMovement(), input.GetZMovement());
            force = Vector3.ClampMagnitude(force, 1.0f);
        } else if (input.IsStopKey()) {
            // If very close to stopping
            if (rigidbody.velocity.sqrMagnitude < 0.001f) {
                force = Vector3.zero;
                rigidbody.velocity = Vector3.zero;
            } else {
                force = StopJetpackMovementForce(transform.InverseTransformDirection(rigidbody.velocity));
            }
        }

        force *= stats.speed;
        return force;
    }

    private Vector3 StopJetpackMovementForce(Vector3 velocity) {
        Vector3 velocityChange = -velocity;

        velocityChange = new Vector3(ClampValue(velocityChange.x), ClampValue(velocityChange.y), ClampValue(velocityChange.z));

        return velocityChange;

    }
    private float ClampValue(float value) {
        float normalClamp = 1.0f;
        value = Mathf.Clamp(value, -normalClamp, normalClamp);

        return value;
    }

    private void Sound() {
        if (playJetSound) {
            jetpackSoundWasPlayed = true;
            if (!jetpackAudioSource.isPlaying || jetpackAudioSource.clip != soundJetpackBurn) {
                PlaySoundBurn();
            }
        } else if (jetpackSoundWasPlayed) {
            PlaySoundShutoff();
            jetpackSoundWasPlayed = false;
        }
    }

    private void PlaySoundBurn() {
        jetpackAudioSource.clip = soundJetpackBurn;
        jetpackAudioSource.volume = volumeJetpackBurn;
        jetpackAudioSource.Play();
    }

    private void PlaySoundShutoff() {
        jetpackAudioSource.clip = soundJetpackShutoff;
        jetpackAudioSource.volume = volumeJetpackShutoff;
        jetpackAudioSource.Play();
    }

    public void OnDeactivate() {
        playJetSound = false;
    }
}
