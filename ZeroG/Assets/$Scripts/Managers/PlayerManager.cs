using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {

    public GameObject playerPrefab;
    public GameObject managerPrefab;

    static GameObject actor;
    //static NetworkView actorView;
    static ActorEnableManager actorManager;

    static bool myActorSpawned;
    private GameObject[] spawnPoints;
    private CameraMove cameraMove;
    MouseLook[] lookers;

    public static PlayerManager instance;

    private new NetworkView networkView;

    void Start() {
        instance = this;
        networkView = GetComponent<NetworkView>();
    }

    public void Init() {
        StartCoroutine(CreateActor());
    }
    public void LevelStart() {
        cameraMove = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMove>();
        spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
        foreach (MouseLook look in lookers) {
            look.LevelStart();
        }
    }
    IEnumerator CreateActor() {
        yield return new WaitForSeconds(0.1f);
        // Spawn a new player object
        actor = Network.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity, 0) as GameObject;
        lookers = actor.GetComponentsInChildren<MouseLook>();

        GameObject temp = Network.Instantiate(managerPrefab, Vector3.zero, Quaternion.identity, 0) as GameObject;
        actorManager = temp.GetComponent<ActorEnableManager>();
        actorManager.SetActor(actor);
        
        DisableLookers();
        StartCoroutine(RemoveActor());
    }
    void DisableLookers() {
        foreach(MouseLook look in lookers) {
            look.Ragdoll(true); // Disable input
        }
    }
    IEnumerator RemoveActor() {
        yield return null;
        actorManager.DisableActor();
        GameManager.instance.SetPlayerMenu(false);
    }

    public void SpawnActor() {
        // Activate Actor
        actorManager.EnableActor();

        // Reset all stats
        foreach (Weapon weap in GameManager.weapon) {
            weap.ResetVariables();
        }
        actor.GetComponent<ActorHealth>().Reset(); // SHould interface this shit
        actor.GetComponent<WeaponInventory>().Reset();
        actor.GetComponent<ActorMotorManager>().Reset();
        actor.GetComponent<ActorGrenades>().Reset();

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

        // Freeze movement and rotation
        actor.GetComponent<Rigidbody>().velocity = Vector3.zero;
        actor.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    public void ActorDied() {
        myActorSpawned = false;

        UIPauseSpawn.PlayerDied();
        GameManager.instance.SetPlayerMenu(false);
        cameraMove.DetachCamera();
        GameManager.SetCursorVisibility(true);

        actorManager.DisableActor();

    }

    public static bool IsActorSpawned() {
        return myActorSpawned;
    }

    private MouseLook GetPlayerCameraMouseLook(GameObject player) {
        return player.transform.FindChild("CameraPos").GetComponent<MouseLook>();
    }
}
