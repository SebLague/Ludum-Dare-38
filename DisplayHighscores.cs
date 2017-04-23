using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DisplayHighscores : MonoBehaviour {
	
	public Text[] highscoreFields;
	public Text myRankField;
	public int refreshRate = 15;
	
	void Start() {
		for (int i = 0; i < highscoreFields.Length; i ++) {
			highscoreFields[i].text = i+1 + ". Fetching...";
		}
		myRankField.text = "";

		Highscores.onHighscoresRetrieved += OnHighscoresRefresh;
		Highscores.RequestHighscores();
	}

	// Request retrieval of highscores from server
	IEnumerator RequestHighscoresRefresh() {
		yield return new WaitForSeconds(refreshRate);
		Highscores.RequestHighscores();
			
	}

	// On highscores retrieved from server
	public void OnHighscoresRefresh(ScoreInfo[] scores, bool successful) {
		if (successful) {
			UpdateUI(scores);
		}

		StartCoroutine(RequestHighscoresRefresh());
	}

	// Update UI with new scores
	public void UpdateUI(ScoreInfo[] highscoreList) {
		for (int i =0; i < highscoreFields.Length; i ++) {
			highscoreFields[i].text = i+1 + ". ";
			if (i < highscoreList.Length) {
				highscoreFields[i].text += highscoreList[i].username + ": " + highscoreList[i].score;
			}
		}


		string myRankString = Highscores.myRank.ToString ();
		int myTopScore = Highscores.myTopScore;


		if (Highscores.myRank != 0) {
			char lastRankDigit = myRankString[myRankString.Length-1];
			myRankString += NumberSuffix(lastRankDigit);
			myRankField.text = "You're ranked " + myRankString + ", with a best score of " + myTopScore;
		} else {
			myRankField.text = "";
		}
	}

	string NumberSuffix(char n) {
		if (n == '1')
			return "st";
		if (n == '2')
			return "nd";
		if (n == '3')
			return "rd";

		return "th";
	}

	void OnDestroy() {
		Highscores.onHighscoresRetrieved -= OnHighscoresRefresh;
	}
}