using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public static class PersistantHighScoreList
{
	public static HighScore[] scoreList;
	//If true, means high scores have changed and will need to be saved.
	public static bool needToSave = false;
	public static HighScore[] OrganizeScoreList(HighScore[] hsArray)
	{
		Array.Sort(
					hsArray, delegate (HighScore score1, HighScore score2)
					{
						return score1.score.CompareTo(score2.score);
					}
		);
		return hsArray;
	}
}


public class ScoreManager : MonoBehaviour
{
	public Text[] leaderboardSlots;

	private HighScore[] defaultHighScoreList = { new HighScore("NCG", 1), new HighScore("BAD", 2), new HighScore("CAV", 3), new HighScore("JHN", 4), new HighScore("ABC", 5) };

	[SerializeField]
	private HighScore[] currentScoreList = new HighScore[5];

	private SaveContainer sc;

	private void Start()
	{
		if (PersistantHighScoreList.scoreList != null)
		{
			currentScoreList = PersistantHighScoreList.scoreList;
			UpdateLeaderboard();
			if (PersistantHighScoreList.needToSave)
			{
				PersistantHighScoreList.needToSave = false;
				SaveScores();
			}
		}
		else if (!File.Exists(Application.persistentDataPath + "/RoomBattle.sav"))
		{
			Debug.Log("No save file exists. resetting leaderboard.");
			ResetCurrentScoreList();
			UpdateLeaderboard();
		}
		else
		{
			LoadScores();
		}







		//if (!File.Exists(Application.persistentDataPath + "/RoomBattle.sav") || PersistantHighScoreList.scoreList == null)
		//{
		//	Debug.Log("No save file exists. resetting leaderboard.");
		//	ResetCurrentScoreList();
		//	UpdateLeaderboard();
		//}
		//else
		//{
		//	if (PersistantHighScoreList.scoreList!=null)
		//	{
		//		currentScoreList = PersistantHighScoreList.scoreList;
		//		UpdateLeaderboard();
		//		if(PersistantHighScoreList.needToSave)
		//		{
		//			PersistantHighScoreList.needToSave = false;
		//			SaveScores();
		//		}
		//	}
		//	else
		//	{
		//		LoadScores();

		//	}
		//}



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
	/// <summary>
	/// Sorts highscore list based on score. 0 index of list is the lowest score.
	/// </summary>
	public void OrganizeScoreList()
	{
		Array.Sort(
					currentScoreList, delegate (HighScore score1, HighScore score2)
					{
						return score1.score.CompareTo(score2.score);
					}
		);
		PersistantHighScoreList.scoreList = currentScoreList;
	}

	public void ResetCurrentScoreList()
	{
		currentScoreList = PersistantHighScoreList.OrganizeScoreList(defaultHighScoreList);
		PersistantHighScoreList.scoreList = currentScoreList;
		SaveScores();
	}

	public void UpdateLeaderboard()
	{
		OrganizeScoreList();
		int scoreIndex = 4;
		for (int boardIndex = 0; boardIndex < 5; boardIndex++)
		{
			leaderboardSlots[boardIndex].text = (boardIndex + 1).ToString() + ": " + currentScoreList[scoreIndex].nickname + " " +
												currentScoreList[scoreIndex].score.ToString();
			scoreIndex--;
		}
	}

}

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

