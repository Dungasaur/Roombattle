using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;
	[SerializeField]
	private GameObject gameOverPanel, finalText, highScorePanel, hsPromptText;

	// All 4 roombas
	public RoombaController[] roombas;
	public Cursor[] cursors;

	// related ui panels for roombas.
	public Text[] scoreText;
	public Text timer;
	private int[] score;
	private int startingScore = 5;

	public int numberOfPlayers;
	private int numberDead;

	private bool gameOver;

	public int timeRemaining = 30;
	private int defaultTimeRemaining;
	private IEnumerator timerRoutine;
	// For high score creation and saving
	public RoombaController winner;
	public int highScorePosition;

	void Awake()
	{
		numberOfPlayers = Statics.numberOfPlayers;
		defaultTimeRemaining = Statics.defaultTime;
		gameOver = false;
		if (instance != null)
		{
			Destroy(this);
		}
		else
		{
			instance = this;
		}
	}

	void Start()
	{
		GetComponent<AudioPlayer>().PlayRandomClip();
		LevelStart();
	}

	void LevelStart()
	{
		for (int i = 3; i > numberOfPlayers - 1; i--) 
		{
			scoreText[i].transform.parent.gameObject.SetActive(false);
		}
		Time.timeScale = 1;
		gameOverPanel.SetActive(false);
		timerRoutine = LevelTimer();
		timeRemaining = defaultTimeRemaining;
		numberDead = 0;
		gameOver = false;
		roombas = FindObjectsOfType<RoombaController>();
		cursors = FindObjectsOfType<Cursor>();
		score = new int[4];
		//Max number in scene on load. Once the objects are gotten, set them inactive, so only the necessary ones are in use.
		foreach (var roomba in roombas)
		{
			roomba.gameObject.SetActive(false);
		}
		foreach (var cursor in cursors)
		{
			cursor.gameObject.SetActive(false);
		}
		//populate with all roombas and set starting score;
		for (int i = 0; i < numberOfPlayers; i++)
		{
			roombas[i].gameObject.SetActive(true);
			roombas[i].Reset();
			cursors[i].gameObject.SetActive(true);
			cursors[i].gameOver = gameOver;
			cursors[i].playerNumber = i;
			roombas[i].playerNumber = i;
			cursors[i].gameObject.GetComponentInChildren<Image>().color = roombas[i].col;
			SetScore(startingScore, i);
		}

		// Start Timer
		StartCoroutine(timerRoutine);
	}

	public void ResetScore(int player)
	{
		if (score[player] < startingScore)
		{
			SetScore(startingScore, player);
		}
	}

	public void ChangeScore(int amount, int player)
	{
		if (!gameOver)
		{
			score[player] += amount;
			if (score[player] < 0)
			{
				score[player] = 0;
			}
			DisplayScore(player);
		}

	}

	public void SetScore(int amount, int player)
	{
		score[player] = amount;
		DisplayScore(player);
	}

	public bool ReduceScore(int amount, int player)
	{
		if (!gameOver)
		{
			bool canReduce = false;
			if (score[player] > 0)
			{
				canReduce = true;
			}
			if (canReduce)
			{
				ChangeScore(-amount, player);
			}
			return canReduce;
		}
		else
		{
			return false;
		}
	}
	void DisplayScore(int player)
	{
		scoreText[player].text = score[player].ToString();
	}

	public void IDied(int playerNumber)
	{
		if(!gameOver)
		{
			numberDead++;
			if (numberDead >= numberOfPlayers)
			{
				GameOver();
			}
		}
		
	}

	public void Reset()
	{
		foreach(RoombaController r in roombas)
		{
			r.Reset();
		}
		gameOverPanel.SetActive(false);
		LevelStart();
	}

	public void GameOver()
	{
		StopCoroutine(timerRoutine);
		gameOver = true;
		List<RoombaController> winners = WhoWon();
		foreach(RoombaController rc in roombas)
		{
			rc.GameOver();
		}

		foreach(Cursor c in cursors)
		{
			c.gameOver = true;
		}

		string winnerText;
		
		if (winners.Count == 1)
		{// One Winner
		 //Check for High Score (only if there is one winner)
			winner = winners[0];
			CheckForHighScore(winners[0]);

			gameOverPanel.GetComponent<Image>().color = winners[0].col;
			winnerText= "Player " + (winners[0].playerNumber + 1) + " wins!\nFinal Score: " + score[winners[0].playerNumber];
		}
		else
		{// Tie
			gameOverPanel.SetActive(true);
			gameOverPanel.GetComponent<Image>().color = Color.black;
			string multiWinnerText = "Players ";
			for (int i = 0; i < winners.Count - 1; i++)
			{
				multiWinnerText += (winners[i].playerNumber + 1).ToString() + ", ";
			}
			multiWinnerText += (winners[winners.Count - 1].playerNumber + 1).ToString() + " Tied!";
			winnerText = multiWinnerText;
		}
		finalText.GetComponent<Text>().text = winnerText;
	}

	private List<RoombaController> WhoWon()
	{
		List<RoombaController> winners = new List<RoombaController>();
		int winnerIndex = -1;
		if (numberDead >= numberOfPlayers - 1)
		{// Winner is the last one left alive
			foreach (RoombaController rc in roombas)
			{
				if (rc.state != RoombaController.State.Dead)
				{
					winnerIndex = rc.playerNumber;
				}
			}
			winners.Add(roombas[winnerIndex - 1]);
		}
		else
		{// if time ran out, winner is most points. tie goes to who hase more balloons.
			winners.Add(roombas[0]);
			//compare every roomba to see which is the highest scorer, starting with the second roomba compared to first.
			for (int i = 1; i < numberOfPlayers; i++)
			{

				if (score[i] > score[winners[0].playerNumber])
				{
					winners.Clear();
					winners.Add(roombas[i]);
				}
				else if (score[i] == score[winners[0].playerNumber])
				{
					if (roombas[i].balloonCount > winners[0].balloonCount)
					{
						winners.Clear();
						winners.Add(roombas[i]);
					}
					else if (roombas[i].balloonCount == winners[0].balloonCount)
					{
						winners.Add(roombas[i]);
					}
				}
			}
		}
		return winners;
	}

	public void CheckForHighScore(RoombaController rc)
	{
		int scorePlaceIndex=-1;
		for(int i=0;i<PersistantHighScoreList.scoreList.Length;i++)
		{
			if(score[rc.playerNumber]>PersistantHighScoreList.scoreList[i].score)
			{
				scorePlaceIndex = i;
				
			}
			else
			{
				i = 100;
			}
		}

		if(scorePlaceIndex!=-1)
		{// If there is a highscore change, Create new HighScore object and insert into high score array in proper place, then save scores.
			highScorePosition = scorePlaceIndex;
			highScorePanel.SetActive(true);
			highScorePanel.GetComponent<Image>().color = rc.col;
			hsPromptText.GetComponent<Text>().text = "Player " + (rc.playerNumber+1).ToString() + ", enter your initials!";
		}
		else
		{
			//Turn on gameOverPanel
			gameOverPanel.SetActive(true);
		}
	}

	public void SaveHighScore(string initials)
	{
		HighScore highScore = new HighScore(initials, score[winner.playerNumber]);
		//insert highscore into highScorePosition index of PersistantHighScoreList
		for (int i = 0; i < highScorePosition; i++)
		{
			PersistantHighScoreList.scoreList[i] = PersistantHighScoreList.scoreList[i + 1];
		}
		PersistantHighScoreList.scoreList[highScorePosition] = highScore;
		PersistantHighScoreList.needToSave = true;
		// Deactivate highscorepanel and activate end game panel.
		DeactivateHighScorePanel();
	}

	public void DeactivateHighScorePanel()
	{
		highScorePanel.SetActive(false);
		gameOverPanel.SetActive(true);
	}

	public void MainMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}

	IEnumerator LevelTimer()
	{
		while (timeRemaining > 0)
		{
			timer.text = timeRemaining.ToString();
			yield return new WaitForSeconds(1);
			timeRemaining--;
		}
		timer.text = timeRemaining.ToString();
		GameOver();
	}


}
