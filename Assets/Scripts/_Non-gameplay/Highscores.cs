using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Highscores : MonoBehaviour {
	
	const string privateCode = "XL7ZKt3hUE2J9RoPNu2-fwN5lNIoNyiUyJTuc-lWvKPg";
	const string publicCode = "58fd3d784389cf024cf8acbb";
	const string webURL = "http://dreamlo.com/lb/";
	const int verificationKey = 14596;

	public static event System.Action<ScoreInfo[], bool> onHighscoresRetrieved;
	public static int myRank {get; private set;}
	public static int myTopScore {get; private set;}

	static Highscores _instance;

	static Highscores instance {
		get {
			if (_instance == null) {
				_instance = GameObject.FindObjectOfType<Highscores>();
			}
			return _instance;
		}
	}

	/// Submit a highscore
	public static void SubmitHighscore(int score) {
		instance.StartCoroutine(UploadHighscore(score));
	}

	/// Request scores be downloaded from server (OnHighscoresRetrieved event will be triggered upon completion)
	public static void RequestHighscores() {
		instance.StartCoroutine(DownloadHighscores());
	}
		
	static IEnumerator UploadHighscore(int score) {
		string code = GenerateVerificationCode (score);
		WWW www = new WWW(webURL + privateCode + "/add/" + WWW.EscapeURL(UserInfo.Username + "=" + UserInfo.DeviceID) + "/" + score + "/" + code);
		yield return www;
		
		if (string.IsNullOrEmpty(www.error)) {
			RequestHighscores();
		}
		else {
			print ("Error uploading: " + www.error);
		}
	}
		
	static IEnumerator DownloadHighscores() {
		WWW www = new WWW (webURL + publicCode + "/pipe/");
		yield return www;

		if (onHighscoresRetrieved != null) {
			if (string.IsNullOrEmpty (www.error)) {
				ScoreInfo[] scores = FormatHighscores (www.text);
				onHighscoresRetrieved (scores, true);
			}
			else {
				onHighscoresRetrieved (null, false);
				print ("Error downloading: " + www.error);
			}
		}
	}
		
	static ScoreInfo[] FormatHighscores(string textStream) {
		string[] entries = textStream.Split(new char[] {'\n'}, System.StringSplitOptions.RemoveEmptyEntries);
		List<ScoreInfo> formattedEntries = new List<ScoreInfo> ();

		for (int i = 0; i <entries.Length; i ++) {
			string[] entryInfo = Split(entries[i], '|');

			// At least three pieces of information are required for valid entry
			// If not all information is present, skip entry
			if (entryInfo.Length < 3) { 
				continue;
			}

			string[] nameAndID = Split(entryInfo[0], '=');
			string verificationCode = entryInfo[2];
			int score = 0;

			// If both name and ID are not present, skip this entry
			if (nameAndID.Length<2) { 
				continue;
			}
			string username = nameAndID[0].Replace('+', ' '); // spaces are encoded as '+' symbols
			string deviceID = nameAndID[1];

			// If score is not in parsable format, skip this entry
			if (!int.TryParse(entryInfo[1],out score)) { 
				continue;
			}

			// Add score if it passes verification
			if (verificationCode == GenerateVerificationCode(score)) {
				formattedEntries.Add(new ScoreInfo(username,score));

				// Track user's top position in highscores (using deviceID)
				if (myRank == 0 || i < myRank) {
					if (deviceID == UserInfo.DeviceID) {
						myRank = formattedEntries.Count;
						myTopScore = score;
					}
				}
			}
		}
		return formattedEntries.ToArray ();
	}
		

	static string[] Split(string text, char separator) {
		return text.Split(new char[] {separator});
	}

	// Generate verification code from score
	static string GenerateVerificationCode(int score) {
		int encryptedVal = score ^ (verificationKey + score / 2 - 7);
		return encryptedVal.ToString();
	}
}

public struct ScoreInfo {
	public string username;
	public int score;

	public ScoreInfo (string username, int score)
	{
		this.username = username;
		this.score = score;
	}

}