using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {

    public GameObject playerPrefab;

    static GameObject actor;
    static bool myActorSpawned;
    private GameObject[] spawnPoints;
    private CameraMove cameraMove;

    public static PlayerManager instance;

    void Start() {
        instance = this;
    }

    public void Init() {
        cameraMove = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMove>();
        spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
       
        // Spawn a new player object
        actor = Network.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity, 0) as GameObject;
        StartCoroutine(RemoveActor());
    }
    IEnumerator RemoveActor() {
        yield return null;
        ActorDied();
    }

    public void SpawnActor() {
        // Activate Actor
        actor.SetActive(true);

        // Reset all stats
        foreach (Weapon weap in GameManager.weapon) {
            weap.ResetVariables();
        }
        actor.GetComponent<ActorHealth>().Reset();
        actor.GetComponent<WeaponInventory>().Reset();
        actor.GetComponent<ActorMotorManager>().Reset();

        // Freeze movement and rotation
        actor.GetComponent<Rigidbody>().velocity = Vector3.zero;
        actor.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        GameManager.SetCursorVisibility(false);

        GetPlayerCameraMouseLook(actor).SetYDirection(SettingsManager.instance.MouseYDirection);
        actor.GetComponent<MouseLook>().SetYDirection(SettingsManager.instance.MouseYDirection);

        actor.GetComponent<ActorTeam>().SetTeam(NetworkManager.MyPlayer().Team); // Apply team to Actor

        UIPlayerHUD.SetupPlayer(actor);
        DynamicCrosshair.SetInventory(actor.GetComponent<WeaponInventory>());
        PlayerColourManager.instance.AssignColour(actor);

        MovePlayerToSpawnPoint();

        cameraMove.PlayerSpawned();

        Radar.instance.ActorsChanged();
        UIPauseSpawn.PlayerSpawned();

        myActorSpawned = true;
    }

    private void MovePlayerToSpawnPoint() {
        int point = Random.Range(0, spawnPoints.Length);

        actor.transform.position = spawnPoints[point].transform.position;
        actor.transform.rotation = spawnPoints[point].transform.rotation;
    }

    public void ActorDied() {
        myActorSpawned = false;

        UIPauseSpawn.PlayerDied();
        GameManager.instance.SetPlayerMenu(false);
        cameraMove.DetachCamera();
        GameManager.SetCursorVisibility(true);

        // Disable Actor
        actor.SetActive(false);
    }

    public static bool IsActorSpawned() {
        return myActorSpawned;
    }

    private MouseLook GetPlayerCameraMouseLook(GameObject player) {
        return player.transform.FindChild("CameraPos").GetComponent<MouseLook>();
    }
}
