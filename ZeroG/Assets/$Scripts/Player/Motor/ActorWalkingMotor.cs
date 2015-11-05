using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class ActorWalkingMotor : MonoBehaviour, IActorMotor, IResetable {

    public AudioSource feetAudio;
    public AudioClip[] soundFootsteps;
    public float volumeFootsteps;
    public float maxVelocityChange = 10.0f;
    public float walkingSoundVelocity;
    public float jumpForce;

    Vector3 jumpVector;
    float sqrWalkingSoundVelocity;
    bool playWalkingSound;
    IActorStats stats;
    new Rigidbody rigidbody;
    IControllerInput input;

    //NetworkView networkView;

    // Use this for initialization
    void Awake() {
        //networkView = GetComponent<NetworkView>();
    }
    void Start() {
        rigidbody = GetComponent<Rigidbody>();
        stats = gameObject.GetInterface<IActorStats>();
        input = gameObject.GetInterface<IControllerInput>();
        sqrWalkingSoundVelocity = walkingSoundVelocity * walkingSoundVelocity;
        jumpVector = new Vector3(0, jumpForce, 0);

        Reset();
    }

    void Update() {
        Sound();
    }

    public void Reset() {
        StopAllCoroutines();
    }

    public void Movement() {
        Vector3 targetVelocity;
        if (UIPauseSpawn.IsShown) {
            targetVelocity = Vector3.zero;
        } else {
            targetVelocity = new Vector3(input.GetXMovement(), 0, input.GetZMovement());
        }

        targetVelocity = Vector3.ClampMagnitude(targetVelocity, 1.0f);
        targetVelocity = transform.TransformDirection(targetVelocity);
        targetVelocity = EdgeDetection(targetVelocity);
        targetVelocity *= stats.speed;

        //Apply a force that attempts to reach our target velocity
        Vector3 velocity = rigidbody.velocity;
        Vector3 velocityChange = (targetVelocity - velocity);

        if (velocity.sqrMagnitude > sqrWalkingSoundVelocity) {
            playWalkingSound = true;
        } else {
            playWalkingSound = false;
        }

        Vector3.ClampMagnitude(velocityChange, maxVelocityChange);
        rigidbody.AddForce(velocityChange, ForceMode.Impulse);

        // Jump if wanted
        Jump();
    }

    void Jump() {
        if (input.GetYMovement() > 0) {
            rigidbody.AddRelativeForce(jumpVector, ForceMode.Impulse);
        }
    }

    /// <summary>
    /// Detects edges and modifies the velocity to avoid falling off.
    /// </summary>
    private Vector3 EdgeDetection(Vector3 targetVelocity) {
        // If not sneaking, don't bother
        if (!InputKey.GetKey(KeyBind.JetDown)) {
            return targetVelocity;
        }

        Vector3 totalPushBackDir = FindEdges();

        // If no edges near, don't bother
        if (totalPushBackDir.sqrMagnitude == 0) {
            return targetVelocity;
        }
        totalPushBackDir.Normalize();
        Debug.DrawRay(transform.position, totalPushBackDir, Color.magenta, 10, false);
        Debug.Log(totalPushBackDir.ToString("G4"));

        totalPushBackDir = new Vector3(1 - Mathf.Abs(totalPushBackDir.x), 1 - Mathf.Abs(totalPushBackDir.y), 1 - Mathf.Abs(totalPushBackDir.z));

        // Return scaled vector
        return (Vector3.Scale(targetVelocity, totalPushBackDir));
    }

    private Vector3 FindEdges() {
        Vector3 totalPushBackDir = Vector3.zero;
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

    int stepKind = 0;
    float nextStepTime = 0;
    private void Sound() {
        if (nextStepTime > Time.time) return;

        if (playWalkingSound) {
            PlayFootstep(stepKind);
            nextStepTime = Time.time + soundFootsteps[stepKind].length;
            stepKind = (++stepKind) % soundFootsteps.Length;
        } else {
            stepKind = 0;
            feetAudio.Stop();
        }
    }

    void PlayFootstep(int stepNum) {
        feetAudio.clip = soundFootsteps[stepNum];
        feetAudio.Play();
    }


    public void OnDeactivate() {
        playWalkingSound = false;
    }
}
