using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;
	[SerializeField]
	private GameObject gameOverPanel, finalText;

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

	void Awake()
	{
		numberOfPlayers = Statics.numberOfPlayers;
		defaultTimeRemaining = timeRemaining;
		gameOver = false;
		if (instance != null)
		{
			Destroy(this);
		}
		else
		{
			instance = this;
		}
		DontDestroyOnLoad(gameObject);
	}

	void Start()
	{
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
		Debug.Log("Game is over, go to bed.");
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

		gameOverPanel.SetActive(true);
		if (winners.Count == 1)
		{// One Winner
			Debug.Log("One Winner");
			gameOverPanel.GetComponent<Image>().color = winners[0].col;
			finalText.GetComponent<Text>().text = "Player " + (winners[0].playerNumber + 1) + " wins!\nFinal Score: " + score[winners[0].playerNumber];
		}
		else
		{// Tie
			Debug.Log("Tie");
			gameOverPanel.GetComponent<Image>().color = Color.black;
			string t = "Players ";
			for (int i = 0; i < winners.Count - 1; i++)
			{
				t += (winners[i].playerNumber + 1).ToString() + ", ";
			}
			t += (winners[winners.Count - 1].playerNumber + 1).ToString() + " Tied!";
		}

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
