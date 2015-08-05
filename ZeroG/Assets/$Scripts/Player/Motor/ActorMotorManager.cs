using UnityEngine;
using System.Collections;

public class ActorMotorManager : MonoBehaviour {

    IActorMotor walkingMotor;
    IActorMotor jetpackMotor;
    IActorMotor currentMotor;

    ActorCameraMotor cameraMotor;

    Rigidbody rigidbody;
    NetworkView networkView;

    float footRayDistance;
    public float maxLandingAngle = 60f;

    bool grounded = false;
    bool active = false;

	// Use this for initialization
	void Start () {
        networkView = GetComponent<NetworkView>();

        if (!networkView.isMine) {
            GetComponent<ActorJetpackMotor>().enabled = false;
            GetComponent<ActorWalkingMotor>().enabled = false;
            GetComponent<ActorCameraMotor>().NotMine();
            this.enabled = false;
            
            return;
        }

        active = true; // If I get here, it's mine

        jetpackMotor = GetComponent<ActorJetpackMotor>();
        walkingMotor = GetComponent<ActorWalkingMotor>();
        cameraMotor = GetComponent<ActorCameraMotor>();
        rigidbody = GetComponent<Rigidbody>();
        
        footRayDistance = (GetComponent<CapsuleCollider>().height * (2f / 3f));

        currentMotor = jetpackMotor;
        cameraMotor.LockMouseLook(true);
        StartCoroutine(MagnetBoots());
        SpawnMove();
	}

    void SpawnMove() {
        rigidbody.AddRelativeForce(Vector3.down * 2, ForceMode.Impulse);
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        grounded = false;
        currentMotor.Movement();
	}
    IEnumerator MagnetBoots() {
        while (true) {
            yield return new WaitForFixedUpdate();

            if (currentMotor != walkingMotor) continue;
            if (grounded) continue;

            if (Physics.Raycast(CollisionRay(), footRayDistance)) {
                rigidbody.AddRelativeForce(Vector3.down, ForceMode.Impulse);
            } else {
                // In the air now
                InAir();
            }
        }
    }

    void Landed() {
        // Deactivate current motor
        currentMotor.OnDeactivate();

        //Replace current motor
        currentMotor = walkingMotor;

        //Adjust camera
        cameraMotor.LockMouseLook(false);
    }
    void InAir() {
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

        grounded = true;

        if (ValidLanding(colInfo)){
            //Snap to surface
            cameraMotor.SnapToSurface(CollisionNormal(colInfo)); // Getting collision normal twice, might cause problems
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
        return colNormal;
    }

    private Ray CollisionRay() {
        // Just fire down
        return new Ray(transform.position, -transform.up);
    }

    bool ValidLanding(Collision colInfo) {
        // If not mine, don't bother
        if (!networkView.isMine) return false;

        Vector3 colNormal = CollisionNormal(colInfo);

        // If the ray didn't hit, the actor isn't walking
        if (colNormal.Equals(Vector3.zero)) return false;

        float angle = Vector3.Angle(transform.up, colNormal);

        return (angle > 0f && angle < maxLandingAngle);
    }

    void OnDeath() {
        // Ragdoll the actor
        InAir();
        rigidbody.constraints = RigidbodyConstraints.None;
        rigidbody.AddTorque(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1), ForceMode.Impulse);
        
        active = false;
        this.enabled = false;
    }

    public void Recoil(float angle) {
        if (grounded) {
            cameraMotor.Recoil(angle);
        } else {
            transform.Rotate(-angle, 0, 0);
        }
    }

    public void PushOffGround() {
        if (active) {
            InAir(); // Activate the jetpack motor
        }
    }
}
