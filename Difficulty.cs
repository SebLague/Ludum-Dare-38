using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Difficulty {

	public const float timeToMaxDifficulty = 3.5f * 60;
	static bool useDebugVal;
	static float debugDifficultyPercent;

	static float gameStartTime;

	public static void OnGameStart() {
		gameStartTime = Time.time;
	}

	public static float difficultyPercent {
		get {
			if (useDebugVal) {
				return debugDifficultyPercent;
			}
			return Mathf.Clamp01((Time.time-gameStartTime) / Difficulty.timeToMaxDifficulty);
		}
		set {
			useDebugVal = true;
			debugDifficultyPercent = value;
		}
	}

	//public animatC


}
	
