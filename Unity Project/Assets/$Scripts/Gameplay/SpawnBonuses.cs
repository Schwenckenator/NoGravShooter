using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnBonuses : MonoBehaviour {

	#region Public Fields
	public float bonusSpawnDelay;
	public float bonusProbability;
	public GameObject[] bonuses;
	public int maxBonuses;
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
			GameObject[] aliveBonuses = GameObject.FindGameObjectsWithTag("BonusPickup");

			if((aliveBonuses.Length < maxBonuses) && (Random.Range(0.0f, 1.0f) < bonusProbability)){ //All decimal
				//Check for free point
				bool picked = false;
				GameObject point = null;
				List<GameObject> remainingPoints = new List<GameObject>();
				for(int i=0; i< bonusSpawnPoints.Length; i++){
					remainingPoints.Add(bonusSpawnPoints[i]);
				}
				while(!picked){
					if(remainingPoints.Count == 0){
						break;
					}
					int possiblePoint = Random.Range(0, remainingPoints.Count);
					if(Physics.CheckSphere(remainingPoints[possiblePoint].transform.position, 0.25f)){ // If hits a collider
						//Point is bad
						remainingPoints.RemoveAt(possiblePoint);
						continue;
					}else{
						point = remainingPoints[possiblePoint];
						picked = true;
						break;
					}
				}
				if(picked){
					Network.Instantiate(bonuses[Random.Range(0, bonuses.Length)], point.transform.position, point.transform.rotation, 0);
				}
			}
			yield return new WaitForSeconds(bonusSpawnDelay);
		}
	}
}
