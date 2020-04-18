using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : MonoBehaviour
{
	public RoombaController papaRoomba;
	private int balloonScore = 5;
	private void OnTriggerEnter(Collider other)
	{
		papaRoomba.BalloonPopped();
		gameObject.SetActive(false);
		other.transform.root.GetComponent<RoombaController>().GetScore(balloonScore);
	}

	public void Reset()
	{
		gameObject.SetActive(true);
	}
}
