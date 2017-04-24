using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour {

	[HideInInspector]
	public float myRadius;
	protected bool dead;

	public virtual void Start() {
		myRadius = GetComponent<SphereCollider> ().radius;
	}

	public virtual void OnHit() {

	}

}
