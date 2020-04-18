using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class HighScore
{
	public string nickname;
	public int score;

	public HighScore()
	{
		nickname = null;
		score = 0;
	}

	public HighScore(string n, int s)
	{
		nickname = n;
		score = s;
	}
}

public class ScoreManager: MonoBehaviour
{
	public Text[] leaderboardSlots;

	private HighScore[] defaultHighScoreList = { new HighScore("NCG", 15), new HighScore("BAD", 13), new HighScore("CAV", 13), new HighScore("JHN", 10), new HighScore("ABC", 20) };

	[SerializeField]
	private HighScore[] currentScoreList = new HighScore[5];

	private SaveContainer sc;

	private void Awake()
	{
		ResetCurrentScoreList();
		UpdateLeaderboard();
	}
	public void SaveScores()
	{
		sc = new SaveContainer(currentScoreList);
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Open(Application.persistentDataPath + "/RoomBattle.sav", FileMode.OpenOrCreate);
		bf.Serialize(file, sc);
		file.Close();
		Debug.Log("Saved");
	}

	public void LoadScores()
	{
		if (File.Exists(Application.persistentDataPath + "/RoomBattle.sav"))
		{
			Stream stream = File.Open(Application.persistentDataPath + "/RoomBattle.sav", FileMode.Open);
			BinaryFormatter bformatter = new BinaryFormatter();
			sc = (SaveContainer)bformatter.Deserialize(stream);
			stream.Close();
			currentScoreList = sc.highScores;
			UpdateLeaderboard();
			Debug.Log("Loaded");
			
		}
	}

	public void OrganizeScoreList()
	{
		Array.Sort(currentScoreList, delegate (HighScore score1, HighScore score2) {
			return score1.score.CompareTo(score2.score);
		});
	}

	public void ResetCurrentScoreList()
	{
		currentScoreList = defaultHighScoreList;
		OrganizeScoreList();
	}

	public void UpdateLeaderboard()
	{
		int scoreIndex = 4;
		for(int boardIndex = 0;boardIndex<5;boardIndex++)
		{
			leaderboardSlots[boardIndex].text = (boardIndex + 1).ToString() + ": " + currentScoreList[scoreIndex].nickname + " " +
												currentScoreList[scoreIndex].score.ToString();
			scoreIndex--;
		}
	}

}

[System.Serializable]
public class SaveContainer
{
	public HighScore[] highScores;
	public SaveContainer() { }
	public SaveContainer(HighScore[] hs)
	{
		highScores = hs;
	}
}

