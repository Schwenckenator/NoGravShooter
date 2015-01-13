using UnityEngine;
using System.Collections;
     
public class Assassinations : MonoBehaviour {
	
    [SerializeField]
    private float detectionRadius = 0.025f; 
    [SerializeField]
	private Transform target = null;
	
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
				if (Vector3.Dot(toTarget, transform.forward) < -0.75 && heading.sqrMagnitude < detectionRadius*detectionRadius/12) { // <-- where did that twelve come from? Magic numbers are bad yo
					//checks if the player is actually facing the player they want to assassinate
					if (Vector3.Dot(toPlayer, target.forward) > 0.75 && resource.GetHealth() > 0 ){
						manager.GetComponent<GUIScript>().ButtonPrompt("Assassinate", (int)GameManager.KeyBind.Interact);
						if(Input.GetKey(GameManager.keyBindings[(int)GameManager.KeyBind.Interact])){
							Debug.Log("stab stab stab.");
							manager.AddToChat("Assassinated " + hit.transform.FindChild("NameText").GetComponent<TextMesh>().text + "!");
                            resource.TakeDamage(100);
						}
					}
				}
			}
		}
	}
}