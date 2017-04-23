using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalSpawner : MonoBehaviour {

	public Vector2 timeMinMax;
	public GameObject prefab;
	public float dst = 250;
	Transform playerT;

	void Start () {
		playerT = FindObjectOfType<Player> ().transform;
		StartCoroutine (Spawn ());
	}

	IEnumerator Spawn() {
		while (true) {
			yield return new WaitForSeconds (Random.Range (timeMinMax.x, timeMinMax.y));

			Vector2 inCircle = Random.insideUnitCircle * (GameConstants.gameRadius - 15);
			Vector3 pos = Vector3.forward * (playerT.position.z + dst) + new Vector3(inCircle.x,inCircle.y);
			Instantiate (prefab, pos, Quaternion.identity);
		}
	}
}
