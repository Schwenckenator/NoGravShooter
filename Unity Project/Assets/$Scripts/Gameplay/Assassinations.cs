using UnityEngine;
using System.Collections;
     
public class Assassinations : MonoBehaviour {
	public float detectionRadius = 0.025f; 
	public Transform target = null;
	private GameManager manager;
	private PlayerResources resource;
	private Vector3 heading;

	void Start () {
		manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
		resource = GetComponent<PlayerResources>();
	}
	void Update () {
		Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);
		foreach(Collider hit in hits){
			if(hit.CompareTag("Player") && !hit.networkView.isMine){
				resource = hit.GetComponent<PlayerResources>();
				target = hit.transform;
				heading = transform.position - target.position;
				Vector3 toTarget = (heading).normalized;
				Vector3 toPlayer = (target.position - transform.position).normalized;
				//checks if player is behind another player and is close enough to melee
				//kinda dodgy but it works, dont ask where the 12 came from
				if (Vector3.Dot(toTarget, transform.forward) < -0.75 && heading.sqrMagnitude < detectionRadius*detectionRadius/12) {
					//checks if the player is actually facing the player they want to assassinate
					if (Vector3.Dot(toPlayer, target.forward) > 0.75){
						manager.GetComponent<GUIScript>().ButtonPrompt((int)GameManager.KeyBind.Interact, "Assassinate");
						if(Input.GetKey(GameManager.keyBindings[(int)GameManager.KeyBind.Interact])){
							Debug.Log("stab stab stab.");
							hit.GetComponent<PlayerResources>().TakeDamage(100);
						}
					}
				}
			}
		}
	}
}