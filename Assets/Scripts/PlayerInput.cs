﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	[SerializeField]
	private string fire1, fire2, fire3, horizontal, vertical, submit, cancel;

	public float hValue, vValue;

	// Update is called once per frame
	protected virtual void Update()
	{
		if (Input.GetButtonDown(submit))
		{
			//do submit
		}
		if (Input.GetButtonDown(cancel))
		{
			//do cancel
		}
		if (Input.GetButtonDown(fire1))
		{
			Fire1();
		}
		if (Input.GetButtonDown(fire2))
		{
			Fire2();
		}
		if (Input.GetButtonDown(fire3))
		{
			Fire3();
		}

		hValue = Input.GetAxis(horizontal);
		vValue = Input.GetAxis(vertical);
	}

	protected virtual void Fire1()
	{

	}
	protected virtual void Fire2()
	{

	}
	protected virtual void Fire3()
	{
		
	}

}
