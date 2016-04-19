using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ExplosionForceDamage : MonoBehaviour, IOwnable {

	public float distanceReduction;
	public float explosionPower;
	public float explosionRadius;

    public NetworkIdentity owner { get; set; }

    [SerializeField]
    private int weaponId;
    [SerializeField]
    private AudioClip soundExplode;
	private AudioSource bombAudio;


	void Start(){
        PlaySound();
		Bang ();
	}

	void Bang(){

        //ChatManager.DebugMessage("Explosion. Owner Player Number: " + GetComponent<Owner>().ID.ToString());

		Collider[] hits;
		hits = Physics.OverlapSphere(transform.position, explosionRadius);

		foreach(Collider hit in hits){
			if(hit.CompareTag("ForceProjectile")) continue; // Don't touch the force

            //if (hit.CompareTag("Player") && hit.GetComponent<NetworkView>().isMine) { // Did it hit my player?
                // Better push myself off the ground
            //    hit.GetComponent<ActorMotorManager>().PushOffGround();
            //}

            if (hit.GetComponent<Rigidbody>()) {
                hit.GetComponent<Rigidbody>().AddExplosionForce(explosionPower, transform.position, explosionRadius);
            }

            // Everything else is server only
            //if (Network.isClient) continue; TODO
            
            if(hit.CompareTag("BonusPickup")){
				hit.GetComponent<DestroyOnNextFrame>().DestroyMe();
			}
            if (hit.CompareTag("Grenade")) {
                hit.GetComponent<MineDetonation>().ForceDetonate();
            }

            if (hit.CompareTag("Player")) {
                //Find distance
                float distance = (hit.transform.position - transform.position).magnitude;
                float damage = WeaponManager.weapon[weaponId].damage / (Mathf.Max(distance * distanceReduction, 1));

                IDamageable damageable = hit.gameObject.GetInterface<IDamageable>();
                damageable.TakeDamage((int)damage, owner, weaponId);
            }
		}
	}

    void PlaySound() {
        bombAudio = GetComponent<AudioSource>();
        bombAudio.clip = soundExplode;
        bombAudio.PlayOneShot(soundExplode);
    }
}
