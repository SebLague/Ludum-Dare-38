using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldDestroy : MonoBehaviour {

	public float force;
	public Rigidbody[] pieces;
	public Transform centre;

	public GameObject explosion;


	public void DoDestroy(float scale = 1) {
		
		foreach (Rigidbody r in pieces) {
			r.transform.localScale = Vector3.one * scale;
			Vector3 dir = (r.position - centre.position).normalized;
			r.AddForce (dir * force, ForceMode.VelocityChange);
			r.AddTorque (dir * force, ForceMode.VelocityChange);
			//r.AddExplosionForce(force,centre.position,explosionRadius);
		}

		GameObject fx = Instantiate (explosion, transform.position, Quaternion.identity) as GameObject;
		fx.transform.localScale = Vector3.one * scale;
	}
}
