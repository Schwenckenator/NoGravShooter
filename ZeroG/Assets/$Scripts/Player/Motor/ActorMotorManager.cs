using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ActorMotorManager : NetworkBehaviour, IResetable {

    public bool debug = false;
    private Logger log;

    private delegate void OnUpdate();
    private OnUpdate update;
    private OnUpdate ready;

    IActorMotor walkingMotor;
    IActorMotor jetpackMotor;
    IActorMotor currentMotor;

    ActorCameraMotor cameraMotor;

    new Rigidbody rigidbody;

    float footRayDistance;
    public float maxLandingAngle = 60f;
    float maxWalkingAngle = 105f;
    private float currentLandingAngle;

    //bool grounded = false;
    bool active = false;

    void Awake() {
        update = NotReady;
        log = new Logger(debug);
    }

    public override void OnStartLocalPlayer() {
        base.OnStartAuthority();
        jetpackMotor = GetComponent<ActorJetpackMotor>();
        walkingMotor = GetComponent<ActorWalkingMotor>();
        cameraMotor = GetComponent<ActorCameraMotor>();
        rigidbody = GetComponent<Rigidbody>();

        footRayDistance = (GetComponent<CapsuleCollider>().height * (2f / 3f));

        log.Log("Foot Ray is " + footRayDistance.ToString());

        Reset();
        update = Ready;
    }

    public void Reset() {

        enabled = true;
        active = true;

        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        currentMotor = jetpackMotor;
        InAir();

        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

        rigidbody.AddRelativeForce(0, -4, 0, ForceMode.Impulse);
    }

    IEnumerator SpawnMove() {
        yield return null;
        rigidbody.AddRelativeForce(Vector3.down * 2, ForceMode.Impulse);
    }

    // Update is called once per frame
    void FixedUpdate() {
        update();
    }
    void NotReady() {

    }
    void Ready() {
        UpdateLandingAngle();
        currentMotor.Movement();
        MagnetBoots();
    }

    void UpdateLandingAngle() {
        if (currentMotor == walkingMotor && InputKey.GetKey(KeyBind.MoveForward) && !InputKey.GetKey(KeyBind.JetDown)) {
            currentLandingAngle = maxWalkingAngle;
        } else {
            currentLandingAngle = maxLandingAngle;
        }
    }
    void MagnetBoots() {

        if (currentMotor != walkingMotor) return;

        if (Physics.Raycast(CollisionRay(), footRayDistance)) {
            rigidbody.AddRelativeForce(Vector3.down, ForceMode.Impulse);
        } else {
            // In the air now
            InAir();
        }
    }

    void Landed() {
        log.Log("Landed!");

        // Deactivate current motor
        currentMotor.OnDeactivate();

        //Replace current motor
        currentMotor = walkingMotor;

        //Adjust camera
        cameraMotor.LockMouseLook(false);
    }
    void InAir() {
        log.Log("In Air!");

        // Deactivate current motor
        currentMotor.OnDeactivate();

        //Replace current motor
        currentMotor = jetpackMotor;

        //Adjust camera
        cameraMotor.LockMouseLook(true);
    }

    void OnCollisionEnter(Collision colInfo) {
        if (!active) return; // Checking every time seems like a waste, but it'll do for now.

        if (ValidLanding(colInfo)) {
            Landed();
        }
    }

    void OnCollisionStay(Collision colInfo) {
        if (!active) return; // Which means this'll probably be here forever.

        //grounded = true;

        if (ValidLanding(colInfo)) {
            //Snap to surface
            cameraMotor.SnapToSurface(LargestValidAngleNormal(colInfo)); // Getting collision normal twice, might cause problems
        }

        RaycastHit hit;
        if (Physics.Raycast(CollisionRay(), out hit, footRayDistance)) {
            //On ground
        } else {
            // Hit weird angle, push away
            rigidbody.AddForce(colInfo.contacts[0].normal, ForceMode.Impulse);
        }

    }
    // Returns Zero vector with no hit
    private Vector3 CollisionNormal(Collision colInfo) {
        RaycastHit[] hits;
        Vector3 colNormal = Vector3.zero;

        hits = Physics.RaycastAll(CollisionRay(), footRayDistance);
        foreach (RaycastHit hit in hits) {
            if (hit.collider == colInfo.collider) {
                colNormal = hit.normal;
                break;
            }
        }
        colNormal = colInfo.contacts[0].normal;
        return colNormal;
    }
    private Vector3 LargestValidAngleNormal(Collision colInfo) {
        float[] angles = new float[colInfo.contacts.Length];

        for (int i = 0; i < angles.Length; i++) {

            if (colInfo.contacts[i].otherCollider.CompareTag("Player")) { // If player contact, don't count it
                angles[i] = -10f; // Lower value than what is possible
                continue;
            }
            angles[i] = Vector3.Angle(transform.up, colInfo.contacts[i].normal);
        }

        int index = -1;
        float max = -1f; // Lowest possible value
        // Find maximum valid angle
        for (int i = 0; i < angles.Length; i++) {
            if (angles[i] > max && angles[i] < currentLandingAngle) {
                index = i;
                max = angles[i];
            }
        }
        return (index < 0) ? Vector3.zero : colInfo.contacts[index].normal;
    }

    private Ray CollisionRay() {
        // Just fire down
        return new Ray(transform.position, -transform.up);
    }

    bool ValidLanding(Collision colInfo) {
        // If not mine, don't bother
        if (!hasAuthority) return false;


        Vector3 colNormal = LargestValidAngleNormal(colInfo);

        // If the ray didn't hit, the actor isn't walking
        if (colNormal.Equals(Vector3.zero)) return false;

        float angle = Vector3.Angle(transform.up, colNormal);

        if (angle > 0) {
            log.Log(angle);
        }

        return (angle > 0f && angle < currentLandingAngle);
    }

    void OnDeath() {
        // Ragdoll the actor
        InAir();
        //StopAllCoroutines();
        rigidbody.constraints = RigidbodyConstraints.None;
        rigidbody.AddTorque(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f), ForceMode.Impulse);

        active = false;
        this.enabled = false;
    }

    public void Recoil(float angle) {
        //if (grounded) {
        //    cameraMotor.Recoil(angle);
        //} else {
        //    transform.Rotate(-angle, 0, 0);
        //}
    }

    public void PushOffGround() {
        if (active) {
            InAir(); // Activate the jetpack motor
            //rigidbody.AddRelativeForce(Vector3.up, ForceMode.Impulse); // Add some up?
        }
    }
}
