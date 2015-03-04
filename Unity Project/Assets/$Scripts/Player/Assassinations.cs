using UnityEngine;
using System.Collections;
     
public class Assassinations : MonoBehaviour {
	
    [SerializeField]
    private float detectionRadius = 5f; 

	private Transform target = null;
	
	private PlayerResources playerResource;
	private Vector3 heading;

	void Start () {
		playerResource = GetComponent<PlayerResources>();
	}
	void Update () {
		Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);
		foreach(Collider hit in hits){
			if(hit.CompareTag("Player") && !hit.networkView.isMine){
				playerResource = hit.GetComponent<PlayerResources>();
				target = hit.transform;
				heading = transform.position - target.position;
				Vector3 toTarget = (heading).normalized;
				Vector3 toPlayer = (target.position - transform.position).normalized;
				//checks if player is behind another player and is close enough to melee
				//kinda dodgy but it works, dont ask where the 12 came from
				if (Vector3.Dot(toTarget, transform.forward) < -0.75 && heading.sqrMagnitude < detectionRadius*detectionRadius/12) { // <-- where did that twelve come from? Magic numbers are bad yo
					//checks if the player is actually facing the player they want to assassinate
					if (Vector3.Dot(toPlayer, target.forward) > 0.75 && playerResource.GetHealth() > 0 ){
                        GuiManager.instance.ButtonPrompt("Assassinate", (int)KeyBind.Interact);
                        if (Input.GetKey(SettingsManager.keyBindings[(int)KeyBind.Interact])) {
							Debug.Log("stab stab stab.");
                            ChatManager.instance.AddToChat("Assassinated " + hit.transform.FindChild("NameText").GetComponent<TextMesh>().text + "!", true);
                            playerResource.TakeDamage(100, Network.player);
						}
					}
				}
			}
		}
	}
}