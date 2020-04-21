using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : MonoBehaviour
{
	public RoombaController papaRoomba;
	private int balloonScore = 5;
	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.name == "Knife")
		{
			papaRoomba.BalloonPopped();
			other.transform.parent.gameObject.GetComponent<RoombaController>().GetScore(balloonScore);
			gameObject.SetActive(false);
		}
		
	}

	public void Reset()
	{
		gameObject.SetActive(true);
	}
}
