using UnityEngine;
using System.Collections;

public class ActorHealth : MonoBehaviour, IDamageable {

    public AudioClip SoundTakeDamage;
    public AudioSource helmetAudio;

    int maxHealth = 100;
    int health;

    bool isDamageSound = false;
    bool isDying = false;

    private NetworkView networkView;

	// Use this for initialization
	void Start () {
        IActorStats stats = gameObject.GetInterface<IActorStats>();
        networkView = GetComponent<NetworkView>();
        maxHealth = stats.maxHealth;
        health = maxHealth;
	}
    void Update() {
        if (Input.GetKeyDown(KeyCode.K) && !GameManager.IsPlayerMenu() && networkView.isMine) { //K is for kill! // This is for testing purposes only
            TakeDamage(100, Network.player);
        }
    }
    // Interface Implementation
    public void TakeDamage(int damage, NetworkPlayer from, int weaponId = -1) {
        networkView.RPC("Damage", RPCMode.All, damage, from, weaponId);
    }
    public void RestoreHealth(int restore) {
        health += restore;
        if (health > maxHealth) {
            health = maxHealth;
        }
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

    [RPC]
    void Damage(int damage, NetworkPlayer fromPlayer, int weaponID) {
        if (isDying) return; // Don't bother if you are already dying

        WillPlayTakeDamageSound();
        health -= damage;
        if (health <= 0) {
            Die(NetworkManager.GetPlayer(fromPlayer), weaponID);
        }
    }

    private void Die(Player killer, int weaponID) {
        if (GameManager.IsSceneTutorial()) {
            // Can't die in tutorial.
            health = 10;
            return;
        }
        health = 0;
		isDying = true;//You is dead nigs

        if (networkView.isMine) {
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
                    killMessage += SettingsManager.instance.PlayerName;

                        
				} else {
                    killMessage = killer.Name + KillMessageGenerator(weaponID) + "themselves.";
                    GameManager.gameMode.Suicide(killer);
                        
				}
                ChatManager.instance.AddToChat(killMessage);
			}
            gameObject.SendMessage("OnDeath");
			StartCoroutine(PlayerCleanup());
		}
    }

    void WillPlayTakeDamageSound() {
        if (networkView.isMine && !isDamageSound) {
            isDamageSound = true;
            StartCoroutine(PlaySoundTakeDamage());
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
        switch (weaponId) {
            case 0:
                return " lasered ";
            case 1:
                return " shot ";
            case 2:
                return " sniped ";
            case 3:
                return " shotgunned ";
            case 4:
                return " forced ";
            case 5:
                return " exploded ";
            case 6:
                return " plasmered ";
            case teamKillID:
                return " betrayed ";
            case assassinated:
                return " assassinated ";
        }

        return " killed ";

    }

    IEnumerator PlayerCleanup() {
        float playerDyingTime = 3.0f;
        BloodyScreen.Show(true);
        yield return new WaitForSeconds(playerDyingTime);
        BloodyScreen.Show(false);
        GameManager.instance.PlayerDied();
        GameManager.instance.ManagerDetachCamera();
        GameManager.SetCursorVisibility(true);
       
        //
        if (Network.isServer) {
            GetComponent<ObjectCleanUp>().KillMe();
        } else {
            GetComponent<ObjectCleanUp>().ClientKillMe();
        }
    }

    public static int GetDefaultMaxHealth() {
        return 100; // Default max health
    }
}