using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SfxLibrary : MonoBehaviour {

	public SfxGroup[] sfxGroups;
	public SfxClip[] sfxClips;

	Dictionary<string,AudioClip[]> groupDictionary = new Dictionary<string, AudioClip[]>();
	Dictionary<string,AudioClip> clipDictionary = new Dictionary<string, AudioClip>();


	void Awake() {
		for (int i =0; i < sfxGroups.Length; i ++) {
			groupDictionary.Add(sfxGroups[i].name, sfxGroups[i].clips);
		}
		for (int i =0; i < sfxClips.Length; i ++) {
			clipDictionary.Add(sfxClips[i].name, sfxClips[i].clip);
		}
	}

	/// <summary>
	/// Returns clip from library with specified name.
	/// If name points to a group, a random clip from that group is returned.
	/// </summary>
	public AudioClip GetClip(string name) {
		if (clipDictionary.ContainsKey (name)) {
			return clipDictionary[name];
		}
		if (groupDictionary.ContainsKey (name)) {
			return groupDictionary[name][Random.Range(0,groupDictionary[name].Length)];
		}
		return null;
	}

	[System.Serializable]
	public class SfxGroup : ID {
		public AudioClip[] clips;
	}

	[System.Serializable]
	public class SfxClip : ID {
		public AudioClip clip;
	}

	[System.Serializable]
	public class ID {
		public string name;
	}
}
