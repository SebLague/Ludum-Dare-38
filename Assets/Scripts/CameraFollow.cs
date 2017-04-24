using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {


	public Transform target;
	public float smoothTime = .2f;
	const float constraintCircleBuffer = 1;
	const float boundsRadius = GameConstants.gameRadius - CameraFollow.constraintCircleBuffer;

	public bool isConstrained { get; private set; }

	float followDst;
	Vector3 smoothMoveV;

	void Start() {
		followDst = target.position.z - transform.position.z;

	}

	void LateUpdate () {
		Vector3 followPoint = target.position + Vector3.back * followDst + Vector3.up * 2;
		Vector2 follow2D = new Vector2 (followPoint.x, followPoint.y);
		float currentSmoothTime = smoothTime;

		float sqrDstFromCircleCentre = follow2D.sqrMagnitude;
		isConstrained = false;
		if (sqrDstFromCircleCentre >boundsRadius*boundsRadius) {
			isConstrained = true;
			Vector2 offset2D = follow2D.normalized * boundsRadius;
			followPoint = new Vector3 (offset2D.x, offset2D.y, followPoint.z);
		}


		transform.position = Vector3.SmoothDamp (transform.position, followPoint, ref smoothMoveV, currentSmoothTime);
		transform.position = followPoint;


	
	}

}
