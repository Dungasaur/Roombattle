using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	// Strings for Input
	public string fire1, fire2, fire3, horizontal, vertical, submit, cancel;
	float hValue, vValue;
	public float speed;
	private float relativeSpeed;
	private RectTransform rectTransform;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
	}
	private void Update()
	{
		if(Input.GetButton(submit))
		{
			//do submit
		}
		if(Input.GetButton(cancel))
		{
			//do cancel
		}
		if(Input.GetButton(fire1))
		{
			Fire1();
		}
		if(Input.GetButton(fire2))
		{
			Fire2();
		}
		if(Input.GetButtonDown(fire3))
		{
			Fire3();
		}

		hValue = Input.GetAxis(horizontal);
		vValue = Input.GetAxis(vertical);
	}


	
	void Fire1()
	{

	}

	void Fire2()
	{

	}

	void Fire3()
	{

	}

	private void FixedUpdate()
	{

		//var y = Screen.Height - Screen.Height / 2;      // 50% 
		//var y = Screen.Height - Screen.Height / 10;    // 10% 
		//var y = Screen.Height - Screen.Height / 5;      // 20% 
		//var y = Screen.Height - Screen.Height / 3;      // 33% 

		// Set a speed relative to a 1920x1080 screen. Get screen size and area. howmuch bigger the screen is than 100x100, multiply speed by that ratio to get the conistent speed.

		Vector2 position = rectTransform.anchoredPosition;

		rectTransform.Translate(new Vector3(hValue * speed, vValue * speed, 0));
		Debug.Log(rectTransform.position);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		speed /= 3f;
	}
	private void OnCollisionExit2D(Collision2D collision)
	{
		speed *= 3f;
	}
}
