using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableOneChild : MonoBehaviour {

	// Use this for initialization
	void Start () {
		int index = Random.Range (0, transform.childCount);
		for (int i = 0; i < transform.childCount; i++) {
			transform.GetChild (i).gameObject.SetActive (i == index);
		}
	}
	

}
