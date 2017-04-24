using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CometSpawner : MonoBehaviour {

	public bool debugMode;
	[Range(0,1)]
	public float debug_difficultyPercent;

	public int cometPoolSize = 500;

	public Vector2 minNumPerPlaneMinMax;
	public Vector2 maxNumPerPlaneMinMax;
	public float dstBetweenPlanes = 10; 
	public float farSpawnPlane = 500;
	const int spawnOnPlayerEveryXPlanes = 1;


	public PoolObject cometPrefab;

	Transform camT;
	Transform player;
	float playerZAtLastSpawn;
	int totSpawnCount;

	void Start() {

		if (Application.isEditor && debugMode) {
			Difficulty.difficultyPercent = debug_difficultyPercent;
		}

		camT = Camera.main.transform;
		player = FindObjectOfType<Player> ().transform;
		PoolManager.instance.CreatePool (cometPrefab, cometPoolSize);

		StartCoroutine (Spawn ());
	}


	IEnumerator Spawn () {

		int countSinceSpawnOnPlayer = 0;

		while (true) {
			playerZAtLastSpawn = player.position.z;

			int spawnCount = (int)Random.Range (Mathf.Lerp (minNumPerPlaneMinMax.x, minNumPerPlaneMinMax.y, Difficulty.difficultyPercent), Mathf.Lerp (maxNumPerPlaneMinMax.x, maxNumPerPlaneMinMax.y, Difficulty.difficultyPercent));

			for (int i = 0; i < spawnCount; i++) {

				Vector2 inCircle = Random.insideUnitCircle * GameConstants.gameRadius;
				Vector3 spawnPos = new Vector3 (inCircle.x, inCircle.y, farSpawnPlane + player.position.z);
			
				if (i == 0 && countSinceSpawnOnPlayer >= spawnOnPlayerEveryXPlanes && totSpawnCount > 0) {
					countSinceSpawnOnPlayer = 0;
					spawnPos = new Vector3 (player.position.x, player.position.y, spawnPos.z);
				}

				PoolManager.instance.TryInstantiate (spawnPos, Quaternion.identity,camT);
				
			}
			totSpawnCount++;
			countSinceSpawnOnPlayer++;

			yield return new WaitUntil (() => player.position.z - playerZAtLastSpawn > dstBetweenPlanes);
		}

	}
		
}
