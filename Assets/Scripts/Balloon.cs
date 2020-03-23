using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : MonoBehaviour
{
	public RoombaController papaRoomba;
	private void OnTriggerEnter(Collider other)
	{
		papaRoomba.BalloonPopped();
		gameObject.SetActive(false);
	}

	public void Reset()
	{
		gameObject.SetActive(true);
	}
}
