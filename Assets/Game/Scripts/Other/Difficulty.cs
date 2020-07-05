using UnityEngine;

public static class Difficulty{

	[SerializeField] private static float secondsToMaxDifficulty = 60;

	public static float GetDifficultyPercent(){
		return Mathf.Clamp01(Time.timeSinceLevelLoad / secondsToMaxDifficulty);
	}
}
