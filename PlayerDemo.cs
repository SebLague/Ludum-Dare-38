using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDemo: MonoBehaviour {

	public float moveSmoothSpeed = 4;
	public float rotSmoothSpeed = 3;
	public float speed = 5;
	public float step = .1f;
	public BezierPath path;
	float dst;


	void Start () {
		dst = path.CalcDst ();
		Vector3 t1 = path.Eval (0);
		transform.LookAt (path.Eval (step));
		transform.position = t1;
	}

	void Update () {
		float percent = (Time.time * speed / dst) % 1;
		float nextPercent = (percent + step) % 1;
		Vector3 target = path.Eval (percent);
		Vector3 lookTarg = path.Eval (nextPercent);

		Quaternion targetRot = Quaternion.FromToRotation (Vector3.forward, (lookTarg - transform.position).normalized);
		transform.rotation = Quaternion.Slerp (transform.rotation, targetRot, Time.deltaTime * rotSmoothSpeed);
		transform.position = target;
		//transform.rotation = targetRot;
		transform.position = Vector3.Lerp(transform.position,target,Time.deltaTime * moveSmoothSpeed);
	}

}
