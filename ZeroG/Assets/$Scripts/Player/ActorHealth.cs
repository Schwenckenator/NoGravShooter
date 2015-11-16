using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

public class ActorHealth : NetworkBehaviour, IDamageable, IResetable {

    public AudioClip SoundTakeDamage;
    public AudioSource helmetAudio;
    public GameObject bloodParticle;
    private DestroyParticleEffect killBlood;
    private ActorManager myManager;
    int maxHealth = 100;
    

    bool isDamageSound = false;

    IActorStats stats;

    private int suicideDamage = 100; // How much damage K does

    [SerializeField]
    [SyncVar]
    int health; //Public for monitoring only

    [SyncVar]
    bool isDying = false;

    public int Health {
        get {
            return health;
        }
    }
    public int MaxHealth {
        get {
            return maxHealth;
        }
    }
    public bool IsFullHealth {
        get {
            return health == maxHealth;
        }
    }
    void Awake() {
        myManager = GetComponent<ActorManager>();
    }
    // Use this for initialization
    public override void OnStartLocalPlayer() {
        stats = gameObject.GetInterface<IActorStats>();
        Reset();
    }
    public void Reset() {
        maxHealth = stats.maxHealth;
        CmdReset(maxHealth);
    }
    [Command]
    public void CmdReset(int maxHealth) {
        isDying = false;
        health = maxHealth;
    }
    void Update() {
        if (!isLocalPlayer) return;
        if (Input.GetKeyDown(KeyCode.K) && !UIPauseSpawn.IsShown) { //K is for kill! // This is for testing purposes only
            CmdDamageSelf(suicideDamage);
        }
    }

    [Command]
    void CmdDamageSelf(int damage) {
        TakeDamage(damage);
    }
   
    [Server]
    public void TakeDamage(int damage, int weaponID = -1) {
        if (isDying) return; // Don't bother if you are already dying

        WillPlayTakeDamageSound();
        health -= damage;
        if (health <= 0) {
            Die(weaponID);
        }
    }
    [Server]
    public void RestoreHealth(int restore) {
        health += restore;
        if (health > maxHealth) {
            health = maxHealth;
        }
    }

    private void Die(int weaponID) {
        if (GameManager.IsSceneTutorial()) {
            // Can't die in tutorial.
            health = 10;
            return;
        }
        health = 0;
		isDying = true;//You is dead nigs
        RpcDie();
		//StartCoroutine(PlayerCleanup());
    }

    [ClientRpc]
    void RpcDie() {
        StartCoroutine(PlayerCleanup());
        if (isLocalPlayer) {
            StartCoroutine(LocalPlayerCleanup());
        }
    }

    void WillPlayTakeDamageSound() {
        if(isLocalPlayer && !isDamageSound) {
            isDamageSound = true;
            StartCoroutine(PlaySoundTakeDamage());
            BloodyScreen.Flash();
        }
    }

    IEnumerator PlaySoundTakeDamage() {
        yield return new WaitForEndOfFrame();
        helmetAudio.PlayOneShot(SoundTakeDamage);
        isDamageSound = false;
    }

    const int teamKillID = 100;
    const int assassinated = 200;
    string KillMessageGenerator(int weaponId) {
        Debug.Log(weaponId + "killed me.");
        if (weaponId < 0 || weaponId > 99) { // Is it special case?
            switch (weaponId) {
                case teamKillID:
                    return " betrayed ";
                case assassinated:
                    return " assassinated ";
            }
        } else {
            return " " + WeaponManager.weapon[weaponId].killMessage + " ";
        }
        return " killed ";

    }
    float playerDyingTime = 3.0f;
    IEnumerator PlayerCleanup() {
        BloodyPlayer();
        
        yield return new WaitForSeconds(playerDyingTime);

        if (NetworkManager.isServer) {
            myManager.ActorDied();
        }

    }

    IEnumerator LocalPlayerCleanup() {
        BloodyScreen.Show(true);
        yield return new WaitForSeconds(playerDyingTime);
        BloodyScreen.Show(false);
    }

    //[RPC]
    void BloodyPlayer(bool sendRPC = true) {
        //Rotate blood to angle TODO
        GameObject newBlood = Instantiate(bloodParticle) as GameObject;
        newBlood.transform.SetParent(this.transform);
        newBlood.transform.localPosition = Vector3.zero;
        ParticleSystem newBloodParticle = newBlood.GetComponent<ParticleSystem>();

        newBloodParticle.Play();
        killBlood = newBloodParticle.GetComponent<DestroyParticleEffect>();
        Invoke("DetachBlood", 2.9f); // Just shy of player deletion time
        //if (sendRPC) {
        //    GetComponent<//NetworkView>().RPC("BloodyPlayer", RPCMode.Others, false);
        //}
    }
    void DetachBlood() {
        killBlood.DestroyAfterDelay();
    }

    public static int GetDefaultMaxHealth() {
        return 100; // Default max health
    }
}