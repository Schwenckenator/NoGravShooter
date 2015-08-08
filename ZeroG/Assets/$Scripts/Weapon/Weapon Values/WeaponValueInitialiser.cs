using UnityEngine;
using System.Collections;

public class WeaponValueInitialiser : MonoBehaviour {
	
	public GameObject[] projectile;
	public GameObject[] particle;
	public AudioClip[] fireSound;
	public AudioClip[] reloadSound;


	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(gameObject);

		SetUp();
	}

	void SetUp(){

		for(int i=0; i< GameManager.weapon.Count; i++){
			GameManager.weapon[i].fireSound = fireSound[i];
			GameManager.weapon[i].hitParticle = particle[i];
			GameManager.weapon[i].projectile = projectile[i];
			GameManager.weapon[i].reloadSound = reloadSound[i];
		}
	}
}
