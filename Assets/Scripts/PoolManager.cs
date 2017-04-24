using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour {

	Queue<PoolObject> poolQueue = new Queue<PoolObject> ();

	static PoolManager _instance;

	public static PoolManager instance {
		get {
			if (_instance == null) {
				_instance = FindObjectOfType<PoolManager> ();
			}
			return _instance;
		}
	}

	public void CreatePool(PoolObject prefab, int poolSize) {

		GameObject poolHolder = new GameObject (prefab.name + " pool");
		poolHolder.transform.parent = transform;

		for (int i = 0; i < poolSize; i++) {
			PoolObject newObject = Instantiate (prefab) as PoolObject;
			poolQueue.Enqueue (newObject);
			newObject.transform.SetParent (poolHolder.transform);
			newObject.gameObject.SetActive (false);
		}


	}

	public bool TryInstantiate(Vector3 position, Quaternion rotation, Transform camRef) {

		PoolObject objectToReuse = poolQueue.Peek ();

		if (!objectToReuse.gameObject.activeSelf) {
			poolQueue.Dequeue ();
			poolQueue.Enqueue (objectToReuse);

			objectToReuse.transform.position = position;
			objectToReuse.transform.rotation = rotation;
			objectToReuse.OnObjectReuse ();
			objectToReuse.SetCam (camRef); // this clearly belongs here lolol
			return true;
		}

		return false;
	}
		
}

