using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metal : MonoBehaviour {

	public Vector2 glowSizeMinMax;
	public float glowSpeed;


	public Vector2 minSpeedMinMax;
	public Vector2 maxSpeedMinMax;
	public Transform glowT;
	public Transform graphicT;
	Material glowMat;

	const float rotMin = 20;
	const float rotMax = 180;
	float rotMultiplier;

	float speed;
	Transform camT;
	float scalePercent;
	const float appearTime = 1;


	void Start() {
		speed = Random.Range (Mathf.Lerp (minSpeedMinMax.x, minSpeedMinMax.y, Difficulty.difficultyPercent), Mathf.Lerp (maxSpeedMinMax.x, maxSpeedMinMax.y, Difficulty.difficultyPercent));
		//print (speed);
		rotMultiplier = Random.Range (rotMin, rotMax);
		graphicT.eulerAngles = Random.insideUnitSphere * 180f;

		glowMat = glowT.GetComponent<MeshRenderer> ().material;
		camT = Camera.main.transform;
	}

	void Update () {

		if (scalePercent < 1) {
			scalePercent = Mathf.Clamp01 (scalePercent + Time.deltaTime / appearTime);
			//transform.localScale = Vector3.one * scalePercent;
		}

		transform.Translate (-Vector3.forward * speed * Time.deltaTime, Space.World);
		graphicT.Rotate (Vector3.right * Time.deltaTime * rotMultiplier, Space.Self);

		float glowPercent = Mathf.PingPong (Time.time * glowSpeed, 1);
		float glowSize = Mathf.Lerp (glowSizeMinMax.x, glowSizeMinMax.y, glowPercent);
		float glowAlpha = Mathf.Lerp (0.05f, .55f, glowPercent);

		glowT.localScale = Vector3.one * glowSize;
		glowMat.color = new Color (glowMat.color.r, glowMat.color.g, glowMat.color.b, glowAlpha);
		// face cam
		glowT.rotation = Quaternion.FromToRotation(transform.forward,(transform.position-camT.position));


		if (transform.position.z < camT.position.z) {
			Destroy (gameObject);

		}
	}
		


	
}
