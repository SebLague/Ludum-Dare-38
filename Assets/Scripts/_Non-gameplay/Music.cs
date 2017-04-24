using UnityEngine;
using System.Collections;

public class Music : MonoBehaviour{

	public AudioClip[] playlist;

	int trackIndex;


	void Start (){
		if (playlist != null && playlist.Length > 0) {
			StartCoroutine (PlayTracks());
		}
	}
		

	// Automatically switch to next track in playlist once current track has finished playing
	// Note: not using "yield return new WaitForSeconds(x)" as this will not work in pause menus where timescale = 0
	IEnumerator PlayTracks() {

		while (true) {
			AudioManager.instance.PlayMusic(playlist[trackIndex]);
			yield return new WaitForSeconds (playlist [trackIndex].length + 1);
				
			trackIndex++;
			trackIndex %= playlist.Length;

		}
	}

}
