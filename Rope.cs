using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour {

	public float strengthFac = 1;
	public Transform end;
	public int segments = 10;
	public int fixedSegments = 3;

	public Vector3[] positions { get; private set; }
	float segmentLength;
	LineRenderer line;

	void Start () {

		line = GetComponent<LineRenderer> ();
		line.numPositions = segments;

		float dstToEnd = Vector3.Distance (transform.position, end.position);
		segmentLength = dstToEnd / segments;

		positions = new Vector3[segments];
		positions [0] = transform.position;
		Vector3 dir = (end.position - transform.position).normalized;
		for (int i = 1; i < segments; i++) {
			positions [i] = transform.position + dir * segmentLength * i;
		}
	}


	void LateUpdate () {
		positions [0] = transform.position;
		for (int i = 1; i < positions.Length; i++) {
			Vector3 dir = (positions [i - 1] - positions [i]).normalized;
			positions [i] = positions [i - 1] - dir * segmentLength;

			if (i <= fixedSegments) {
				Vector3 fixedPos = transform.position - transform.forward * segmentLength * i;
				float strength = 1-((float)i / (float)fixedSegments);
				strength *= strengthFac;
				positions [i] = positions [i] * (1 - strength) + fixedPos * strength;
			}


		}
		line.SetPositions (positions);
	}

	// some hand smoothing for tethers in main menu demo thingy coz detaching :/
	public void WorldPosUpdated() {
		line.SetPosition (positions.Length - 1, end.position);
		float str = .7f;
		line.SetPosition (positions.Length - 2, end.position * str + line.GetPosition(positions.Length - 2) * (1-str));
		str = .3f;
		line.SetPosition (positions.Length - 3, line.GetPosition(positions.Length - 2) * str + line.GetPosition(positions.Length - 3) * (1-str));
		str = .1f;
		line.SetPosition (positions.Length - 4, line.GetPosition(positions.Length - 3) * str + line.GetPosition(positions.Length - 4) * (1-str));
	}

	public Vector3 lastPos {
		get {
			return positions [positions.Length - 1];
		}
	}

	public Vector3 firstPos {
		get {
			return positions [0];
		}
	}

}
