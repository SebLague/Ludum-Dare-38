using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : Destructable {
	
	public bool faceTethers; // true in menu demo
	public Rope[] tethers;

	public WorldDestroy destroyEffect;

	public override void Start ()
	{
		base.Start ();
	}

	public override void OnHit ()
	{
		base.OnHit ();
		if (!dead) {
			dead = true;
			gameObject.SetActive (false);
			WorldDestroy fx = Instantiate (destroyEffect, transform.position, Quaternion.identity) as WorldDestroy;
			fx.DoDestroy (1);

			GameOver.EndGame ();
		}
	}


	void LateUpdate () {
		// set position based on tethers
		Vector3 sumPos = Vector3.zero;
		Vector3 sumLook = Vector3.zero;
		foreach (Rope r in tethers) {
			sumPos += r.lastPos;
			sumLook += r.positions[r.segments-3];
		}
		transform.position = sumPos / tethers.Length;

		if (faceTethers) {
			Vector3 originPos = sumLook / tethers.Length;
			Vector3 dirBetweenTetherEnds = (tethers [0].positions[tethers[0].segments-3] - tethers [1].positions[tethers[1].segments-3]).normalized;
			Vector3 relUp = Vector3.Cross (dirBetweenTetherEnds, (originPos - transform.position).normalized);
			//Debug.DrawRay (transform.position, relUp * 10, Color.green);
			transform.LookAt (originPos,relUp);
			foreach (Rope r in tethers) {
				r.WorldPosUpdated ();
			}
		}
	}
}
