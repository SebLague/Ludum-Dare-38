using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotation : MonoBehaviour {
	public float multiplier = 1;
	public Vector3 speed;

	public bool random;
	public Vector2 multiplierMinMax;

	// Use this for initialization
	void Start () {
		if (random) {
			multiplier = Random.Range (multiplierMinMax.x, multiplierMinMax.y);
			speed = Random.insideUnitSphere;
			transform.eulerAngles = speed * 180f;
		}
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (transform.right * Time.deltaTime * speed.x * multiplier, Space.World);
		transform.Rotate (transform.up * Time.deltaTime * speed.y * multiplier, Space.World);
		transform.Rotate (transform.forward * Time.deltaTime * speed.z * multiplier, Space.World);
	}
}
