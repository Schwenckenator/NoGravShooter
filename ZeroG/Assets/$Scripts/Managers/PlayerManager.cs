using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerManager : NetworkBehaviour {

    public static PlayerManager singleton { get; private set; }
    public static bool isMyActorSpawned { get; private set; }

    Collider myCollider;
    public Renderer myRenderer;
    Rigidbody myRigidbody;

    CameraMove cameraMove;

    void Awake() {
        myCollider = GetComponent<Collider>();
        myRigidbody = GetComponent<Rigidbody>();

        ChangeActorState(false);
    }

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();
        singleton = this;
        Debug.Log("Player Manager Start Local player.");
        cameraMove = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMove>();
        myRenderer.enabled = false;
    }

    public void SpawnActor() {
        ChangeActorState(true);
        CmdSpawnActor();
    }

    [Command]
    void CmdSpawnActor() {
        ChangeActorState(true);
    }
    [ClientRpc]
    void RpcSpawnActor() {
        ChangeActorState(false);
    }

    [Server]
    public void ActorDied() {
        RpcActorDied();
    }

    [ClientRpc]
    void RpcActorDied() {
        ChangeActorState(false);

        if(isLocalPlayer && SettingsManager.singleton.AutoSpawn) {
            SpawnActor();
        }
    }

    void ChangeActorState(bool alive) {
        myCollider.enabled = alive;
        myRigidbody.isKinematic = !alive;

        if (isLocalPlayer) {
            // My player
            isMyActorSpawned = alive;
            if (alive) {
                // Is alive
                MyActorAlive();
            } else {
                // Is Dead
                MyActorDead();
            }
        } else {
            // Not my player
            myRenderer.enabled = alive;
        }
    }

    private void MyActorAlive() {
        UIPauseSpawn.PlayerSpawned();
        UIPlayerHUD.singleton.SetupPlayer(gameObject);

        foreach (IResetable reset in gameObject.GetInterfacesInChildren<IResetable>()) {
            reset.Reset();
        }

        GameManager.SetCursorVisibility(false);

        foreach (MouseLook look in GetComponentsInChildren<MouseLook>()) {
            look.SetYDirection(SettingsManager.singleton.MouseYDirection);
        }
        //        actor.GetComponent<ActorTeam>().SetTeam(NetworkManager.MyPlayer().Team); // Apply team to Actor
        DynamicCrosshair.SetInventory(GetComponent<WeaponInventory>());

        cameraMove.AttachToPlayer(gameObject);
    }

    private void MyActorDead() {
        cameraMove.DetachCamera();
        GameManager.SetCursorVisibility(true);
        UIPauseSpawn.PlayerDied();

    }

    //    static bool myActorSpawned;
    //    private GameObject[] spawnPoints;
    //    private CameraMove cameraMove;
    //    MouseLook[] lookers;

    //    IResetable[] myResetables;

    //    public static PlayerManager singleton;

    //    ////private new //NetworkView //NetworkView;

    //    void Start() {
    //        singleton = this;

    //    }

    //    //public void Init() {
    //    //    //tStartCoroutine(CreateActor());
    //    //}
    //    //void OnLevelWasLoaded() {
    //    //    if (GameManager.IsSceneMenu()) {
    //    //        //myActorSpawned = false;

    //    //        //UIPauseSpawn.PlayerDied();
    //    //        //GameManager.singleton.SetPlayerMenu(false);
    //    //        //GameManager.SetCursorVisibility(true);

    //    //        //cameraMove = null;
    //    //        //spawnPoints = null;

    //    //        //if (.isClient || .isServer) {
    //    //        //    actorManager.DisableActor(); // Should only disable if connected
    //    //        //} else {
    //    //        //    lookers = null; // If not connected, clear
    //    //        //}
    //    //    } else {
    //    //        cameraMove = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMove>();
    //    //        spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");

    //    //        float wait = lookers == null ? 0.5f : 0.0f;
    //    //        StartCoroutine(LookerLevelStart(wait));
    //    //    }
    //    //}
    //    //IEnumerator LookerLevelStart(float wait) {
    //    //    yield return new WaitForSeconds(wait);
    //    //    foreach (MouseLook look in lookers) {
    //    //        look.LevelStart();
    //    //    }
    //    //}
    //    //IEnumerator CreateActor() {
    //    //    yield return new WaitForSeconds(0.1f);
    //    //    // Spawn a new player object
    //    //    actor = .Instantiate(playerPrefab, new Vector3(100, 100, 100), Quaternion.identity, 0) as GameObject; // Spawn player way out, middle of nowhere
    //    //    lookers = actor.GetComponentsInChildren<MouseLook>();
    //    //    actor.GetComponentInChildren<MeshRenderer>().enabled = false;

    //    //    GameObject temp = .Instantiate(managerPrefab, Vector3.zero, Quaternion.identity, 0) as GameObject;
    //    //    actorManager = temp.GetComponent<ActorEnableManager>();
    //    //    actorManager.SetActor(actor);
    //    //    DynamicCrosshair.myActor = actor.GetComponent<Collider>();

    //    //    DisableLookers();
    //    //    StartCoroutine(RemoveActor());
    //    //}
    //    //void DisableLookers() {
    //    //    foreach(MouseLook look in lookers) {
    //    //        look.Ragdoll(true); // Disable input
    //    //    }
    //    //}
    //    //IEnumerator RemoveActor() {
    //    //    yield return null;
    //    //    actorManager.DisableActor();
    //    //    GameManager.singleton.SetPlayerMenu(false);
    //    //}

    //    public void SpawnActor() {
    //        // Activate Actor
    //        actorManager.EnableActor();

    //        // Reset all stats
    //        foreach (Weapon weap in WeaponManager.weapon) {
    //            weap.ResetVariables();
    //        }
    //        actor.GetComponent<ActorHealth>().Reset(); // SHould interface this shit
    //        actor.GetComponent<WeaponInventory>().Reset();
    //        actor.GetComponent<ActorMotorManager>().Reset();
    //        actor.GetComponent<ActorGrenades>().Reset();

    //        GameManager.SetCursorVisibility(false);

    //        GetPlayerCameraMouseLook(actor).SetYDirection(SettingsManager.singleton.MouseYDirection);
    //        actor.GetComponent<MouseLook>().SetYDirection(SettingsManager.singleton.MouseYDirection);

    //        actor.GetComponent<ActorTeam>().SetTeam(NetworkManager.MyPlayer().Team); // Apply team to Actor

    //        UIPlayerHUD.SetupPlayer(actor);
    //        DynamicCrosshair.SetInventory(actor.GetComponent<WeaponInventory>());
    //        PlayerColourManager.singleton.AssignColour(actor);

    //        MovePlayerToSpawnPoint();

    //        cameraMove.PlayerSpawned();

    //        Radar.instance.ActorsChanged();
    //        UIPauseSpawn.PlayerSpawned();

    //        myActorSpawned = true;
    //    }

    //    private void MovePlayerToSpawnPoint() {
    //        int point = Random.Range(0, spawnPoints.Length);

    //        // Freeze movement and rotation
    //        actor.GetComponent<Rigidbody>().velocity = Vector3.zero;
    //        actor.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

    //        actor.transform.position = spawnPoints[point].transform.position;
    //        actor.transform.rotation = spawnPoints[point].transform.rotation;
    //        //actor.transform.position = new Vector3(5, 0, 0);
    //    }

    //    public void ActorDied() {
    //        myActorSpawned = false;

    //        UIPauseSpawn.PlayerDied();
    //        GameManager.singleton.SetPlayerMenu(false);
    //        cameraMove.DetachCamera();
    //        GameManager.SetCursorVisibility(true);

    //        actorManager.DisableActor();

    //    }

    //    public static bool IsActorSpawned() {
    //        return myActorSpawned;
    //    }

    //    private MouseLook GetPlayerCameraMouseLook(GameObject player) {
    //        return player.transform.FindChild("CameraPos").GetComponent<MouseLook>();
    //    }
}
