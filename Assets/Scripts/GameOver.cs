using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour {

	public event System.Action OnGameOver;
	public GameObject ui;

	bool over;
	public static GameOver instance;

	void Awake() {
		instance = this;
	}

	void Update() {

	}

	public static void EndGame() {
		instance.EndGameInstance ();
	}

	public void EndGameInstance() {
		if (!over) {
			over = true;
			if (OnGameOver != null) {
				OnGameOver ();
			}

			ui.SetActive (true);
			FindObjectOfType<ScreenFade> ().FadeTo (new Color (0, 0, 0, .4f), 2);

		}
	}
}
