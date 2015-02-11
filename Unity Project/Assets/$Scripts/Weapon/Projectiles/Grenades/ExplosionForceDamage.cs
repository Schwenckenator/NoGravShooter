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
		bombAudio = this.GetComponent<AudioSource>();
		bombAudio.clip = soundExplode;
		bombAudio.PlayOneShot(soundExplode);
		Bang ();
	}

	void Bang(){

        Debug.Log("Explosion. Owner Player Number: " + GetComponent<ProjectileOwnerName>().ProjectileOwner.ToString());

		Collider[] hits;
		hits = Physics.OverlapSphere(transform.position, explosionRadius);

		foreach(Collider hit in hits){
			if(hit.CompareTag("ForceProjectile")) continue; // Don't touch the force

			if(hit.CompareTag("BonusPickup")){
				hit.GetComponent<DestroyOnNextFrame>().DestroyMe();
			}
            if (hit.CompareTag("GrenadeMine")) {
                hit.GetComponent<MineDetonation>().ForceDetonate();
            }

			if(hit.rigidbody){
				hit.rigidbody.AddExplosionForce(explosionPower, transform.position, explosionRadius, 1.0f, ForceMode.Force);
			}

			if(hit.CompareTag("Player")){
				//Find distance
				float distance = (hit.transform.position - transform.position).magnitude;
				float damage = maxDamage / (Mathf.Max(distance * distanceReduction, 1));

				hit.GetComponent<PlayerResources>().TakeDamage((int)damage, GetComponent<ProjectileOwnerName>().ProjectileOwner, weaponId);
			}
		}
	}
}
