using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;
	// All 4 roombas
	public RoombaController[] roombas;
	public Cursor[] cursors;

	// related ui panels for roombas.
	public Text[] scoreText;
	public Text timer;
	private int[] score;
	private int startingScore = 5;

	public int numberOfPlayers;

	private int timeRemaining;
	private IEnumerator timerRoutine;

	void Awake()
	{
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
		timerRoutine = LevelTimer();
		timeRemaining = 30;
		// Start Timer
		StartCoroutine(timerRoutine);
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
			cursors[i].gameObject.SetActive(true);
			cursors[i].playerNumber = i;
			roombas[i].playerNumber = i;
			cursors[i].gameObject.GetComponentInChildren<Image>().color = roombas[i].col;
			SetScore(startingScore, i);
		}
	}

	public void ResetScore(int player)
	{
		if(score[player]<startingScore)
		{
			SetScore(startingScore, player);
		}
	}

	public void ChangeScore(int amount, int player)
	{
		score[player] += amount;
		if (score[player] < 0)
		{
			score[player] = 0;
		}
		DisplayScore(player);
	}

	public void SetScore(int amount, int player)
	{
		score[player] = amount;
		DisplayScore(player);
	}

	public bool ReduceScore(int amount, int player)
	{
		bool canReduce = false;
		if(score[player]>0)
		{
			canReduce = true;
		}
		if (canReduce)
		{
			ChangeScore(-amount, player);
		}
		return canReduce;
	}
	void DisplayScore(int player)
	{
		scoreText[player].text = score[player].ToString();
	}

	public void GameOver()
	{
		Debug.Log("Game is over, go to bed.");
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
