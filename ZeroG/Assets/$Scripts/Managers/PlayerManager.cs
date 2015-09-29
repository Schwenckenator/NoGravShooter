using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {

    public GameObject playerPrefab;
    public GameObject managerPrefab;

    static GameObject actor;
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
    void OnLevelWasLoaded() {
        if (GameManager.IsSceneMenu()) {
            myActorSpawned = false;

            UIPauseSpawn.PlayerDied();
            GameManager.instance.SetPlayerMenu(false);
            GameManager.SetCursorVisibility(true);

            cameraMove = null;
            spawnPoints = null;

            if (Network.isClient || Network.isServer) {
                actorManager.DisableActor(); // Should only disable if connected
            } else {
                lookers = null; // If not connected, clear
            }
        } else {
            cameraMove = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMove>();
            spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
            
            float wait = lookers == null ? 0.5f : 0.0f;
            StartCoroutine(LookerLevelStart(wait));
        }
    }
    IEnumerator LookerLevelStart(float wait) {
        yield return new WaitForSeconds(wait);
        foreach (MouseLook look in lookers) {
            look.LevelStart();
        }
    }
    IEnumerator CreateActor() {
        yield return new WaitForSeconds(0.1f);
        // Spawn a new player object
        actor = Network.Instantiate(playerPrefab, new Vector3(100, 100, 100), Quaternion.identity, 0) as GameObject; // Spawn player way out, middle of nowhere
        lookers = actor.GetComponentsInChildren<MouseLook>();
        actor.GetComponentInChildren<MeshRenderer>().enabled = false;

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
        actor.GetComponentInChildren<MeshRenderer>().enabled = true;
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
