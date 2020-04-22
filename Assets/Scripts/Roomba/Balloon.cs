using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : MonoBehaviour
{
	public RoombaController papaRoomba;
	public GameObject popParticle;
	private int balloonScore = 5;
	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.name == "Knife")
		{
			papaRoomba.BalloonPopped();
			other.transform.parent.gameObject.GetComponent<RoombaController>().GetScore(balloonScore);
			GameObject.Instantiate(popParticle, transform.parent, true);
			gameObject.SetActive(false);
		}
		
	}

	public void Reset()
	{
		gameObject.SetActive(true);
	}
}
