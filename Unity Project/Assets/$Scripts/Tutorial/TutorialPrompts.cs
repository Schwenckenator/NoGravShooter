using UnityEngine;
using System.Collections;
     
public class TutorialPrompts : MonoBehaviour {
	private GameManager manager;
	private bool playerpresent = false;
	
	public GameManager.KeyBind Button;
	public string Message;
	
	private int ButtonId;

	void Start(){
		manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
		ButtonId = (int)Button;
	}
	
	void Update(){
		if(playerpresent){
			manager.GetComponent<GUIScript>().ButtonPrompt(ButtonId, Message);
		}
	}
	
	void OnTriggerEnter(Collider other){
        if(other.gameObject.tag=="Player"){
			playerpresent = true;
		}
    }
	
	void OnTriggerExit(Collider other){
        if(other.gameObject.tag=="Player"){
			playerpresent = false;
		}
    }
}