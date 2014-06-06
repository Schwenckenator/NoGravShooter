using UnityEngine;
using System.Collections;

public class GameManagerScript : MonoBehaviour {
	private bool paused;
	private MouseLook cameraLook;

	private bool myPlayerSpawned = false;
	public GameObject playerPrefab;
	private GameObject[] spawnPoints;

	//int bullshit = 1; // This is bullshit


	void Awake(){
		DontDestroyOnLoad(gameObject);
	}

	void OnLevelWasLoaded(int level){

		if(!GameManagerScript.IsSceneMenu()){
			spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
			
			Pause (false);
		}

		if(!GameManagerScript.IsSceneMenu()){
			//int numOfSpawns = spawnPoints.GetLength(0);
			Network.Instantiate(playerPrefab, spawnPoints[int.Parse(Network.player.ToString())].transform.position, spawnPoints[int.Parse(Network.player.ToString())].transform.rotation, 0);
			Spawned(true);

			if(myPlayerSpawned){
				cameraLook = GameObject.FindGameObjectWithTag("CameraPosObj").GetComponent<MouseLook>();
			}
		}
	}

	public static bool IsSceneMenu(){
		return Application.loadedLevelName == "MenuScene";
	}
	public bool IsPaused(){
		return paused;
	}
	public bool IsPlayerSpawned(){
		return myPlayerSpawned;
	}
	private void Spawned(bool playerSpawned){
		myPlayerSpawned = playerSpawned;
		SendMessage("PlayerSpawned", playerSpawned);
	}
	
	// Update is called once per frame
	void Update () {
		if(!GameManagerScript.IsSceneMenu()){

			if(Input.GetKeyDown(KeyCode.Escape)){
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
		CursorVisible(input);

	}
	public void CursorVisible(bool visible){
		Screen.showCursor = visible;
		Screen.lockCursor = !visible;
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
