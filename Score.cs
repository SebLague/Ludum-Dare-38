using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {

	public Text scoreText;
	public Text gameOverScoreText;
	public Text multiplierText;

	SecureInt currentMultiplier = 1;
	SecureInt score;
	float scoreF;

	IEnumerator currentAnimRo;
	bool gameOver;


	void Start () {

		Difficulty.OnGameStart (); // why is the score class responsible for doing this? Good question!

		gameOverScoreText.gameObject.SetActive (false);
		FindObjectOfType<Player> ().MetalEaten += OnMultiplierIncrease;
		FindObjectOfType<GameOver> ().OnGameOver += OnGameOver ;
		multiplierText.gameObject.SetActive (false);

		StartCoroutine (UpdateScore ());
	}
	
	IEnumerator UpdateScore() {
		while (true) {
			//yield return new WaitForSeconds(1);
			yield return null;
			if (gameOver) {
				break;
			} else {
				scoreF += currentMultiplier * Time.deltaTime;
				if (scoreF - (int)scoreF > 1 + currentMultiplier) {
					Debug.LogError ("Cheat detected");
					scoreF = 0;
				}
				score = (int)scoreF;
				scoreText.text = ((int)score).ToString ("D5");
			}
		}
	}

	void OnMultiplierIncrease() {
		if (!gameOver) {
			currentMultiplier++;
			multiplierText.text = "X" + (int)currentMultiplier;

			if (currentAnimRo != null) {
				StopCoroutine (currentAnimRo);
			}
			currentAnimRo = AnimateMultiplierText ();
			StartCoroutine (currentAnimRo);
		}

	}

	IEnumerator AnimateMultiplierText() {
		float time = 1;
		float percent = 0;

		float startScale = .2f;

		multiplierText.transform.localScale = Vector3.one * startScale;
		multiplierText.gameObject.SetActive (true);
		multiplierText.color = new Color (multiplierText.color.r, multiplierText.color.g, multiplierText.color.b, 1);
		multiplierText.CrossFadeAlpha (0, time, true);
			
		while (percent <= 1) {
			percent += Time.deltaTime / time;
			multiplierText.transform.localScale = Vector3.one * Mathf.Lerp (startScale, 1, percent);
			//multiplierText.CrossFadeAlpha(

			yield return null;

		}

		multiplierText.gameObject.SetActive (false);
	}

	void OnGameOver() {
		if (!gameOver) {
			gameOverScoreText.gameObject.SetActive (true);
			gameOverScoreText.text = scoreText.text;
			scoreText.gameObject.SetActive (false);
			multiplierText.gameObject.SetActive (false);
			gameOver = true;

			Highscores.SubmitHighscore (score);
		}
	}
}
