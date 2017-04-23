using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comet : PoolObject {

	//speed
	const float speedMin_min = 15;
	const float speedMin_max = 40;
	const float speedMax_min = 38;
	const float speedMax_max = 120;

	// size
	const float sizeMin_min = .5f;
	const float sizeMin_max = 5f;
	const float sizeMax_min = 6;
	const float sizeMax_max = 12;

	const float appearTime = 2.3f;

	// rot
	const float rotMin = 20;
	const float rotMax = 180;

	float speed;
	float rotMultiplier;
	float targetScale;
	float scalePercent;

	public WorldDestroy destroyEffect;

	public override void Start ()
	{
		base.Start ();
		Transform graphicT = transform.FindChild ("Graphic");
		int index = Random.Range (0, graphicT.childCount);
		for (int i = 0; i < graphicT.childCount; i++) {
			graphicT.GetChild (i).gameObject.SetActive (i == index);
		}
	}

	public override void OnHit ()
	{
		base.OnHit ();
		if (!dead) {
			dead = true;
			WorldDestroy fx = Instantiate (destroyEffect, transform.position, Quaternion.identity) as WorldDestroy;
			fx.DoDestroy (myRadius);
			Destroy ();
		}
	}

	void Update () {
		transform.Translate (Vector3.back * speed * Time.deltaTime, Space.World);

		if (transform.position.z < camT.position.z -myRadius) {
			Destroy ();
		}

		transform.Rotate (Vector3.right * Time.deltaTime * rotMultiplier, Space.Self);

		if (scalePercent < 1) {
			scalePercent = Mathf.Clamp01 (scalePercent + Time.deltaTime / appearTime);
			myRadius = targetScale * scalePercent;
			transform.localScale = Vector3.one * myRadius;
		}

	}

	public override void OnObjectReuse ()
	{
		base.OnObjectReuse ();
		dead = false;
		speed = Mathf.Lerp (Mathf.Lerp(speedMin_min,speedMin_max,Difficulty.difficultyPercent), Mathf.Lerp(speedMax_min,speedMax_max,Difficulty.difficultyPercent), Random.value);
		targetScale = Mathf.Lerp (Mathf.Lerp(sizeMin_min,sizeMin_max,Difficulty.difficultyPercent), Mathf.Lerp(sizeMax_min,sizeMax_max,Difficulty.difficultyPercent), Random.value);
		scalePercent = 0;
		transform.localScale = Vector3.zero;
	
		//rot
		rotMultiplier = Random.Range (rotMin, rotMax);
		transform.eulerAngles = Random.insideUnitSphere * 180f;
		//speed = 20;
	}

	void OnTriggerEnter(Collider hitCollider) {
		if (hitCollider.tag != "Comet") {
			if (hitCollider.gameObject.GetComponent<Destructable>()) {
				hitCollider.gameObject.GetComponent<Destructable>().OnHit();
				OnHit ();
			}
		}
	}
}
