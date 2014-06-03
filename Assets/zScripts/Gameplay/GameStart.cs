using UnityEngine;
using System.Collections;

public class GameStart : MonoBehaviour {
	private bool paused;
	private MouseLook cameraLook;

	// Use this for initialization
	void Start () {
		cameraLook = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MouseLook>();

		paused = false;
		CursorStuff(true);
	}
	
	// Update is called once per frame
	void Update () {
//		if(Input.GetAxis("Mouse Y") != 0){
//			Debug.Log(Input.GetAxis("Mouse Y").ToString());
//		}


		if(Input.GetKeyDown(KeyCode.Escape)){
			paused = !paused; // Toggle pause
			if(paused){
				Time.timeScale = 0;
				CursorStuff(false);
			}else{
				Time.timeScale = 1;
				CursorStuff(true);
			}
		}
		if(Input.GetKeyDown(KeyCode.F1)){

			// Multiply by -1, reversing the direction
			cameraLook.MultYDirection(-1);
		}
	}

	void CursorStuff(bool playing){
		Screen.showCursor = !playing;
		Screen.lockCursor = playing;
	}
}
