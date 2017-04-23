using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DependencyInjector : MonoBehaviour {

	public Dependency[] dependencies;
	static GameObject[] instantiateDependencies;

	void Awake() {

		if (instantiateDependencies == null) {
			instantiateDependencies = new GameObject[dependencies.Length];
		}

		for (int i = 0; i < dependencies.Length; i++) {
			if (instantiateDependencies [i] == null) {
				GameObject instantiatedDependency = Instantiate (dependencies [i].prefab) as GameObject;
				instantiateDependencies [i] = instantiatedDependency;
			
				if (dependencies [i].dontDestroyOnLoad) {
					DontDestroyOnLoad (instantiatedDependency);
				}
			}
		}
			
	}

	[System.Serializable]
	public struct Dependency {
		public GameObject prefab;
		public bool dontDestroyOnLoad;
	}

}