using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class BezierPath : MonoBehaviour {

	public bool debug;
	public Transform[] points;
	public int smoothEnd = 1;

	[Range(0.01f,.5f)]
	public float stepSize = .1f;


	public void Update() {
		if (debug) {
			float t = 0;
			Vector3 prev = Eval (0);

			while (true) {
				t += stepSize;
				Vector3 newP = Eval (t);
				Debug.DrawLine (prev, newP, Color.black);
				prev = newP;
				if (t > 1) {
					break;
				}
			}
		}

	}
		
	public float CalcDst() {
		float dst = 0;
		for (int i = 0; i < points.Length; i++) {
			dst += Vector3.Distance (points [i].position, points [(i + 1) % points.Length].position);
		}
		return dst;
	}

	public Vector3 Eval(float t) {
		t = Mathf.Clamp01 (t);
		
		Vector3 tot = Vector3.zero;
		int n = points.Length+smoothEnd;
		for (int i =0; i <= n; i ++) {
			tot += Fac(n)/(Fac(i) * Fac(n-i)) * Mathf.Pow (1-t, n-i) * Mathf.Pow(t,i) * points[i%points.Length].position;
		}

		return tot;
	}

	float Fac(int n) {
		int x = 1;
		for (int i = n; i > 0; i --) {
			x *= n;
		}
		return x;
	}
}
