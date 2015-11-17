using UnityEngine;
using System.Collections;
     
public class Assassinations : MonoBehaviour {
	
    //[SerializeField]
    //private float detectionRadius = 5f; 

	//private Transform target = null;
	
	//private IDamageable playerHealth;
	//private Vector3 heading;

    //NetworkView myActorView;
    //private Collider myActorCollider;

 //   void Start() {
 //       //myActorView = GetComponent<//NetworkView>();
 //       myActorCollider = GetComponent<Collider>();
 //       //if (!myActorView.isMine) {
 //       //    this.enabled = false;
 //       //}
 //   }

	//void Update () {
	//	Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);
	//	foreach(Collider hit in hits){
 //           if (hit.CompareTag("Player") && hit != myActorCollider) {
 //               AssassinateTargetInRange(hit);
	//		}
	//	}
	//}

 //   private void AssassinateTargetInRange(Collider hit) {
 //       target = hit.transform;
 //       heading = transform.position - target.position;
 //       Vector3 toTarget = (heading).normalized;
 //       Vector3 toPlayer = (target.position - transform.position).normalized;
 //       //checks if player is close enough to melee
 //       if (heading.sqrMagnitude < detectionRadius * detectionRadius) {
 //           CheckFacing(hit, toPlayer);
 //       }
 //   }

 //   private void CheckFacing(Collider hit, Vector3 toPlayer) {
 //       target = hit.transform;
 //       heading = transform.position - target.position;
 //       Vector3 toTarget = (heading).normalized;
 //       playerHealth = hit.gameObject.GetInterface<IDamageable>();
 //       //checks if the player is actually facing the player they want to melee
 //       if (Vector3.Dot(toTarget, transform.forward) < -0.75 && playerHealth.Health > 0) {
 //           //checks if the player is behind the player they want to melee
	//		if (Vector3.Dot(toPlayer, target.forward) > 0.75) {
	//			UIPlayerHUD.Prompt(InputKey.GetKeyName(KeyBind.Interact) + " - Assassinate");
	//			Assassinate();
	//		} else {
	//			UIPlayerHUD.Prompt(InputKey.GetKeyName(KeyBind.Interact) + " - Melee");
	//			MeleeAttack(hit);
	//		}
 //       }
 //   }

 //   private void Assassinate() {
 //       if (InputKey.GetKey(KeyBind.Interact)) {
 //           playerHealth.TakeDamage(100, 200); // 200 is assassination ID
 //       }
 //   }
	//private int pushstrength = 2;
	//private float meleecooldown = 0f;
 //   private void MeleeAttack(Collider hit) {
 //       if (InputKey.GetKey(KeyBind.Interact) && Time.time > meleecooldown) {
 //           playerHealth.TakeDamage(10);
	//		//hit.GetComponent<ActorMotorManager>().PushOffGround();
	//		//hit.GetComponent<Rigidbody>().AddForce(transform.forward * pushstrength, ForceMode.Impulse);
	//		meleecooldown = Time.time + 0.5F;
 //       }
 //   }
}