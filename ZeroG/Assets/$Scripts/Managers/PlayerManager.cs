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

    public static PlayerManager instance;

    private new NetworkView networkView;

    void Start() {
        instance = this;
        networkView = GetComponent<NetworkView>();
    }

    public void Init() {
        cameraMove = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMove>();
        spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");

        StartCoroutine(CreateActor());
    }
    IEnumerator CreateActor() {
        yield return new WaitForSeconds(0.1f);
        // Spawn a new player object
        actor = Network.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity, 0) as GameObject;
        GameObject temp = Network.Instantiate(managerPrefab, Vector3.zero, Quaternion.identity, 0) as GameObject;
        actorManager = temp.GetComponent<ActorEnableManager>();
        actorManager.SetActor(actor);

        DisableLookers();

        StartCoroutine(RemoveActor());
    }
    void DisableLookers() {
        MouseLook[] lookers = actor.GetComponentsInChildren<MouseLook>();
        foreach(MouseLook look in lookers) {
            look.Ragdoll(true); // Disable input
        }
    }
    IEnumerator RemoveActor() {
        yield return null;
        ActorDied();
    }

    public void SpawnActor() {
        // Activate Actor
        actorManager.EnableActor();

        // Reset all stats
        foreach (Weapon weap in GameManager.weapon) {
            weap.ResetVariables();
        }
        actor.GetComponent<ActorHealth>().Reset();
        actor.GetComponent<WeaponInventory>().Reset();
        actor.GetComponent<ActorMotorManager>().Reset();

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
