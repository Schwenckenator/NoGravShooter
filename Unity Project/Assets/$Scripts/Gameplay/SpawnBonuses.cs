using UnityEngine;
using System.Collections;

public class SpawnBonuses : MonoBehaviour {

	#region Public Fields
	public float bonusSpawnDelay;
	public float bonusProbability;
	public GameObject[] bonuses;
	#endregion

	private GameObject[] bonusSpawnPoints;
	// Use this for initialization
	void Start () {
		if(Network.isServer){
			bonusSpawnPoints = GameObject.FindGameObjectsWithTag("BonusSpawnPoint");
			StartCoroutine("SpawnBonus");
		}
	}

	IEnumerator SpawnBonus(){
		while(true){
			if(Random.Range(0.0f, 1.0f) < bonusProbability){ //All decimal

				int point = Random.Range(0, bonusSpawnPoints.Length);
				Network.Instantiate(bonuses[0], bonusSpawnPoints[point].transform.position, bonusSpawnPoints[point].transform.rotation, 0);
			}else{
			}
			yield return new WaitForSeconds(bonusSpawnDelay);
		}
	}
}
