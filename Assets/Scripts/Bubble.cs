using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour {

	Transform player;
	public float changeDuration;
	public float timeBetweenChanges;
	public Color[] colours;
	Material mat;
	Coroutine animRoutine;
	// Use this for initialization
	void Start () {

		mat = GetComponent<MeshRenderer> ().material;

		player = FindObjectOfType<Player> ().transform;
		transform.localScale = new Vector3 (GameConstants.gameRadius, GameConstants.gameRadius, transform.localScale.z);
		animRoutine = StartCoroutine (AnimateColours ());
	}

	IEnumerator AnimateColours() {
		if (colours != null && colours.Length > 1) {
			mat.color = colours [0];
			int targetIndex = 1;
			Color targetCol = colours [targetIndex];

			while (true) {
				yield return new WaitForSeconds (timeBetweenChanges);
				Color currCol = mat.color;
				float percent = 0;

				while (percent <= 1) {
					percent += Time.deltaTime / changeDuration;
					mat.color = Color.Lerp (currCol, targetCol, percent);
					yield return null;
				}
				targetIndex++;
				targetIndex %= colours.Length;
				targetCol = colours [targetIndex];
			}
		}
		yield return null;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = Vector3.forward * player.position.z;
	}

	void OnDestroy() {
		if (animRoutine != null) {
			StopCoroutine (animRoutine);
		}
	}
		
}
