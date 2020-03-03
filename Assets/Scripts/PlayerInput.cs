using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	// Strings for Input
	public string fire1, fire2, fire3, horizontal, vertical, submit, cancel;
	float hValue, vValue;
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
}
