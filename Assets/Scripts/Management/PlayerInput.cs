using System.Collections;
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
			Fire1Down();
		}

		if(Input.GetButtonUp(fire1))
		{
			Fire1Up();
		}
		if (Input.GetButtonDown(fire2))
		{
			Fire2Down();
		}
		if(Input.GetButtonUp(fire2))
		{
			Fire2Up();
		}
		if (Input.GetButtonDown(fire3))
		{
			Fire3();
		}

		hValue = Input.GetAxis(horizontal);
		vValue = Input.GetAxis(vertical);
	}

	protected virtual void Fire1Down()
	{

	}

	protected virtual void Fire1Up()
	{

	}
	protected virtual void Fire2Down()
	{

	}
	protected virtual void Fire2Up()
	{

	}
	protected virtual void Fire3()
	{
		
	}

}
