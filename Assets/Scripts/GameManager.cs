using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;
	// All 4 roombas
	public GameObject roomba1, roomba2, roomba3, roomba4;
	public GameObject cursor1, cursor2, cursor3, cursor4;
	// related ui panels for roombas.
	public Text scoreText1, scoreText2, scoreText3, scoreText4;

	private int score1, score2, score3, score4;
    
    void Awake()
    {
        if(instance !=null)
		{
			Destroy(this);
		}
		instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
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
