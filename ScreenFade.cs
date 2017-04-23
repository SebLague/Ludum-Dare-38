using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour {

	public event System.Action onFadeComplete;
	RawImage overlay;
	IEnumerator currentFadeRoutine;

	void Awake () {
		overlay = GetComponent<RawImage> ();
	}

	public void FadeBetween(Color colA, Color colB, float time) {
		if (currentFadeRoutine != null) {
			StopCoroutine (currentFadeRoutine);
		}

		currentFadeRoutine = Fade (colA, colB, time);
		StartCoroutine (currentFadeRoutine);
	}

	public void FadeTo(Color targetCol, float time) {
		FadeBetween (Color.clear, targetCol, time);
	}

	IEnumerator Fade(Color colA, Color colB, float time) {
		float percent = 0;

		while (percent <= 1) {
			percent += Time.deltaTime * 1 / time;
			overlay.color = Color.Lerp (colA, colB, percent);
			yield return null;
		}

		if (onFadeComplete != null) {
			onFadeComplete ();
		}

	}

}
