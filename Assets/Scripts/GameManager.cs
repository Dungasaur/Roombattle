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
	public Text scoreText1, scoreText2, scoreText3, scoreText4;

	private int score1, score2, score3, score4;
	public int numberOfPlayers;
    
    void Awake()
    {
        if(instance !=null)
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

		roombas = FindObjectsOfType<RoombaController>();
		cursors = FindObjectsOfType<Cursor>();
		//Max number in scene on load. Once the objects are gotten, set them inactive, so only the necessary ones are in use.
		foreach(var roomba in roombas)
		{
			roomba.gameObject.SetActive(false);
		}
		foreach(var cursor in cursors)
		{
			cursor.gameObject.SetActive(false);
		}
		//populate with all roombas
		for (int i = 0; i < numberOfPlayers; i++)
		{
			roombas[i].gameObject.SetActive(true);
			cursors[i].gameObject.SetActive(true);
			cursors[i].playerNumber = roombas[i].playerNumber = i + 1;
			cursors[i].gameObject.GetComponentInChildren<Image>().color = roombas[i].col;
		}
	}



	public void ChangeScore(int amount, int player)
	{
		switch(player)
		{
			case 1:
				score1 += amount;
				scoreText1.text = amount.ToString();
				break;
			case 2:
				score2 += amount;
				scoreText2.text = amount.ToString();
				break;
			case 3:
				score3 += amount;
				scoreText3.text = amount.ToString();
				break;
			case 4:
				score4 += amount;
				scoreText4.text = amount.ToString();
				break;
			default:
				break;
		}
	}

	public void SetScore(int amount, int player)
	{
		switch (player)
		{
			case 1:
				score1 = amount;
				scoreText1.text = amount.ToString();
				break;
			case 2:
				score2 = amount;
				scoreText2.text = amount.ToString();
				break;
			case 3:
				score3 = amount;
				scoreText3.text = amount.ToString();
				break;
			case 4:
				score4 = amount;
				scoreText4.text = amount.ToString();
				break;
			default:
				break;
		}
	}
}
