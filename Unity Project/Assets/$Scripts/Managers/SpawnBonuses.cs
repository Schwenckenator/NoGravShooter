using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnBonuses : MonoBehaviour {

	#region Public Fields
    [SerializeField]
	private float bonusSpawnDelay;
    [SerializeField]
	private float bonusProbability;
    [SerializeField]
	private GameObject[] bonuses;
    [SerializeField]
	private int maxBonuses;
	#endregion

	private GameObject[] bonusSpawnPoints;
    private List<GameObject> spawnableBonuses;

	// Use this for initialization
	void Start () {
		if(Network.isServer){
            //Determine what bonuses are being spawned this game
            spawnableBonuses = new List<GameObject>();

            bool[] allowedBonuses = SettingsManager.instance.GetAllowedBonuses();

            for (int i = 0; i < allowedBonuses.Length; i++) {
                if (allowedBonuses[i]) {
                    spawnableBonuses.Add(bonuses[i]);
                }
            }

            bonusSpawnPoints = GameObject.FindGameObjectsWithTag("BonusSpawnPoint");
            if (spawnableBonuses.Count > 0) {
                StartCoroutine("SpawnBonus");
            }
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
					Network.Instantiate(spawnableBonuses[Random.Range(0, spawnableBonuses.Count)], point.transform.position, point.transform.rotation, 0);
                    GuiManager.instance.ActorsChanged();
                    Radar.instance.ActorsChanged();
				}
			}
			yield return new WaitForSeconds(bonusSpawnDelay);
		}
	}
}
