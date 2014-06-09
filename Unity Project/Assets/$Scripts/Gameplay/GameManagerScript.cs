using UnityEngine;
using System.Collections;

public class GameManagerScript : MonoBehaviour {
	private bool paused;
	private MouseLook cameraLook;
	private CameraMove cameraMove;

	private bool myPlayerSpawned = false;
	public GameObject playerPrefab;
	private GameObject[] spawnPoints;

	//int bullshit = 1; // This is bullshit


	void Awake(){
		DontDestroyOnLoad(gameObject);
	}

	void OnLevelWasLoaded(int level){
		CursorVisible(true);
		if(!GameManagerScript.SceneIsMenu()){
			spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
			cameraMove = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMove>();
			Pause (false);
		}
	}

	public static bool SceneIsMenu(){
		return Application.loadedLevelName == "MenuScene";
	}
	public bool IsPaused(){
		return paused;
	}
	public bool IsPlayerSpawned(){
		return myPlayerSpawned;
	}
	public void Spawn(){
		myPlayerSpawned = true;

		CursorVisible(false);

		Network.Instantiate(playerPrefab, spawnPoints[0].transform.position, spawnPoints[0].transform.rotation, 0);
		cameraMove.Spawn();

		GameObject[] list = GameObject.FindGameObjectsWithTag("CameraPosObj");
		foreach(GameObject mLook in list){
			if(mLook.transform.parent.networkView.isMine){
				cameraLook = mLook.GetComponent<MouseLook>();
			}
		}

	}

	public void PlayerDied(){
		myPlayerSpawned = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(!GameManagerScript.SceneIsMenu()){

			if(Input.GetKeyDown(KeyCode.Escape)){
				CursorVisible(!paused);
				Pause (!paused); // Toggle Pause

			}
			if(Input.GetKeyDown(KeyCode.F1)){

				// Multiply by -1, reversing the direction
				cameraLook.MultYDirection(-1);
			}
		}
	}
	public void Pause(bool input){
		paused = input;
	}
	public void CursorVisible(bool visible){
		Screen.showCursor = visible;
		Screen.lockCursor = !visible;
	}
	public void ManagerDetachCamera(){
		cameraMove.DetachCamera();
	}

	void OnApplicationQuit(){
		if(Network.isClient || Network.isServer){
			Network.Disconnect();
		}
	}

	void OnPlayerDisconnected(NetworkPlayer player){

		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}
}
