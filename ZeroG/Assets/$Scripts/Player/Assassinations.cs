using UnityEngine;
using System.Collections;
     
public class Assassinations : MonoBehaviour {
	
    [SerializeField]
    private float detectionRadius = 5f; 

	private Transform target = null;
	
	private IDamageable playerResource;
	private Vector3 heading;

	void Update () {
		Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);
		foreach(Collider hit in hits){
            if (hit.CompareTag("Player") && !hit.GetComponent<NetworkView>().isMine) { // Don't like the GetComponent in Update()

                AssassinateTargetInRange(hit);
			}
		}
	}

    private void AssassinateTargetInRange(Collider hit) {
        target = hit.transform;
        heading = transform.position - target.position;
        Vector3 toTarget = (heading).normalized;
        Vector3 toPlayer = (target.position - transform.position).normalized;
        //checks if player is close enough to melee
        if (heading.sqrMagnitude < detectionRadius * detectionRadius) {
            CheckFacing(hit, toPlayer);
        }
    }

    private void CheckFacing(Collider hit, Vector3 toPlayer) {
        target = hit.transform;
        heading = transform.position - target.position;
        Vector3 toTarget = (heading).normalized;
        playerResource = hit.gameObject.GetInterface<IDamageable>();
        //checks if the player is actually facing the player they want to melee
        if (Vector3.Dot(toTarget, transform.forward) < -0.75 && playerResource.GetHealth() > 0) {
            //checks if the player is behind the player they want to melee
			if (Vector3.Dot(toPlayer, target.forward) > 0.75) {
				UIPlayerHUD.Prompt(InputConverter.GetKeyName(KeyBind.Interact) + " - Assassinate");
				Assassinate();
			} else {
				UIPlayerHUD.Prompt(InputConverter.GetKeyName(KeyBind.Interact) + " - Melee");
				MeleeAttack(hit);
			}
        }
    }

    private void Assassinate() {
        if (InputConverter.GetKey(KeyBind.Interact)) {
            playerResource.TakeDamage(100, Network.player, 200); // 200 is assassination ID
        }
    }
	private float meleecooldown = 0f;
    private void MeleeAttack(Collider hit) {
        if (InputConverter.GetKey(KeyBind.Interact) && Time.time > meleecooldown) {
            playerResource.TakeDamage(10, Network.player, 200);
			//hit.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * 2, ForceMode.Impulse);
			meleecooldown = Time.time + 0.5F;;
        }
    }
}