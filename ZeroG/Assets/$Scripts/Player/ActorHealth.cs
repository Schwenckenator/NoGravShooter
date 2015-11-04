using UnityEngine;
using System.Collections;


public class ActorHealth : MonoBehaviour, IDamageable {

    public AudioClip SoundTakeDamage;
    public AudioSource helmetAudio;
    public GameObject bloodParticle;
    private DestroyParticleEffect killBlood;

    int maxHealth = 100;
    int health;

    bool isDamageSound = false;
    bool isDying = false;

    //private //NetworkView //NetworkView;
    IActorStats stats;

    private int suicideDamage = 9; // How much damage K does

	// Use this for initialization
	void Awake () {
        stats = gameObject.GetInterface<IActorStats>();
        ////NetworkView = GetComponent<//NetworkView>();
        Reset();
	}
    void Update() {
        //if (Input.GetKeyDown(KeyCode.K) && !GameManager.IsPlayerMenu() && //NetworkView.isMine) { //K is for kill! // This is for testing purposes only
        //    TakeDamage(suicideDamage, Network.player);
        //}
    }
    // Interface Implementation
    public void TakeDamage(int damage, NetworkPlayer from, int weaponId = -1) {
        ////NetworkView.RPC("Damage", RPCMode.All, damage, from, weaponId);
    }
    public void RestoreHealth(int restore) {
        health += restore;
        if (health > maxHealth) {
            health = maxHealth;
        }
        CheckHealthForWaver();
    }
    public int GetHealth() {
        return health;
    }
    public int GetMaxHealth() {
        return maxHealth;
    }
    public bool IsFullHealth() {
        return health == maxHealth;
    }

    ////[RPC]
    void Damage(int damage, NetworkPlayer fromPlayer, int weaponID) {
        if (isDying) return; // Don't bother if you are already dying

        WillPlayTakeDamageSound();
        health -= damage;
        if (health <= 0) {
            Die(NetworkManager.GetPlayer(fromPlayer), weaponID);
        }
    }

    bool CheckHealthForWaver() {
        bool willWaver = true;
        if (health <= 40) {
            BloodyScreen.Waver(0.5f, .25f);
        } else if (health <= 30) {
            BloodyScreen.Waver(0.5f, .5f);
        } else if (health <= 20) {
            BloodyScreen.Waver(0.75f, .75f);
        } else if (health <= 10) {
            BloodyScreen.Waver(1.25f, 1f);
        }else{
            BloodyScreen.StopWaver();
            willWaver = false;
	    }
        return willWaver;
    }

    public void Reset() {
        maxHealth = stats.maxHealth;
        health = maxHealth;
        isDying = false;
    }

    private void Die(Player killer, int weaponID) {
        if (GameManager.IsSceneTutorial()) {
            // Can't die in tutorial.
            health = 10;
            return;
        }
        health = 0;
		isDying = true;//You is dead nigs

        //if (//NetworkView.isMine) {
			if(killer != null){
                string killMessage;
				if(killer.ID != Network.player){
                    GameManager.gameMode.Kill(killer, NetworkManager.MyPlayer());
                    GameManager.gameMode.PlayerDied(NetworkManager.MyPlayer());

                    killMessage = killer.Name;

                    if (killer.IsOnTeam(NetworkManager.MyPlayer().Team)) {
                            
                        killMessage += KillMessageGenerator(teamKillID);
                    } else {
                        killMessage += KillMessageGenerator(weaponID);
                    }
                    killMessage += SettingsManager.singleton.PlayerName;

                        
				} else {
                    killMessage = killer.Name + KillMessageGenerator(weaponID) + "themselves.";
                    GameManager.gameMode.Suicide(killer);
                        
				}
                ChatManager.singleton.AddToChat(killMessage);
			}
            gameObject.SendMessage("OnDeath");
			StartCoroutine(PlayerCleanup());
		//}
    }

    void WillPlayTakeDamageSound() {
        //if (//NetworkView.isMine && !isDamageSound) {
            isDamageSound = true;
            StartCoroutine(PlaySoundTakeDamage());
            if (!CheckHealthForWaver()) {
                BloodyScreen.Flash();
            }
        //}
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
            return " " + GameManager.weapon[weaponId].killMessage + " ";
        }
        return " killed ";

    }

    IEnumerator PlayerCleanup() {
        float playerDyingTime = 3.0f;
        BloodyPlayer();
        BloodyScreen.Show(true);
        yield return new WaitForSeconds(playerDyingTime);
        BloodyScreen.Show(false);
        //PlayerManager.singleton.ActorDied();

        if (SettingsManager.singleton.AutoSpawn) {
            //PlayerManager.singleton.SpawnActor();
        }
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