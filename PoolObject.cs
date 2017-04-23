using UnityEngine;
using System.Collections;

public class PoolObject : Destructable {

	protected Transform camT;

	public void SetCam(Transform t) {
		camT = t;
	}

	public virtual void OnObjectReuse() {
		gameObject.SetActive (true);
	}

	protected void Destroy() {
		gameObject.SetActive (false);
	}
}
