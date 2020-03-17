using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : PlayerInput
{
	// Strings for Input
	
	public float speed;
	public float relativeSpeed;
	private RectTransform rectTransform;
	RaycastHit hit;

	private void Awake()
	{
		rectTransform = GetComponentInChildren<RectTransform>();
		
	}

	private void Start()
	{
		SetRelativeSpeed();
	}
	private void Update()
	{
	}

	private void FixedUpdate()
	{
		
		//SetRelativeSpeed();
		//var y = Screen.Height - Screen.Height / 2;      // 50% 
		//var y = Screen.Height - Screen.Height / 10;    // 10% 
		//var y = Screen.Height - Screen.Height / 5;      // 20% 
		//var y = Screen.Height - Screen.Height / 3;      // 33% 

		// Set a speed relative to a 1920x1080 screen. Get screen size and area. howmuch bigger the screen is than 100x100, multiply speed by that ratio to get the conistent speed.

		Vector2 position = rectTransform.anchoredPosition;

		rectTransform.Translate(new Vector3(hValue * relativeSpeed,vValue * relativeSpeed, 0));
		if(rectTransform.position.y>Screen.height)
		{
			rectTransform.position = new Vector2(rectTransform.position.x, Screen.height);
		}
		else if(rectTransform.position.y < 0)
		{
			rectTransform.position = new Vector2(rectTransform.position.x, 0);
		}

		if(rectTransform.position.x>Screen.width)
		{
			rectTransform.position = new Vector2(Screen.width, rectTransform.position.y);
		}
		else if (rectTransform.position.x < 0)
		{
			rectTransform.position = new Vector2(0, rectTransform.position.y);
		}
	}
	private void OnCollisionEnter2D(Collision2D collision)
	{
		relativeSpeed /= 3f;
		Debug.Log("Collided");
	}
	private void OnCollisionExit2D(Collision2D collision)
	{
		relativeSpeed *= 3f;
		Debug.Log("Exited");
	}
	protected override void Fire1()
	{
		PlaceDirt();
	}
	
	void SetRelativeSpeed()
	{
		// current speed is based off 1024*768 resolution. Get ratio of current screen size and adjust speed to match ratio.
		relativeSpeed = Screen.width / (speed * 115);

	}

	void PlaceDirt()
	{
		if(CastDown())
		{
			if(hit.collider.gameObject.tag=="Ground")
			{
				GameObject dirt = ObjectPool.instance.GetDirt();
				dirt.SetActive(true);
				dirt.transform.position = hit.point;

			}
		}
	}

	bool CastDown()
	{
		bool hitSomething = false;
		Ray ray =Camera.main.ViewportPointToRay(new Vector3(rectTransform.position.x / Screen.width, rectTransform.position.y / Screen.height));
		if (Physics.Raycast(ray,out hit))
		{
			Debug.Log(hit.collider.gameObject.name);
			hitSomething = true;
		}
		else
		{
			Debug.Log("Missed");
		}
		return hitSomething;
	}

}
