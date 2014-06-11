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
			Debug.Log ("Roll for bonus!");
			if(Random.Range(0.0f, 1.0f) < bonusProbability){ //All decimal
				Debug.Log("Yeah! Bonus!");

				int point = Random.Range(0, bonusSpawnPoints.Length);
				Network.Instantiate(bonuses[0], bonusSpawnPoints[point].transform.position, bonusSpawnPoints[point].transform.rotation, 0);
			}else{
				Debug.Log ("No bonus :(");
			}
			yield return new WaitForSeconds(bonusSpawnDelay);
		}
	}
}
