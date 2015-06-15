using UnityEngine;
using System.Collections;

public class ExplosionForceDamage : MonoBehaviour {

	public float distanceReduction;
	public float explosionPower;
	public float explosionRadius;
	public float maxDamage;

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

            if (hit.GetComponent<Rigidbody>()) {
                hit.GetComponent<Rigidbody>().AddExplosionForce(explosionPower, transform.position, explosionRadius);
            }

            // Everything else is server only
            if (Network.isClient) continue;
            
            if(hit.CompareTag("BonusPickup")){
				hit.GetComponent<DestroyOnNextFrame>().DestroyMe();
			}
            if (hit.CompareTag("Grenade")) {
                hit.GetComponent<MineDetonation>().ForceDetonate();
            }

			if(hit.CompareTag("Player")){
				//Find distance
				float distance = (hit.transform.position - transform.position).magnitude;
				float damage = maxDamage / (Mathf.Max(distance * distanceReduction, 1));

                hit.GetComponent<NoGravCharacterMotor>().PushOffGround();
                IDamageable damageable = hit.GetComponent(typeof(IDamageable)) as IDamageable;
				damageable.TakeDamage((int)damage, GetComponent<Owner>().ID, weaponId);
			}
		}
	}

    void PlaySound() {
        bombAudio = this.GetComponent<AudioSource>();
        bombAudio.clip = soundExplode;
        bombAudio.PlayOneShot(soundExplode);
    }
}
