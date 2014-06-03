using UnityEngine;
using System.Collections;

public class GUIScript : MonoBehaviour {
	public Texture empty;
	public Texture full;

	private PlayerResources res;


	// Use this for initialization
	void Start () {
		if(Application.loadedLevelName != "MenuScene"){
			res = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerResources>();
		}
	}

	void OnGUI(){
		if(Application.loadedLevelName != "MenuScene"){
			GUI.DrawTexture(new Rect(10, 10, 300, 40), empty);
			GUI.DrawTexture(new Rect(10, 10, res.GetFuel()*3, 40), full);
		}
	}
}
